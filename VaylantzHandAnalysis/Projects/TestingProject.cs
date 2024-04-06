using SynchroStats;
using CardSourceGenerator;
using SynchroStats.Data;
using SynchroStats.Features.Analysis;
using SynchroStats.Data.Operations;

namespace VaylantzHandAnalysis.Projects;

internal class TestingProject : IProject
{
    private record ProjectContext(VaylantzCalculationContext Context, List<YGOCards.YGOCardName> DesiredCards, HandAnalyzer<CardGroup<YGOCards.YGOCardName>, YGOCards.YGOCardName> ProspOptimizedAnalyzer);

    private CardList<VaylantzCardGroup, YGOCards.YGOCardName> OriginalCardList { get; }
    private List<YGOCards.YGOCardName> DesiredCards { get; }
    private VaylantzCalculationContext Context { get; }

    public string ProjectName { get; } = nameof(TestingProject);

    public TestingProject(CardList<VaylantzCardGroup, YGOCards.YGOCardName> cardList, IEnumerable<YGOCards.YGOCardName> desiredCards, VaylantzCalculationContext context)
    {
        OriginalCardList = cardList;
        DesiredCards = desiredCards.ToList();
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Run(IHandAnalyzerOutputStream outputStream)
    {
        var cardList = OriginalCardList;
        var analyzerArgs = HandAnalyzerBuildArguments.Create("Test Analyzer", 5, cardList);
        var size = cardList.GetNumberOfCards();
        var analyzer = HandAnalyzer.Create(analyzerArgs);

        var prospOptimizedAnalyzerCardList = OriginalCardList.CreateSimplifiedCardList(YGOCards.C_PotofProsperity, new("misc", 1), DesiredCards);
        var prospOptimizedAnalyzerArgs = HandAnalyzerBuildArguments.Create("Test Analyzer Simplified", 5, prospOptimizedAnalyzerCardList);
        var prospOptimizedAnalyzer = HandAnalyzer.Create(prospOptimizedAnalyzerArgs);

        var projectContext = new ProjectContext(Context, DesiredCards, prospOptimizedAnalyzer);

        var comparison = HandAnalyzerComparison.Create(new[] { analyzer });
        comparison.Add("w/ Prosp", Context.PercentageFormatter, projectContext, static (analyzer, projectContext) =>
        {
            var (context, desiredCards, optimizedAnalyzer) = projectContext;
            var totalProb = 0.0;

            // Check for hands that have the cards we need.
            foreach (var hand in analyzer.Combinations)
            {
                if (hand.HasThisCard(YGOCards.C_ShinonometheVaylantzPriestess))
                {
                    totalProb += analyzer.CalculateProbability(hand);
                }
                else if (hand.HasThisCard(YGOCards.C_VaylantzWakeningSoloActivation) && Vaylantz.SoloActivationLive(analyzer, hand))
                {
                    totalProb += analyzer.CalculateProbability(hand);
                }
                else if (hand.HasThisCard(YGOCards.C_SmallWorld) && Vaylantz.SmallWorldCanFindShinonome(analyzer, context, hand))
                {
                    totalProb += analyzer.CalculateProbability(hand);
                }
            }

            // Figure out the prosperity component.
            // This will be lower than it should be, because
            // only the desired cards are in the optimizer. So,
            // it is assumed that Solo Activation and Small World
            // will work 100% of the time, which is not true.
            foreach (var hand in optimizedAnalyzer.Combinations)
            {
                if (hand.HasThisCard(YGOCards.C_ShinonometheVaylantzPriestess))
                {
                    continue;
                }
                else if (hand.HasThisCard(YGOCards.C_VaylantzWakeningSoloActivation))
                {
                    continue;
                }
                else if (hand.HasThisCard(YGOCards.C_SmallWorld))
                {
                    continue;
                }
                else if (hand.HasThisCard(YGOCards.C_PotofProsperity))
                {
                    var probOfHand = optimizedAnalyzer.CalculateProbability(hand);
                    var prospAnalyzer = optimizedAnalyzer.Remove(hand, static (group, amount) => CardGroup.Create(group.Name, amount, group.Minimum, Math.Min(amount, group.Maximum)));
                    var probOfProspTargets = 0.0;

                    foreach (var prospHand in prospAnalyzer.Combinations)
                    {
                        if (prospHand.HasAnyOfTheseCards(desiredCards))
                        {
                            probOfProspTargets += prospAnalyzer.CalculateProbability(prospHand);
                        }
                    }

                    // I believe this is a valid interpretation of these events.
                    // While the probability of finding prosperity targets hinges
                    // on whether it was drawn, the probability of drawing this hand
                    // has nothing to do with whether or not pot prosperity finds
                    // something. So, Bayes' thereom is not appropriate here.
                    totalProb += probOfHand * probOfProspTargets;
                }
            }

            return totalProb;
        });
        comparison.Add("w/o Prosp", Context.PercentageFormatter, projectContext, static (analyzer, projectContext) =>
        {
            var (context, desiredCards, optimizedAnalyzer) = projectContext;
            var totalProb = 0.0;

            foreach (var hand in analyzer.Combinations)
            {
                if (hand.HasThisCard(YGOCards.C_ShinonometheVaylantzPriestess))
                {
                    totalProb += analyzer.CalculateProbability(hand);
                }
                else if (hand.HasThisCard(YGOCards.C_VaylantzWakeningSoloActivation) && Vaylantz.SoloActivationLive(analyzer, hand))
                {
                    totalProb += analyzer.CalculateProbability(hand);
                }
                else if (hand.HasThisCard(YGOCards.C_SmallWorld) && Vaylantz.SmallWorldCanFindShinonome(analyzer, context, hand))
                {
                    totalProb += analyzer.CalculateProbability(hand);
                }
            }

            return totalProb;
        });
        comparison.Run(outputStream, Context.HandAnalyzerNameFormatter);
    }
}

public sealed class HandAssessmentCategory : IHandAnalyzerComparisonCategory<VaylantzCardGroup, YGOCards.YGOCardName>
{
    public string Name => throw new NotImplementedException();

    public string Run(IEnumerable<HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName>> analyzers)
    {
        throw new NotImplementedException();
    }
}

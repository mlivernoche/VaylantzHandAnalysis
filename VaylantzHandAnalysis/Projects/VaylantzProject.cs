using SynchroStats;
using SynchroStats.Features.Analysis;
using CardSourceGenerator;
using SynchroStats.Data.Operations;
using SynchroStats.Features.Combinations;

namespace VaylantzHandAnalysis.Projects;

internal class VaylantzProject : IProject
{
    private VaylantzCardListBuilder CardListBuilder { get; }
    private VaylantzCalculationContext Context { get; }

    public string ProjectName => "Vaylantz Main Project";

    public VaylantzProject(VaylantzCardListBuilder vaylantzCardListBuilder, VaylantzCalculationContext context)
    {
        CardListBuilder = vaylantzCardListBuilder;
        Context = context;
    }

    public void Run(IHandAnalyzerOutputStream outputStream)
    {
        var cardList = CardListBuilder.CreateCardList();
        var basicDeckList = HandAnalyzerBuildArguments.Create("Vaylantz (F)", 5, cardList);
        var analyzers = HandAnalyzer.CreateInParallel(new[]
        {
            basicDeckList,
            basicDeckList with
            {
                AnalyzerName = "Vaylantz (S)",
                HandSize = 6
            }
        });

        var comparison = HandAnalyzerComparison.Create(analyzers);
        comparison.Add("Small World is Live", Context.PercentageFormatter, Context, static (analyzer, context) =>
        {
            if (!analyzer.HasCard(YGOCards.C_SmallWorld))
            {
                return 0.0;
            }

            var live = analyzer.CalculateProbability(context, Vaylantz.SmallWorldCanFindShinonome);
            var has = analyzer.CalculateProbability(static (analyzer, hand) => hand.HasThisCard(YGOCards.C_SmallWorld));
            return live / has;
        });
        comparison.Add("Solo Activation is Live", Context.PercentageFormatter, Context, static (analyzer, context) =>
        {
            if (!analyzer.HasCard(YGOCards.C_SmallWorld))
            {
                return 0.0;
            }

            var live = analyzer.CalculateProbability(Vaylantz.SoloActivationLive);
            var has = analyzer.CalculateProbability(static (analyzer, hand) => hand.HasThisCard(YGOCards.C_VaylantzWakeningSoloActivation));
            return live / has;
        });
        comparison.Add("Play w/ 3 Non-Engine", Context.PercentageFormatter, Context, static (analyzer, context) =>
        {
            static bool HasThreeNonEngine(HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> analyzer, HandCombination<YGOCards.YGOCardName> hand)
            {
                return analyzer
                    .GetCardGroups(hand)
                    .Where(static tuple =>
                    {
                        var (group, hand) = tuple;
                        return group.Traits.HasFlag(CardTraits.Defensive);
                    })
                    .Sum(static tuple =>
                    {
                        var (group, hand) = tuple;
                        return hand.MinimumSize;
                    }) == 3;
            }

            var hasThreeNonEngine = analyzer.CalculateProbability(HasThreeNonEngine);

            var canPlay = analyzer.CalculateProbability(context, static (analyzer, context, hand) =>
            {
                return HasThreeNonEngine(analyzer, hand) && Vaylantz.HasShinonome(analyzer, context, hand);
            });

            return canPlay / hasThreeNonEngine;
        });
        comparison.Add("Has Access to Shinonome", Context.PercentageFormatter, Context, static (analyzer, context) => analyzer.CalculateProbability(context, Vaylantz.HasShinonome));
        comparison.Add("Has Access to Fenrir", Context.PercentageFormatter, Context, static (analyzer, context) => analyzer.CalculateProbability(context, Vaylantz.HasFenrir));

        HandAnalyzerComparison.RunInParallel(outputStream, comparison, Context.HandAnalyzerNameFormatter);
    }
}

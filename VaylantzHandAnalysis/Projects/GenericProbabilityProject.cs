using SynchroStats;
using SynchroStats.Data.CardSearch;
using SynchroStats.Data;
using SynchroStats.Features.Analysis;
using SynchroStats.Formatting;
using CardSourceGenerator;
using SynchroStats.Data.Operations;

namespace VaylantzHandAnalysis.Projects;

internal sealed class GenericProbabilityProject : IProject
{
    private static void RunGenericProbabilities(Range range, int deckSize, CardSearchNodeCollection<YGOCards.YGOCardName> cardSearchGraphs, IHandAnalyzerComparisonFormatter<double> formatter, IHandAnalyzerComparisonFormatter<string> handFormatter, IHandAnalyzerOutputStream outputStream)
    {
        var analyzerBuilderCollection = new List<HandAnalyzerBuildArguments<CardGroup<YGOCards.YGOCardName>, YGOCards.YGOCardName>>();

        for (var i = range.Start.Value; i <= range.End.Value; i++)
        {
            var engineGroup = new CardGroup<YGOCards.YGOCardName>()
            {
                Name = new("Engine", -1),
                Size = deckSize - i,
                Minimum = 0,
                Maximum = deckSize - i,
            };
            var nonEngineGroup = new CardGroup<YGOCards.YGOCardName>()
            {
                Name = new("Non-Engine", -2),
                Size = i,
                Minimum = 0,
                Maximum = i,
            };

            var cardList = CardList.Create<CardGroup<YGOCards.YGOCardName>, YGOCards.YGOCardName>([engineGroup, nonEngineGroup]);

            // Going first analyzer.
            var analyzerArgs = HandAnalyzerBuildArguments.Create($"E{engineGroup.Size:N0} v N{nonEngineGroup.Size:N0} (5)", 5, cardList);
            analyzerBuilderCollection.Add(analyzerArgs);

            // Going second analyzer.
            analyzerBuilderCollection.Add(analyzerArgs with { AnalyzerName = $"E{engineGroup.Size:N0} v N{nonEngineGroup.Size:N0} (6)", HandSize = 6 });
        }

        // Create hand analyzers.
        var analyzerCollection = HandAnalyzer.CreateInParallel(analyzerBuilderCollection);

        // Create a HandAnalyzerComparison, which is used for comparing hand analyzers and then outputting them to something.
        var analyzerComparer = HandAnalyzerComparison.Create(analyzerCollection);

        // Calculate probability of drawing a hand with 0 non-engine.
        analyzerComparer.Add("(==0) Non-Engine", formatter, static analyzer => analyzer.CalculateProbability(static hand => hand.CountCopiesOfCardInHand(new("Non-Engine", -2)) == 0));

        // Calculate probability of drawing a hand with 1 or 2 non-engine.
        analyzerComparer.Add("(==1 or 2) Non-Engine", formatter, static analyzer => analyzer.CalculateProbability(static hand => hand.CountCopiesOfCardInHand(new("Non-Engine", -2)) is 1 or 2));

        // Calculate probability of drawing a hand with 3 or more non-engine.
        analyzerComparer.Add("(>=3) Non-Engine", formatter, static analyzer => analyzer.CalculateProbability(static hand => hand.CountCopiesOfCardInHand(new("Non-Engine", -2)) >= 3));
        
        // Run the models.
        HandAnalyzerComparison.RunInParallel(outputStream, analyzerComparer, handFormatter);
    }

    private VaylantzCalculationContext VaylantzContext { get; }

    public string ProjectName { get; } = "Generic Probabilities";
    public Range NonEngineRange { get; }

    public GenericProbabilityProject(Range nonEngineRange, VaylantzCalculationContext vaylantzCalculationContext)
    {
        NonEngineRange = nonEngineRange;
        VaylantzContext = vaylantzCalculationContext;
    }

    public void Run(IHandAnalyzerOutputStream outputStream)
    {
        RunGenericProbabilities(NonEngineRange, 44, VaylantzContext.CardSearchGraphs, VaylantzContext.PercentageFormatter, VaylantzContext.HandAnalyzerNameFormatter, outputStream);
    }
}

using CardSourceGenerator;
using SynchroStats.Data.CardSearch;
using SynchroStats.Formatting;

namespace VaylantzHandAnalysis;

public record VaylantzCalculationContext
{
    public required CardSearchNodeCollection<YGOCards.YGOCardName> CardSearchGraphs { get; init; }
    public required IHandAnalyzerComparisonFormatter<double> PercentageFormatter { get; init; }
    public required IHandAnalyzerComparisonFormatter<string> HandAnalyzerNameFormatter { get; init; }
}

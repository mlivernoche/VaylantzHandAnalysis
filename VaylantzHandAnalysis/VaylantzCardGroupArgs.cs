using CardSourceGenerator;

namespace VaylantzHandAnalysis;

public record class VaylantzCardGroupArgs
{
    public required YGOCards.YGOCardName Name { get; init; }
    public required int Size { get; init; }
    public CardTraits CardTraits { get; init; }
}

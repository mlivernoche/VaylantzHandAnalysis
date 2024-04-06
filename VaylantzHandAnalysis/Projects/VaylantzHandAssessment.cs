using CardSourceGenerator;
using SynchroStats.Features.Assessment;
using SynchroStats.Features.Combinations;

namespace VaylantzHandAnalysis.Projects
{
    public sealed class VaylantzHandAssessment : IHandAssessment<YGOCards.YGOCardName>
    {
        public bool Included { get; }
        public HandCombination<YGOCards.YGOCardName> Hand { get; }
        public bool LosesToDroll { get; init; }
        public bool OneCardCombo { get; init; }
        public bool TwoCardCombo { get; init; }
        public int UniqueNonEngine { get; init; }
        public int NiceToHaves { get; init; }

        public VaylantzHandAssessment(bool included, HandCombination<YGOCards.YGOCardName> hand)
        {
            Included = included;
            Hand = hand;
        }
    }
}

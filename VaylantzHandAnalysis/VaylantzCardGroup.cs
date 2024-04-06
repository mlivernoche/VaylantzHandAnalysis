using SynchroStats.Data;
using CardSourceGenerator;
using SynchroStats.Features.SmallWorld;
using System.Diagnostics;

namespace VaylantzHandAnalysis
{
    [DebuggerDisplay("{Name}")]
    public record VaylantzCardGroup : ICardGroup<YGOCards.YGOCardName>, ISmallWorldCard<YGOCards.YGOCardName>
    {
        public YGOCards.YGOCardName Name { get; }
        public int Size { get; init; }
        public int Minimum => 0;
        public int Maximum => Size;
        public CardTraits Traits { get; }

        public ISmallWorldTraits? SmallWorldTraits { get; }

        public VaylantzCardGroup(YGOCards.YGOCardName name, int size)
        {
            Name = name;
            Size = size;
        }

        public VaylantzCardGroup(YGOCards.IYGOCard card, VaylantzCardGroupArgs args)
        {
            Name = card.Name;
            Size = args.Size;
            Traits = args.CardTraits;

            SmallWorldTraits = VaylantzHandAnalysis.SmallWorldTraits.TryCreate(card);
        }

        public static CardSearchNode? CreateSearchGraph(IEnumerable<YGOCards.YGOCardName> names)
        {
            static CardSearchNode? CreateGraph(int currentNode, YGOCards.YGOCardName[] cardNames)
            {
                if (currentNode == cardNames.Length)
                {
                    return null;
                }

                return new CardSearchNode(cardNames[currentNode], CreateGraph(currentNode + 1, cardNames));
            }

            return CreateGraph(0, names.ToArray());
        }
    }
}

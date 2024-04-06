using CardSourceGenerator;
using System.Collections;
using SynchroStats;

namespace VaylantzHandAnalysis;

internal sealed class VaylantzCardListBuilder : IEnumerable<VaylantzCardGroup>
{
    private DictionaryWithGeneratedKeys<YGOCards.YGOCardName, YGOCards.IYGOCard> CardData { get; }
    private DictionaryWithGeneratedKeys<YGOCards.YGOCardName, VaylantzCardGroup> Cards { get; }

    public VaylantzCardListBuilder(IEnumerable<YGOCards.IYGOCard> cards)
    {
        CardData = new DictionaryWithGeneratedKeys<YGOCards.YGOCardName, YGOCards.IYGOCard>(static card => card.Name, cards);
        Cards = new DictionaryWithGeneratedKeys<YGOCards.YGOCardName, VaylantzCardGroup>(static group => group.Name);
    }

    public VaylantzCardListBuilder Add(VaylantzCardListBuilder cardListBuilder)
    {
        foreach (var card in cardListBuilder.Cards.Values)
        {
            Cards.AddOrUpdate(card);
        }
        return this;
    }

    public VaylantzCardListBuilder Add(VaylantzCardGroupArgs args)
    {
        if (!CardData.TryGetValue(args.Name, out var card))
        {
            throw new Exception();
        }

        Cards.AddOrUpdate(new VaylantzCardGroup(card, args));
        return this;
    }

    public CardList<VaylantzCardGroup, YGOCards.YGOCardName> CreateCardList()
    {
        return CardList.Create<VaylantzCardGroup, YGOCards.YGOCardName>(Cards.Values);
    }

    public IEnumerator<VaylantzCardGroup> GetEnumerator()
    {
        IEnumerable<VaylantzCardGroup> enumerable = Cards.Values;
        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        IEnumerable enumerable = Cards.Values;
        return enumerable.GetEnumerator();
    }
}

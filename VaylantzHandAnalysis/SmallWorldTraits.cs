using CardSourceGenerator;
using CommunityToolkit.Diagnostics;
using SynchroStats.Features.SmallWorld;

namespace VaylantzHandAnalysis;

public class SmallWorldTraits : ISmallWorldTraits
{
    public int Level { get; }
    public int AttackPoints { get; }
    public int DefensePoints { get; }
    public string MonsterType { get; }
    public string MonsterAttribute { get; }

    private SmallWorldTraits(YGOCards.IYGOCard card)
    {
        Guard.IsNotNull(card.Level);
        Guard.IsNotNull(card.AttackPoints);
        Guard.IsNotNull(card.DefensePoints);
        Guard.IsNotNullOrEmpty(card.MonsterType);
        Guard.IsNotNullOrWhiteSpace(card.MonsterType);
        Guard.IsNotNullOrEmpty(card.MonsterAttribute);
        Guard.IsNotNullOrWhiteSpace(card.MonsterAttribute);

        Level = card.Level.Value;
        AttackPoints = card.AttackPoints.Value;
        DefensePoints = card.DefensePoints.Value;
        MonsterType = card.MonsterType;
        MonsterAttribute = card.MonsterAttribute;
    }

    public static ISmallWorldTraits? TryCreate(YGOCards.IYGOCard card)
    {
        if (card.Level is null)
        {
            return null;
        }

        if (card.AttackPoints is null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(card.MonsterType) || string.IsNullOrWhiteSpace(card.MonsterType))
        {
            return null;
        }

        if (string.IsNullOrEmpty(card.MonsterAttribute) || string.IsNullOrWhiteSpace(card.MonsterAttribute))
        {
            return null;
        }

        return new SmallWorldTraits(card);
    }
}

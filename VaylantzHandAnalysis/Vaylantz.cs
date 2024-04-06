using SynchroStats;
using CardSourceGenerator;
using SynchroStats.Features.Combinations;
using SynchroStats.Features.SmallWorld;
using SynchroStats.Data.Operations;
using SynchroStats.Features.Analysis;

namespace VaylantzHandAnalysis
{
    internal static class Vaylantz
    {
        public static VaylantzCardListBuilder CreateBasicCardListBuilder(IEnumerable<YGOCards.IYGOCard> cardData)
        {
            return new VaylantzCardListBuilder(cardData)
            {
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_ShinonometheVaylantzPriestess,
                    Size = 3,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.LowLevelWaterVaylantz | CardTraits.ProsperityTarget,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_SaiontheVaylantzArcher,
                    Size = 3,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.LowLevelWaterVaylantz,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_NazukitheVaylantzNinja,
                    Size = 3,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.HighLevelWaterVaylantz,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_HojotheVaylantzWarrior,
                    Size = 1,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.HighLevelWaterVaylantz,
                },

                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzBusterBaron,
                    Size = 0,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.LowLevelFireVaylantz,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzVoltageViscount,
                    Size = 3,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.LowLevelFireVaylantz,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzMadMarquess,
                    Size = 3,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.HighLevelFireVaylantz,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzDominatorDuke,
                    Size = 1,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.HighLevelFireVaylantz,
                },

                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzWorldShinraBansho,
                    Size = 1,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzWorldKonigWissen,
                    Size = 2,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_VaylantzWakeningSoloActivation,
                    Size = 3,
                    CardTraits = CardTraits.BeyondThePendMat | CardTraits.ProsperityTarget,
                },

                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_ReSolfachordDreamia,
                    Size = 1,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_DoSolfachordCutia,
                    Size = 0,
                    CardTraits = CardTraits.BeyondThePendMat,
                },

                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_PotofProsperity,
                    Size = 3,
                },

                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_SmallWorld,
                    Size = 3,
                    CardTraits = CardTraits.ProsperityTarget,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_NobledragonMagician,
                    Size = 1,
                },
            };
        }

        public static VaylantzCardListBuilder CreateNonengineCardListBuilder(IEnumerable<YGOCards.IYGOCard> cardData)
        {
            return new VaylantzCardListBuilder(cardData)
            {
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_KashtiraFenrir,
                    Size = 3,
                    CardTraits = CardTraits.Defensive,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_PressuredPlanetWraitsoth,
                    Size = 0,
                    CardTraits = CardTraits.Defensive,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_Terraforming,
                    Size = 0,
                    CardTraits = CardTraits.Defensive,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_AshBlossomJoyousSpring,
                    Size = 3,
                    CardTraits = CardTraits.Defensive,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_NibiruthePrimalBeing,
                    Size = 3,
                    CardTraits = CardTraits.Defensive,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_InfiniteImpermanence,
                    Size = 3,
                    CardTraits = CardTraits.Defensive,
                },
                new VaylantzCardGroupArgs
                {
                    Name = YGOCards.C_EffectVeiler,
                    Size = 1,
                    CardTraits = CardTraits.Defensive,
                },
            };
        }

        public static bool HasFenrir(HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> analyzer, VaylantzCalculationContext context, HandCombination<YGOCards.YGOCardName> possibleHand)
        {
            if (possibleHand.HasThisCard(YGOCards.C_KashtiraFenrir))
            {
                return true;
            }

            foreach (var card in possibleHand.GetCardsInHand())
            {
                if (!analyzer.CardGroups.TryGetValue(card.HandName, out var group))
                {
                    throw new Exception($"Card in hand \"{card.HandName}\" not in card list.");
                }

                if (context.CardSearchGraphs.HasPathBetweenNodes(card.HandName, YGOCards.C_KashtiraFenrir))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool SmallWorldCanFindShinonome(HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> analyzer, VaylantzCalculationContext context, HandCombination<YGOCards.YGOCardName> possibleHand)
        {
            if (!possibleHand.HasThisCard(YGOCards.C_SmallWorld))
            {
                return false;
            }

            var cardsInDeck = analyzer.CardGroups.Values.Minus(possibleHand, static (card, amount) => card with { Size = amount });
            var smallWorldAnalyzer = SmallWorldAnalyzer.Create(cardsInDeck);

            foreach (var card in possibleHand.GetCardsInHand())
            {
                if (smallWorldAnalyzer.HasBridge(card.HandName, YGOCards.C_ShinonometheVaylantzPriestess))
                {
                    return true;
                }

                if (!analyzer.CardGroups.TryGetValue(card.HandName, out var group))
                {
                    throw new Exception($"Card in hand \"{card.HandName}\" not in card list.");
                }

                foreach (var name in context.CardSearchGraphs.GetCardsAccessibleFromName(card.HandName))
                {
                    var deckWithoutCard = cardsInDeck.RemoveCardFromDeck(name, static (card, amount) => card with { Size = amount });
                    var newSmallWorldAnalyzer = SmallWorldAnalyzer.Create(deckWithoutCard);

                    if (newSmallWorldAnalyzer.HasBridge(name, YGOCards.C_ShinonometheVaylantzPriestess))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private readonly static YGOCards.YGOCardName[] SoloActivationDesiredCards =
        [
            YGOCards.C_ShinonometheVaylantzPriestess,
            YGOCards.C_NazukitheVaylantzNinja,
            YGOCards.C_VaylantzWorldKonigWissen,
            YGOCards.C_VaylantzWorldShinraBansho,
            YGOCards.C_PressuredPlanetWraitsoth,
            YGOCards.C_PlanetPathfinder,
            YGOCards.C_Terraforming,
            YGOCards.C_VaylantzBusterBaron,
        ];

        public static bool SoloActivationLive(HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> analyzer, HandCombination<YGOCards.YGOCardName> possibleHand)
        {
            return possibleHand.HasThisCard(YGOCards.C_VaylantzWakeningSoloActivation) && possibleHand.HasAnyOfTheseCards(SoloActivationDesiredCards);
        }

        public static bool HasShinonome(HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> analyzer, VaylantzCalculationContext context, HandCombination<YGOCards.YGOCardName> possibleHand)
        {
            return
                possibleHand.HasThisCard(YGOCards.C_ShinonometheVaylantzPriestess) ||
                SmallWorldCanFindShinonome(analyzer, context, possibleHand) ||
                SoloActivationLive(analyzer, possibleHand);
        }
    }
}

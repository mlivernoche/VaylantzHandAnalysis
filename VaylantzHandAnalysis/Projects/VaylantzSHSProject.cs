using CardSourceGenerator;
using SynchroStats;
using SynchroStats.Data.Operations;
using SynchroStats.Features.Analysis;
using SynchroStats.Features.Combinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VaylantzHandAnalysis.Projects;

public sealed class VaylantzSHSProject : IProject
{
    private static CardList<VaylantzCardGroup, YGOCards.YGOCardName> CreateCardList(IEnumerable<YGOCards.IYGOCard> cardData)
    {
        return new VaylantzCardListBuilder(cardData)
        {
            // WATER Vaylantz
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_ShinonometheVaylantzPriestess,
                Size = 3,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_SaiontheVaylantzArcher,
                Size = 3,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_NazukitheVaylantzNinja,
                Size = 1,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_HojotheVaylantzWarrior,
                Size = 1,
            },

            // FIRE Vaylantz
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzBusterBaron,
                Size = 0,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzVoltageViscount,
                Size = 3,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzMadMarquess,
                Size = 0,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzDominatorDuke,
                Size = 0,
            },

            // SPELL Vaylantz
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzWakeningSoloActivation,
                Size = 3,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzWarsThePlaceofBeginning,
                Size = 0,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzWorldShinraBansho,
                Size = 2,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_VaylantzWorldKonigWissen,
                Size = 1,
            },

            // Superheavy Samurai
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_SuperheavySamuraiMotorbike,
                Size = 3,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_SuperheavySamuraiProdigyWakaushi,
                Size = 3,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_SuperheavySamuraiSoulpiercer,
                Size = 1,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_SuperheavySamuraiMonkBigBenkei,
                Size = 1,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_SuperheavySamuraiGeneralCoral,
                Size = 1,
            },

            // Tech Options
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_WaveringEyes,
                Size = 3,
            },

            // Non Engine
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_ForbiddenChalice,
                Size = 3,
                CardTraits = CardTraits.Defensive | CardTraits.MultipleOK,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_KashtiraFenrir,
                Size = 2,
                CardTraits = CardTraits.Defensive,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_NibiruthePrimalBeing,
                Size = 3,
                CardTraits = CardTraits.Defensive,
            },
            new VaylantzCardGroupArgs()
            {
                Name = YGOCards.C_TripleTacticsTalent,
                Size = 3,
                CardTraits = CardTraits.Defensive,
            },
        }.CreateCardList();
    }

    private VaylantzCalculationContext Context { get; }
    private IReadOnlyDictionary<YGOCards.YGOCardName, YGOCards.IYGOCard> CardData { get; }

    public string ProjectName { get; } = "Vaylantz SHS";

    public VaylantzSHSProject(VaylantzCalculationContext context, IReadOnlyDictionary<YGOCards.YGOCardName, YGOCards.IYGOCard> cardData)
    {
        Context = context;
        CardData = cardData ?? throw new ArgumentNullException(nameof(cardData));
    }

    public void Run(IHandAnalyzerOutputStream outputStream)
    {
        var cardList = CreateCardList(CardData.Values);
        var handAnalyzerGoingFirst = HandAnalyzerBuildArguments.Create(ProjectName, 5, cardList);
        var handAnalyzerGoingSecond = handAnalyzerGoingFirst with { HandSize = 6 };

        var handAnalyzers = HandAnalyzer.CreateInParallel([
            handAnalyzerGoingFirst,
            handAnalyzerGoingSecond,
        ]);

        var comparison = HandAnalyzerComparison.Create(handAnalyzers);
        comparison.Add("Full Combo (1,2)", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.Probability;
        });
        comparison.Add("FC (1,2), Non Engine==0", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.UniqueNonEngine == 0);
        });
        comparison.Add("FC (1,2), Non Engine==1", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.UniqueNonEngine == 1);
        });
        comparison.Add("FC (1,2), Non Engine==2", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.UniqueNonEngine == 2);
        });
        comparison.Add("FC (1,2), Non Engine==3", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.UniqueNonEngine == 3);
        });
        comparison.Add("FC (1,2), Non Engine==4", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.UniqueNonEngine == 4);
        });
        comparison.Add("FC (1,2), Non Engine==5", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.UniqueNonEngine == 5);
        });
        comparison.Add("FC (1,2), NTH==0", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.NiceToHaves == 0);
        });
        comparison.Add("FC (1,2), NTH==1", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.NiceToHaves == 1);
        });
        comparison.Add("FC (1,2), NTH==2", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.NiceToHaves == 2);
        });
        comparison.Add("FC (1,2), NTH==3", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.NiceToHaves == 3);
        });
        comparison.Add("FC (1,2), NTH==4", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.NiceToHaves == 4);
        });
        comparison.Add("FC (1,2), NTH==5", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => (assessment.OneCardCombo || assessment.TwoCardCombo) && assessment.NiceToHaves == 5);
        });
        comparison.Add("Full Combo (1)", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo);
        });
        comparison.Add("FC (1), Non Engine==0", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo && assessment.UniqueNonEngine == 0);
        });
        comparison.Add("FC (1), Non Engine==1", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo && assessment.UniqueNonEngine == 1);
        });
        comparison.Add("FC (1), Non Engine==2", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo && assessment.UniqueNonEngine == 2);
        });
        comparison.Add("FC (1), Non Engine==3", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo && assessment.UniqueNonEngine == 3);
        });
        comparison.Add("FC (1), Non Engine==4", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo && assessment.UniqueNonEngine == 4);
        });
        comparison.Add("FC (1), Non Engine==5", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.OneCardCombo && assessment.UniqueNonEngine == 5);
        });
        comparison.Add("Full Combo (2)", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo);
        });
        comparison.Add("FC (2), Non Engine==0", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo && assessment.UniqueNonEngine == 0);
        });
        comparison.Add("FC (2), Non Engine==1", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo && assessment.UniqueNonEngine == 1);
        });
        comparison.Add("FC (2), Non Engine==2", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo && assessment.UniqueNonEngine == 2);
        });
        comparison.Add("FC (2), Non Engine==3", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo && assessment.UniqueNonEngine == 3);
        });
        comparison.Add("FC (2), Non Engine==4", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo && assessment.UniqueNonEngine == 4);
        });
        comparison.Add("FC (2), Non Engine==5", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => assessment.TwoCardCombo && assessment.UniqueNonEngine == 5);
        });
        comparison.Add("No Combo", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo);
        });
        comparison.Add("NC, Non Engine==0", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo && assessment.UniqueNonEngine == 0);
        });
        comparison.Add("NC, Non Engine==1", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo && assessment.UniqueNonEngine == 1);
        });
        comparison.Add("NC, Non Engine==2", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo && assessment.UniqueNonEngine == 2);
        });
        comparison.Add("NC, Non Engine==3", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo && assessment.UniqueNonEngine == 3);
        });
        comparison.Add("NC, Non Engine==4", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo && assessment.UniqueNonEngine == 4);
        });
        comparison.Add("NC, Non Engine==5", Context.PercentageFormatter, static analyzer =>
        {
            var assessment = analyzer.AssessHands(AssessVaylantzHands);
            return assessment.CalculateProbability(static assessment => !assessment.OneCardCombo && !assessment.TwoCardCombo && assessment.UniqueNonEngine == 5);
        });
        comparison.RunInParallel(outputStream, Context.HandAnalyzerNameFormatter);
    }

    private static VaylantzHandAssessment AssessVaylantzHands(HandCombination<YGOCards.YGOCardName> hand, HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> handAnalyzer)
    {
        var oneCardCombo = HasOneCardCombo(hand);
        var twoCardCombo = HasTwoCardCombo(hand);
        var hasFullCombo = oneCardCombo || twoCardCombo;
        var nonEngineCount = CountNonEngine(hand, handAnalyzer);

        return new VaylantzHandAssessment(hasFullCombo, hand)
        {
            OneCardCombo = oneCardCombo,
            TwoCardCombo = twoCardCombo,
            UniqueNonEngine = nonEngineCount,
            NiceToHaves = NiceToHaves(hand),
        };
    }

    private static int NiceToHaves(HandCombination<YGOCards.YGOCardName> hand)
    {
        return hand.CountCardNamesInHand([
            YGOCards.C_WaveringEyes,
            YGOCards.C_VaylantzWorldShinraBansho,
            YGOCards.C_VaylantzWorldKonigWissen,
            YGOCards.C_VaylantzVoltageViscount,
            YGOCards.C_SaiontheVaylantzArcher,
            YGOCards.C_HojotheVaylantzWarrior,
            YGOCards.C_NazukitheVaylantzNinja,
        ]);
    }

    private static int CountNonEngine(HandCombination<YGOCards.YGOCardName> hand, HandAnalyzer<VaylantzCardGroup, YGOCards.YGOCardName> handAnalyzer)
    {
        var nonEngine = 0;

        foreach (var card in hand.GetCardsInHand(handAnalyzer))
        {
            if (!card.Traits.HasFlag(CardTraits.Defensive))
            {
                continue;
            }

            // non OPT
            if (card.Traits.HasFlag(CardTraits.MultipleOK))
            {
                nonEngine += hand.CountCopiesOfCardInHand(card.Name);
            }
            // OPT
            else
            {
                nonEngine += hand.CountCardNameInHand(card.Name);
            }
        }

        return nonEngine;
    }

    private static bool HasOneCardCombo(HandCombination<YGOCards.YGOCardName> hand)
    {
        return hand.HasAnyOfTheseCards(
        [
            YGOCards.C_SuperheavySamuraiMotorbike,
            YGOCards.C_SuperheavySamuraiProdigyWakaushi,
            YGOCards.C_ShinonometheVaylantzPriestess,
        ]);
    }

    private static bool HasTwoCardCombo(HandCombination<YGOCards.YGOCardName> hand)
    {
        // We want to look at hands with only two card combos.
        if (HasOneCardCombo(hand))
        {
            return false;
        }

        if (
            hand.HasThisCard(YGOCards.C_SuperheavySamuraiSoulpiercer) &&
            hand.HasAnyOfTheseCards(
            [
                YGOCards.C_SaiontheVaylantzArcher,
                YGOCards.C_VaylantzVoltageViscount,
                YGOCards.C_VaylantzWakeningSoloActivation,
            ]))
        {
            return true;
        }

        if (
            hand.HasThisCard(YGOCards.C_VaylantzWakeningSoloActivation) &&
            hand.HasAnyOfTheseCards(
            [
                YGOCards.C_KashtiraFenrir,
                YGOCards.C_SuperheavySamuraiSoulpiercer,
                YGOCards.C_HojotheVaylantzWarrior,
                YGOCards.C_NazukitheVaylantzNinja,
                YGOCards.C_SaiontheVaylantzArcher,
                YGOCards.C_VaylantzVoltageViscount,
                YGOCards.C_SuperheavySamuraiGeneralCoral,
            ]))
        {
            return true;
        }

        return false;
    }
}

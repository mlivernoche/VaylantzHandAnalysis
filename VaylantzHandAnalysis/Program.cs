using VaylantzHandAnalysis;
using SynchroStats;
using SynchroStats.Formatting;
using CardSourceGenerator;
using SynchroStats.Data.CardSearch;
using SynchroStats.Features.Analysis;
using VaylantzHandAnalysis.Projects;

var cardData = new DictionaryWithGeneratedKeys<YGOCards.YGOCardName, YGOCards.IYGOCard>(static card => card.Name);

foreach (var path in YGOCards.Paths)
{
    var list = YGOCards.LoadCardDataFromYgoPro(path);

    foreach (var card in list)
    {
        cardData.AddOrUpdate(card);
    }
}

var cardSearchGraphs = new CardSearchNodeCollection<YGOCards.YGOCardName>()
{
    {
        YGOCards.C_Terraforming,
        new []
        {
            YGOCards.C_PressuredPlanetWraitsoth,
            YGOCards.C_VaylantzWorldKonigWissen,
            YGOCards.C_VaylantzWorldShinraBansho,
        }
    },
    {
        YGOCards.C_PressuredPlanetWraitsoth,
        new []
        {
            YGOCards.C_KashtiraFenrir,
            YGOCards.C_KashtiraRiseheart,
        }
    },
    {
        YGOCards.C_KashtiraFenrir,
        new []
        {
            YGOCards.C_KashtiraRiseheart,
        }
    },
    {
        YGOCards.C_VaylantzWakeningSoloActivation,
        new []
        {
            YGOCards.C_ShinonometheVaylantzPriestess,
            YGOCards.C_VaylantzBusterBaron,
            YGOCards.C_SaiontheVaylantzArcher,
            YGOCards.C_VaylantzVoltageViscount,
            YGOCards.C_NazukitheVaylantzNinja,
            YGOCards.C_VaylantzMadMarquess,
            YGOCards.C_HojotheVaylantzWarrior,
            YGOCards.C_VaylantzDominatorDuke,
        }
    },
    {
        YGOCards.C_ShinonometheVaylantzPriestess,
        new []
        {
            YGOCards.C_VaylantzWakeningSoloActivation,
            YGOCards.C_VaylantzWarsThePlaceofBeginning,
            YGOCards.C_VaylantzWorldKonigWissen,
            YGOCards.C_VaylantzWorldShinraBansho,
        }
    }
};

var percentFormatter = HandAnalyzerComparisonFormatter.CreateProbabilityFormatter<double>(25, 18);
var handAnalyzerNameFormatter = HandAnalyzerComparisonFormatter.CreateHandAnalyzerNamesFormatter(25, 18);
var context = new VaylantzCalculationContext
{
    CardSearchGraphs = cardSearchGraphs,
    PercentageFormatter = percentFormatter,
    HandAnalyzerNameFormatter = handAnalyzerNameFormatter,
};
var outputStream = new HandAnalyzerConsoleOutputStream();

var baseCardListBuilder = Vaylantz.CreateBasicCardListBuilder(cardData.Values);
var nonEngineCardListBuilder = Vaylantz.CreateNonengineCardListBuilder(cardData.Values);

var vaylantzCardList = new VaylantzCardListBuilder(cardData.Values)
{
    baseCardListBuilder,
    nonEngineCardListBuilder
};

var vaylantzBasicCardList = vaylantzCardList.CreateCardList();
var projects = new List<IProject>
{
    new VaylantzSHSProject(context, cardData),
    new TestingProject(vaylantzBasicCardList, new[]{ YGOCards.C_ShinonometheVaylantzPriestess, YGOCards.C_VaylantzWakeningSoloActivation, YGOCards.C_SmallWorld }, context),
    new GenericProbabilityProject(new Range(13, 17), context),
    new GenericProbabilityProject(new Range(18, 22), context),
    new VaylantzProsperityProject<VaylantzCardGroup, YGOCards.YGOCardName>(new VaylantzProsperityProjectArgs<YGOCards.YGOCardName>
    {
        Depth = 3,
        HandSize = 5,
        DeckSize = vaylantzBasicCardList.GetNumberOfCards(),
        MiscCardsName = new("misc", -1),
        PotOfProsperityName = YGOCards.C_PotofProsperity,
        Context = context,
    }, vaylantzBasicCardList, vaylantzBasicCardList.Where(static group => group.Traits.HasFlag(CardTraits.ProsperityTarget)).Select(static group => group.Name)),
    new VaylantzProsperityProject<VaylantzCardGroup, YGOCards.YGOCardName>(new VaylantzProsperityProjectArgs<YGOCards.YGOCardName>
    {
        Depth = 6,
        HandSize = 5,
        DeckSize = vaylantzBasicCardList.GetNumberOfCards(),
        MiscCardsName = new("misc", -1),
        PotOfProsperityName = YGOCards.C_PotofProsperity,
        Context = context,
    }, vaylantzBasicCardList, vaylantzBasicCardList.Where(static group => group.Traits.HasFlag(CardTraits.ProsperityTarget)).Select(static group => group.Name)),
    new VaylantzProject(vaylantzCardList, context)
};
Console.Clear();
var projectHandler = new ProjectHandler();
projectHandler.RunProjects(projects, outputStream);

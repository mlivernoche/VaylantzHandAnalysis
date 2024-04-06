using SynchroStats;
using SynchroStats.Data;
using SynchroStats.Data.Operations;
using SynchroStats.Features.Analysis;

namespace VaylantzHandAnalysis.Projects;

internal record VaylantzProsperityProjectArgs<TCardGroupName>
    where TCardGroupName : struct, IEquatable<TCardGroupName>, IComparable<TCardGroupName>
{
    public int Depth { get; init; }
    public int HandSize { get; init; }
    public int DeckSize { get; init; }
    public TCardGroupName PotOfProsperityName { get; init; }
    public TCardGroupName MiscCardsName { get; init; }
    public required VaylantzCalculationContext Context { get; init; }
}

internal class VaylantzProsperityProject<TCardGroup, TCardGroupName> : IProject
    where TCardGroup : ICardGroup<TCardGroupName>
    where TCardGroupName : struct, IEquatable<TCardGroupName>, IComparable<TCardGroupName>
{
    private record Context(TCardGroupName ProspName, TCardGroupName MiscName, int Depth, List<TCardGroupName> DesiredCards);

    private CardList<TCardGroup, TCardGroupName> OriginalCardList { get; }
    private VaylantzProsperityProjectArgs<TCardGroupName> Args { get; }
    private List<TCardGroupName> ProsperityDesiredCards { get; }

    public VaylantzProsperityProject(VaylantzProsperityProjectArgs<TCardGroupName> args, CardList<TCardGroup, TCardGroupName> cardList, IEnumerable<TCardGroupName> desiredCards)
    {
        Args = args;
        ProjectName = $"{args.PotOfProsperityName} ({args.Depth:N0})";
        OriginalCardList = cardList;
        ProsperityDesiredCards = desiredCards.ToList();
    }

    public string ProjectName { get; }

    public void Run(IHandAnalyzerOutputStream outputStream)
    {
        var prospCardList = OriginalCardList.CreateSimplifiedCardList(Args.PotOfProsperityName, Args.MiscCardsName, ProsperityDesiredCards);
        var prospBuildArgs = HandAnalyzerBuildArguments.Create(nameof(VaylantzProsperityProject<TCardGroup, TCardGroupName>), Args.HandSize, prospCardList);
        var prospHandAnalyzer = HandAnalyzer.Create(prospBuildArgs);
        var prospComparison = HandAnalyzerComparison.Create(new[] { prospHandAnalyzer });
        var context = new Context(Args.PotOfProsperityName, Args.MiscCardsName, Args.Depth, ProsperityDesiredCards);

        prospComparison.Add("Prosp Finds Target", Args.Context.PercentageFormatter, context, static (analyzer, context) =>
        {
            var (name, miscName, desiredCards, depth) = (context.ProspName, context.MiscName, context.DesiredCards, context.Depth);
            return analyzer.PotOfProsperityHitProbability(name, depth, desiredCards, static (group, amount) => CardGroup.Create(group.Name, amount, group.Minimum, Math.Min(group.Maximum, amount)));
        });
        prospComparison.Run(outputStream, Args.Context.HandAnalyzerNameFormatter);
    }
}

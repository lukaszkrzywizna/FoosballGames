using FoosballGames.Contracts;
using FoosballGames.Contracts.Exceptions;
using NodaTime;

namespace FoosballGames;

public record FoosballGame(Guid Id, LocalDateTime Start, LocalDateTime? End, ISets Sets)
{
    public static FoosballGame Initialize(CreateFoosballGame command)
    {
        return new(command.Id, LocalDateTime.FromDateTime(command.Start), null,
            new FirstSetRunning(RunningSet.New()));
    }

    public FoosballGame AddPointForTeam(Team team)
    {
        var sets = Sets.AddPointForTeam(team);
        var end = sets is FinishedSets ? LocalDateTime.FromDateTime(DateTime.Now) : (LocalDateTime?) null;
        return this with {Sets = sets, End = end};
    }
}

public interface ISets
{
    public ISets AddPointForTeam(Team team);
}

public record FirstSetRunning(RunningSet Set) : ISets
{
    public ISets AddPointForTeam(Team team)
    {
        var currentSet = Set.AddPointForTeam(team);

        return currentSet switch
        {
            FinishedSet finishedSet => new SecondSetRunning(finishedSet, RunningSet.New()),
            RunningSet runningSet => this with {Set = runningSet},
            _ => throw new ArgumentOutOfRangeException(nameof(currentSet), currentSet, "Unexpected set type.")
        };
    }
}

public record SecondSetRunning(FinishedSet FirstSet, RunningSet SecondSet) : ISets
{
    public ISets AddPointForTeam(Team team)
    {
        var currentSet = SecondSet.AddPointForTeam(team);

        return currentSet switch
        {
            FinishedSet finishedSet when finishedSet.WinnerTeam != FirstSet.WinnerTeam =>
                new ThirdSetRunning(FirstSet, finishedSet, RunningSet.New()),
            FinishedSet finishedSet when finishedSet.WinnerTeam == FirstSet.WinnerTeam =>
                new FinishedSets(FirstSet, finishedSet, null, finishedSet.WinnerTeam),
            RunningSet runningSet => this with {SecondSet = runningSet},
            _ => throw new ArgumentOutOfRangeException(nameof(currentSet), currentSet, "Unexpected set type.")
        };
    }
}

public record ThirdSetRunning(FinishedSet FirstSet, FinishedSet SecondSet, RunningSet ThirdSet) : ISets
{
    public ISets AddPointForTeam(Team team)
    {
        var currentSet = ThirdSet.AddPointForTeam(team);

        return currentSet switch
        {
            FinishedSet finishedSet => new FinishedSets(FirstSet, SecondSet, finishedSet, finishedSet.WinnerTeam),
            RunningSet runningSet => this with {ThirdSet = runningSet},
            _ => throw new ArgumentOutOfRangeException(nameof(currentSet), currentSet, "Unexpected set type.")
        };
    }
}

public record FinishedSets(FinishedSet FirstSet, FinishedSet SecondSet, FinishedSet? ThirdSet,
    Team WinnerTeam) : ISets
{
    public ISets AddPointForTeam(Team team) => throw new CannotAddPointToFinishedSet();
}

public interface ISet
{
}

public record RunningSet(byte RedTeamScore, byte BlueTeamScore) : ISet
{
    private const byte MaxPointsScore = 10;
    public static RunningSet New() => new(0, 0);

    public ISet AddPointForTeam(Team team)
    {
        var (blueScore, redScore) = team switch
        {
            Team.Blue => ((byte)(BlueTeamScore + 1), RedTeamScore),
            Team.Red => (BlueTeamScore, (byte)(RedTeamScore + 1)),
            _ => throw new ArgumentOutOfRangeException(nameof(team), team, "Unexpected team.")
        };

        if (blueScore == MaxPointsScore || redScore == MaxPointsScore)
            return new FinishedSet(RedTeamScore: redScore, BlueTeamScore: blueScore, team);

        return new RunningSet(RedTeamScore: redScore, BlueTeamScore: blueScore);
    }
}

public record FinishedSet(byte RedTeamScore, byte BlueTeamScore, Team WinnerTeam) : ISet;
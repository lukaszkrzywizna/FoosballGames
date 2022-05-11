using FoosballGames.Contracts;
using Xunit;

namespace FoosballGames.Tests;

public class FoosballGamesTests
{
    [Fact]
    public void CreateNewGame_NewGameHasOnlyFirstSetRunning()
    {
        var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
        var set = Assert.IsType<FirstSetRunning>(game.Sets);
        Assert.Equal(0, set.Set.BlueTeamScore);
        Assert.Equal(0, set.Set.RedTeamScore);
    }

    [Theory]
    [InlineData(Team.Blue)]
    [InlineData(Team.Red)]
    public void AddPointForTeam_ScoreIsIncremented(Team team)
    {
        var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
        var newGame = game.AddPointForTeam(team);
        var set = Assert.IsType<FirstSetRunning>(newGame.Sets);
        Assert.Equal(team == Team.Blue ? 1 : 0, set.Set.BlueTeamScore);
        Assert.Equal(team == Team.Blue ? 0 : 1, set.Set.RedTeamScore);
    }

    [Theory]
    [InlineData(Team.Blue)]
    [InlineData(Team.Red)]
    public void Add10PointsForTeam_SetIsFinished(Team team)
    {
        var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
        var newGame = Enumerable.Range(0, 10).Aggregate(game, (c, _) => c.AddPointForTeam(team));
        var (firstSet, _) = Assert.IsType<SecondSetRunning>(newGame.Sets);
        Assert.Equal(team == Team.Blue ? 10 : 0, firstSet.BlueTeamScore);
        Assert.Equal(team == Team.Blue  ? 0 : 10, firstSet.RedTeamScore);
    }

    [Theory]
    [InlineData(Team.Blue)]
    [InlineData(Team.Red)]
    public void OneTeamWinsTwoSetsInRow_GameIsFinished(Team team)
    {
        var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
        var newGame = Enumerable.Range(0, 20).Aggregate(game, (c, _) => c.AddPointForTeam(team));
        var sets = Assert.IsType<FinishedSets>(newGame.Sets);
        Assert.Equal(team, sets.WinnerTeam);
    }

    [Fact]
    public void TieAfterTwoSets_ThirdSetIsCreated()
    {
        var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
        var newGame = Enumerable
            .Range(0, 20).Aggregate(game, (c, n) => c.AddPointForTeam(n < 10 ? Team.Blue : Team.Red));
        Assert.IsType<ThirdSetRunning>(newGame.Sets);
    }

    [Theory]
    [InlineData(Team.Blue)]
    [InlineData(Team.Red)]
    public void OneTeamsWinsNotInRow_ThirdSetIsFinishedAndResultIsCounted(Team team)
    {
        var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
        var newGame = Enumerable
            .Range(0, 20).Aggregate(game, (c, n) => c.AddPointForTeam(n < 10 ? Team.Blue : Team.Red));
        newGame = Enumerable.Range(0, 10).Aggregate(newGame, (c, _) => c.AddPointForTeam(team));
        var sets = Assert.IsType<FinishedSets>(newGame.Sets);
        Assert.Equal(team, sets.WinnerTeam);
    }
}
using System;
using System.Linq;
using FoosballGames.Contracts;
using Xunit;

namespace FoosballGames.Tests
{
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
        [InlineData(true)]
        [InlineData(false)]
        public void AddPointForTeam_ScoreIsIncremented(bool forBlueTeam)
        {
            var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
            var newGame = game.AddPointForTeam(forBlueTeam);
            var set = Assert.IsType<FirstSetRunning>(newGame.Sets);
            Assert.Equal(forBlueTeam ? 1 : 0, set.Set.BlueTeamScore);
            Assert.Equal(forBlueTeam ? 0 : 1, set.Set.RedTeamScore);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Add10PointsForTeam_SetIsFinished(bool forBlueTeam)
        {
            var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
            var newGame = Enumerable.Range(0, 10).Aggregate(game, (c, _) => c.AddPointForTeam(forBlueTeam));
            var (firstSet, _) = Assert.IsType<SecondSetRunning>(newGame.Sets);
            Assert.Equal(forBlueTeam ? 10 : 0, firstSet.BlueTeamScore);
            Assert.Equal(forBlueTeam ? 0 : 10, firstSet.RedTeamScore);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OneTeamWinsTwoSetsInRow_GameIsFinished(bool forBlueTeam)
        {
            var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
            var newGame = Enumerable.Range(0, 20).Aggregate(game, (c, _) => c.AddPointForTeam(forBlueTeam));
            var sets = Assert.IsType<FinishedSets>(newGame.Sets);
            Assert.Equal(forBlueTeam, sets.BlueTeamWon);
        }

        [Fact]
        public void TieAfterTwoSets_ThirdSetIsCreated()
        {
            var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
            var newGame = Enumerable.Range(0, 20).Aggregate(game, (c, n) => c.AddPointForTeam(n < 10));
            Assert.IsType<ThirdSetRunning>(newGame.Sets);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OneTeamsWinsNotInRow_ThirdSetIsFinishedAndResultIsCounted(bool forBlueTeam)
        {
            var game = FoosballGame.Initialize(new CreateFoosballGame(Guid.NewGuid(), DateTime.Now));
            var newGame = Enumerable.Range(0, 20).Aggregate(game, (c, n) => c.AddPointForTeam(n < 10));
            newGame = Enumerable.Range(0, 10).Aggregate(newGame, (c, _) => c.AddPointForTeam(forBlueTeam));
            var sets = Assert.IsType<FinishedSets>(newGame.Sets);
            Assert.Equal(forBlueTeam, sets.BlueTeamWon);
        }
    }
}
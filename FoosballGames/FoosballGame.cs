using System;
using System.Linq;
using FoosballGames.Contracts;
using FoosballGames.Contracts.Exceptions;
using NodaTime;

namespace FoosballGames
{
    public record FoosballGame(Guid Id, LocalDateTime Start, LocalDateTime? End, Sets Sets)
    {
        public static FoosballGame Initialize(CreateFoosballGame command)
        {
            return new(command.Id, LocalDateTime.FromDateTime(command.Start), null,
                new FirstSetRunning(RunningSet.New()));
        }

        public FoosballGame AddPointForTeam(bool forBlueTeam)
        {
            var sets = Sets.AddPointForTeam(forBlueTeam);
            var end = sets is FinishedSets ? LocalDateTime.FromDateTime(DateTime.Now) : (LocalDateTime?) null;
            return this with {Sets = sets, End = end};
        }
    }

    public abstract record Sets
    {
        public abstract Sets AddPointForTeam(bool forBlueTeam);
    }

    public record FirstSetRunning(RunningSet Set) : Sets
    {
        public override Sets AddPointForTeam(bool forBlueTeam)
        {
            var currentSet = Set.AddPointForTeam(forBlueTeam);

            return currentSet switch
            {
                FinishedSet finishedSet => new SecondSetRunning(finishedSet, RunningSet.New()),
                RunningSet runningSet => this with {Set = runningSet},
                _ => throw new ArgumentOutOfRangeException(nameof(currentSet), currentSet, "Unexpected set type.")
            };
        }
    }

    public record SecondSetRunning(FinishedSet FirstSet, RunningSet SecondSet) : Sets
    {
        public override Sets AddPointForTeam(bool forBlueTeam)
        {
            var currentSet = SecondSet.AddPointForTeam(forBlueTeam);

            return currentSet switch
            {
                FinishedSet finishedSet when finishedSet.BlueTeamWon != FirstSet.BlueTeamWon =>
                    new ThirdSetRunning(FirstSet, finishedSet, RunningSet.New()),
                FinishedSet finishedSet when finishedSet.BlueTeamWon == FirstSet.BlueTeamWon =>
                    new FinishedSets(FirstSet, finishedSet, null, finishedSet.BlueTeamWon),
                RunningSet runningSet => this with {SecondSet = runningSet},
                _ => throw new ArgumentOutOfRangeException(nameof(currentSet), currentSet, "Unexpected set type.")
            };
        }
    }

    public record ThirdSetRunning(FinishedSet FirstSet, FinishedSet SecondSet, RunningSet ThirdSet) : Sets
    {
        public override Sets AddPointForTeam(bool forBlueTeam)
        {
            var currentSet = ThirdSet.AddPointForTeam(forBlueTeam);

            return currentSet switch
            {
                FinishedSet finishedSet => new FinishedSets(FirstSet, SecondSet, finishedSet,
                    CalculateBlueTeamWon(finishedSet)),
                RunningSet runningSet => this with {ThirdSet = runningSet},
                _ => throw new ArgumentOutOfRangeException(nameof(currentSet), currentSet, "Unexpected set type.")
            };
        }

        private bool CalculateBlueTeamWon(FinishedSet thirdSet)
        {
            return new[] {FirstSet.BlueTeamWon, SecondSet.BlueTeamWon, thirdSet.BlueTeamWon}.Sum(x => x ? 1 : 0) > 2;
        }
    }

    public record FinishedSets(FinishedSet FirstSet, FinishedSet SecondSet, FinishedSet? ThirdSet,
        bool BlueTeamWon) : Sets
    {
        public override Sets AddPointForTeam(bool forBlueTeam) => throw new CannotAddPointToFinishedSet();
    }

    public abstract record Set
    {
        public const byte MaxPointsScore = 10;
    }

    public record RunningSet(byte RedTeamScore, byte BlueTeamScore) : Set
    {
        public static RunningSet New() => new(0, 0);

        public Set AddPointForTeam(bool forBlueTeam)
        {
            var newScore = (byte) (forBlueTeam ? BlueTeamScore + 1 : RedTeamScore + 1);

            var redScore = !forBlueTeam ? newScore : RedTeamScore;
            var blueScore = forBlueTeam ? newScore : BlueTeamScore;

            if (newScore == MaxPointsScore)
                return new FinishedSet(RedTeamScore: redScore, BlueTeamScore: blueScore, RedTeamScore < BlueTeamScore);

            return new RunningSet(RedTeamScore: redScore, BlueTeamScore: blueScore);
        }
    }

    public record FinishedSet(byte RedTeamScore, byte BlueTeamScore, bool BlueTeamWon) : Set;
}
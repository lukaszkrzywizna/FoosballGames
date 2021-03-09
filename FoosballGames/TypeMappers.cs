using System;
using System.Collections.Generic;
using System.Linq;
using FoosballGames.Infrastructure;

namespace FoosballGames
{
    public static class TypeMappers
    {
        public static Contracts.FoosballGame ToContract(this FoosballGame game)
        {
            return new(
                Id: game.Id,
                Start: game.Start.ToDateTimeUnspecified(),
                End: game.End?.ToDateTimeUnspecified(),
                Sets: game.Sets.ToContract().ToArray(),
                IsFinished: game.Sets is FinishedSets,
                BlueTeamWon: game.Sets is FinishedSets finishedSets ? finishedSets.BlueTeamWon : (bool?) null
            );
        }

        public static IEnumerable<Contracts.Set> ToContract(this Sets sets)
        {
            switch (sets)
            {
                case FinishedSets finishedSets:
                    var response = finishedSets.FirstSet
                        .ToContract(1)
                        .ToSingletonEnumerable()
                        .Append(finishedSets.SecondSet.ToContract(2));
                    if (finishedSets.ThirdSet is not null)
                        response = response.Append(finishedSets.ThirdSet.ToContract(3));
                    return response;
                case FirstSetRunning firstSetRunning:
                    return firstSetRunning.Set.ToContract(1).ToSingletonEnumerable();
                case SecondSetRunning secondSetRunning:
                    return secondSetRunning.FirstSet
                        .ToContract(1)
                        .ToSingletonEnumerable()
                        .Append(secondSetRunning.SecondSet.ToContract(2));
                case ThirdSetRunning thirdSetRunning:
                    return thirdSetRunning.FirstSet
                        .ToContract(1)
                        .ToSingletonEnumerable()
                        .Append(thirdSetRunning.SecondSet.ToContract(2))
                        .Append(thirdSetRunning.ThirdSet.ToContract(3));
                default:
                    throw new ArgumentOutOfRangeException(nameof(response));
            }
        }

        public static Contracts.Set ToContract(this Set set, int number)
        {
            return set switch
            {
                FinishedSet finishedSet => new Contracts.Set(number, true, ReadTeamScore: finishedSet.RedTeamScore,
                    BlueTeamScore: finishedSet.BlueTeamScore),
                RunningSet runningSet => new Contracts.Set(number, false, ReadTeamScore: runningSet.RedTeamScore,
                    BlueTeamScore: runningSet.BlueTeamScore),
                _ => throw new ArgumentOutOfRangeException(nameof(set))
            };
        }
    }
}
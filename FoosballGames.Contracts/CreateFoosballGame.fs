namespace FoosballGames.Contracts

open System
open FoosballGames.Infrastructure.Messaging

type CreateFoosballGame = {Id: Guid; Start: DateTime} interface ICommand
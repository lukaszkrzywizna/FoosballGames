namespace FoosballGames.Contracts

open System.Collections.Generic
open FoosballGames.Contracts
open FoosballGames.Infrastructure.Messaging

type FoosballGamesResponse = {FoosballGames: FoosballGame IReadOnlyCollection}

// Record can't be empty, so we're using a normal class
type GetFoosballGames() =
    interface IQuery<FoosballGamesResponse>


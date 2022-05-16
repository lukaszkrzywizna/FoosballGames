namespace FoosballGames.OldContracts

open System
open FoosballGames.Infrastructure.Messaging

type Team =
    | Blue = 0
    | Red = 1

type AddPointForTeam = {GameId: Guid; Team: Team} interface ICommand

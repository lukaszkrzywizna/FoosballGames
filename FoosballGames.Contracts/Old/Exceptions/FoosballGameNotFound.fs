namespace FoosballGames.OldContracts.Exceptions

open FoosballGames.Infrastructure

type FoosballGameNotFound() =
    inherit DomainException("Foosball game not found.")
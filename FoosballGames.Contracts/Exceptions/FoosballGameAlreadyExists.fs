namespace FoosballGames.Contracts.Exceptions

open FoosballGames.Infrastructure

type FoosballGameAlreadyExists() =
    inherit DomainException("Foosball game already exists.")
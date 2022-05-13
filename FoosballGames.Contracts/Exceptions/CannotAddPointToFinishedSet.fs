namespace FoosballGames.Contracts.Exceptions

open FoosballGames.Infrastructure

type CannotAddPointToFinishedSet() =
    inherit DomainException("Can not add point to finished set.")
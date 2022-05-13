namespace FoosballGames.Infrastructure

open System

type DomainException(message: string) =
    inherit Exception(message)
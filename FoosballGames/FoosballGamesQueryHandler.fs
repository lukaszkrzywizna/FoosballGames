namespace FoosballGames

open FoosballGames
open FoosballGames.Contracts
open FoosballGames.Infrastructure.Messaging
open TypeMappers

type FoosballGamesQueryHandler(repository: IFoosballGamesRepository) =
    interface IQueryHandler<GetFoosballGame, FoosballGame> with
        member this.HandleAsync(query) =
            task {
                let! result = repository.Get query.Id
                return result.ToContract()
            }

    interface IQueryHandler<GetFoosballGames, FoosballGamesResponse> with
        member this.HandleAsync _ =
            task {
                let! results = repository.GetAll()

                let ordered =
                    results
                    |> Seq.map (fun x -> x.ToContract())
                    |> Seq.sortByDescending (fun x -> x.Start)
                    |> Seq.toArray

                return { FoosballGames = ordered }
            }

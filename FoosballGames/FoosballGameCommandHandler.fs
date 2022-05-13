namespace FoosballGames

open FoosballGames
open FoosballGames.Contracts
open FoosballGames.Infrastructure.Messaging

type FoosballGameCommandHandler(repository: IFoosballGamesRepository) =
    interface ICommandHandler<CreateFoosballGame> with
        member this.HandleAsync(command) =
            let game = FoosballGame.Initialize command
            repository.Add game
            
    interface ICommandHandler<AddPointForTeam> with
        member this.HandleAsync(command) =
            task {
                let! game = repository.Get command.GameId
                let newGame = game.AddPointForTeam command.Team
                do! repository.Update newGame
            }
        
module FoosballGames.CompositionRoot

open FoosballGames.Contracts

type HandleCommand = Command -> Error
//type HandleQuery = 

type Root =
    { HandleCmd: HandleCommand
    }
    
let composeRoot dbCtx =
    
    
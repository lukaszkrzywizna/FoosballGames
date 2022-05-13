module Tests

open System
open FoosballGames
open FoosballGames.Contracts
open Xunit

[<Fact>]
let ``Creating a new game cause setting first set running`` () =
    let game = FoosballGame.Initialize { Id = Guid.NewGuid(); Start = DateTime.Now }
    let set = Assert.IsType<FirstSetRunning> game.Sets;
    Assert.Equal(0uy, set.Set.BlueTeamScore);
    Assert.Equal(0uy, set.Set.RedTeamScore)
    
[<Theory>]
[<InlineData(Team.Blue)>]
[<InlineData(Team.Red)>]
let ``Adding a point to a team cause score incrementing`` team =
    let game = FoosballGame.Initialize { Id = Guid.NewGuid(); Start = DateTime.Now }
    let newGame = game.AddPointForTeam(team);
    let set = Assert.IsType<FirstSetRunning>(newGame.Sets);
    Assert.Equal((if team = Team.Blue then 1uy else 0uy), set.Set.BlueTeamScore);
    Assert.Equal((if team = Team.Blue then 0uy else 1uy), set.Set.RedTeamScore)
    
[<Theory>]
[<InlineData(Team.Blue)>]
[<InlineData(Team.Red)>]
let ``Adding maximum set points to a team cause set is finished`` team =
    let game = FoosballGame.Initialize { Id = Guid.NewGuid(); Start = DateTime.Now }
    let newGame = seq { 1..10 }
                  |> Seq.fold (fun (c: FoosballGames.FoosballGame) _ -> c.AddPointForTeam team) game 
    let set = Assert.IsType<SecondSetRunning> newGame.Sets
    Assert.Equal((if team = Team.Blue then 10uy else 0uy), set.FirstSet.BlueTeamScore);
    Assert.Equal((if team = Team.Blue then 0uy else 10uy), set.FirstSet.RedTeamScore)
    
[<Theory>]
[<InlineData(Team.Blue)>]
[<InlineData(Team.Red)>]
let ``If same team wins two sets in row then game is finished`` team =
    let game = FoosballGame.Initialize { Id = Guid.NewGuid(); Start = DateTime.Now }
    let newGame = seq{ 1..20 }
                  |> Seq.fold (fun (c: FoosballGames.FoosballGame) _ -> c.AddPointForTeam team) game 
    let set = Assert.IsType<FinishedSets> newGame.Sets
    Assert.Equal(team, set.WinnerTeam);

[<Fact>]
let ``If there is a tie after two sets in row then third set is created`` () =
    let game = FoosballGame.Initialize { Id = Guid.NewGuid(); Start = DateTime.Now }
    let newGame = seq { 1..20 }
                  |> Seq.fold (fun (c: FoosballGames.FoosballGame) n ->
                      c.AddPointForTeam (if n <= 10 then Team.Blue else Team.Red)) game 
    Assert.IsType<ThirdSetRunning> newGame.Sets

[<Theory>]
[<InlineData(Team.Blue)>]
[<InlineData(Team.Red)>]
let ``If same team wins two sets not in row then game has finished third set`` team =
    let game = FoosballGame.Initialize { Id = Guid.NewGuid(); Start = DateTime.Now }
    let newGame = seq { 1..20 }
                  |> Seq.fold (fun (c: FoosballGames.FoosballGame) n ->
                                    c.AddPointForTeam (if n <= 10 then Team.Blue else Team.Red)) game
    let newGame = seq { 1..10 }
                  |> Seq.fold (fun (c: FoosballGames.FoosballGame) _ -> c.AddPointForTeam team) newGame                   
    let set = Assert.IsType<FinishedSets> newGame.Sets
    Assert.Equal(team, set.WinnerTeam);
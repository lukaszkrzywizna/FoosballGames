module FoosballGames.TypeMappers

open System
open FoosballGames

type ISet with
    member this.ToContract number : Contracts.Set =
        match this with
        | :? FinishedSet as f ->
            { Number = number; Finished = true; RedTeamScore = f.RedTeamScore; BlueTeamScore = f.BlueTeamScore }
        | :? RunningSet as r ->
            { Number = number; Finished = false; RedTeamScore = r.RedTeamScore; BlueTeamScore = r.BlueTeamScore }
        | _ -> raise <| ArgumentOutOfRangeException("Set", this, "Unknown set type")


type ISets with
    member this.ToContract() : Contracts.Set seq =
        match this with
        | :? FinishedSets as f ->
            seq {
                yield f.FirstSet.ToContract 1
                yield f.SecondSet.ToContract 2
                match f.ThirdSet with
                | Some s -> yield s.ToContract 3
                | None -> ()
            }
        | :? FirstSetRunning as f ->
            f.Set.ToContract 1 |> Seq.singleton
        | :? SecondSetRunning as s ->
            seq {
                yield s.FirstSet.ToContract 1
                yield s.SecondSet.ToContract 2
            }
        | :? ThirdSetRunning as t ->
            seq {
                yield t.FirstSet.ToContract 1
                yield t.SecondSet.ToContract 2
                yield t.ThirdSet.ToContract 3
            }
        | _ -> raise <| ArgumentOutOfRangeException("Sets", this, "Unknown sets type")

type FoosballGame with
    member this.ToContract() : Contracts.FoosballGame =
        { Id = this.Id
          Start = this.Start.ToDateTimeUnspecified()
          End = this.End
                |> Option.map (fun x -> x.ToDateTimeUnspecified())
                |> Option.toNullable
          Sets = this.Sets.ToContract() |> Seq.toArray
          Finished = this.Sets :? FinishedSets
          WinnerTeam =
                match this.Sets with
                | :? FinishedSets as f -> Nullable f.WinnerTeam
                | _ -> Nullable()
           }

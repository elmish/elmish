module Elmish.ProgramTests

open FsCheck.NUnit
open Swensen.Unquote
open Elmish

type Model = int
type Msg =
  | Increment
  | Increment100Times

let init msgs = 0, msgs |> List.map Cmd.ofMsg |> Cmd.batch

let update msg m =
  match msg with
  | Increment -> m + 1, Cmd.none
  | Increment100Times -> m, Cmd.batch [ for _ in 1..100 -> Cmd.ofMsg Increment ] 

// [<Property>]
// let dispatchesBatch (msgs: Msg list) =  
//     let expected = (0, msgs) ||> List.fold (fun s -> function Increment -> s + 1 | Increment100Times -> s + 100)
//     let mutable counted = 0
//     let count m _ = counted <- m
    
//     async {
//       Program.mkProgram init update count
//       |> Program.runWith msgs
//     } |> Async.Start
//     System.Threading.Thread.Sleep (expected * 100)
//     counted =! expected

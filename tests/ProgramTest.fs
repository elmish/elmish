module Elmish.ProgramTests

open NUnit.Framework
open FsCheck.NUnit
open Swensen.Unquote
open Elmish

type Model = int
type Msg =
  | Increment
  | Decrement
  | Increment10Times

[<Property(MaxTest = 10, EndSize = 100)>]
let dispatchesBatch (msgs: Msg list) =  
    let init msgs = 0, msgs |> List.map Cmd.ofMsg |> Cmd.batch
    let update msg m =
        match msg with
        | Increment -> m + 1, Cmd.none
        | Decrement -> m - 1, Cmd.none
        | Increment10Times -> m, Cmd.batch [ for _ in 1..10 -> Cmd.ofMsg Increment ] 
    printfn "Folding..."
    let expected =
      (0, msgs)
      ||> List.fold (fun s -> function Increment -> s + 1 | Decrement -> s - 1 | Increment10Times -> s + 10)
    let mutable counted = 0
    let count m _ = counted <- m
    printfn "Starting..."
    async {
      Program.mkProgram init update count
      |> Program.runWith msgs
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    counted =! expected

[<Test>]
let ``update throwing exception should not crash the program`` () =
    let o = obj()
    let syncDispatch dispatch = lock o (fun () -> dispatch)
    let init _ = 0, Cmd.OfAsync.perform async.Return Increment id
    let update _ _ = failwith "Boom!"
    let view _ = ignore
    let mutable called = false
    let onError _ = called <- true
    Program.mkProgram init update view
    |> Program.withConsoleTrace
    |> Program.mapErrorHandler (fun _ -> onError)
    |> Program.runWithDispatch syncDispatch ()
    System.Threading.Thread.Sleep (1_000)
    called =! true


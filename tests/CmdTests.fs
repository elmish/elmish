module Elmish.CmdTests

open NUnit.Framework
open Swensen.Unquote
open Elmish

type Msg =
    | OnError of exn
    | OnSuccess of string


[<Test>]
let ``Cmd.OfAsync.either - works`` () =
    let myTask () =
        async {
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.either myTask () OnSuccess OnError
    let update msg _ =
        match msg with
        | OnError ex -> ex.Message, Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Task completed"

[<Test>]
let ``Cmd.OfAsync.attempt - works`` () =
    let mutable result = ""
    let myTask () =
        async {
            // Cmd.OfAsync.attempt don't notifiy about success
            // so we use a mutable variable to check if the task was called
            result <- "Task called"
            return "Task completed"
        }
    let init () = "initial", Cmd.OfAsync.attempt myTask () OnError
    let update msg _ =
        match msg with
        | OnError ex -> "Error was captured", Cmd.none
        | OnSuccess res -> res, Cmd.none
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Task called"

[<Test>]
let ``Cmd.OfAsync.perform - works`` () =
    let myTask () =
        async {
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.perform myTask () OnSuccess
    let update msg _ =
        match msg with
        | OnError ex -> ex.Message, Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Task completed"

[<Test>]
let ``Cmd.OfAsync.either - thrown exception when generating task should be captured`` () =
    let myTask () =
        failwith "Boom!"
        async {
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.either myTask () OnSuccess OnError
    let update msg _ =
        match msg with
        | OnError ex -> ex.Message, Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Boom!"

[<Test>]
let ``Cmd.OfAsync.attempt - thrown exception when generating task should be captured`` () =
    let myTask () =
        failwith "Boom!"
        async {
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.attempt myTask () OnError
    let update msg _ =
        match msg with
        | OnError ex -> "Error was captured", Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Error was captured"

[<Test>]
let ``Cmd.OfAsync.perform - thrown exception when generating task should be discarded`` () =
    let myTask () =
        failwith "Boom!"
        async {
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.perform myTask () OnSuccess
    let update msg _ =
        match msg with
        | OnError ex -> ex.Message, Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "initial"

[<Test>]
let ``Cmd.OfAsync.either - thrown exception from inside task should be captured`` () =
    let myTask () =
        async {
            failwith "Boom!"
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.either myTask () OnSuccess OnError
    let update msg _ =
        match msg with
        | OnError ex -> ex.Message, Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Boom!"

[<Test>]
let ``Cmd.OfAsync.attempt - thrown exception from inside task should be captured`` () =
    let myTask () =
        async {
            failwith "Boom!"
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.attempt myTask () OnError
    let update msg _ =
        match msg with
        | OnError ex -> "Error was captured", Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "Error was captured"

[<Test>]
let ``Cmd.OfAsync.perform - thrown exception from inside task should be discarded`` () =
    let myTask () =
        async {
            failwith "Boom!"
            return "Task completed"
        }

    let init () = "initial", Cmd.OfAsync.perform myTask () OnSuccess
    let update msg _ =
        match msg with
        | OnError ex -> ex.Message, Cmd.none
        | OnSuccess res -> res, Cmd.none
    let mutable result = ""
    let view model _ = result <- model
    async {
        Program.mkProgram init update view
        |> Program.run
    } |> Async.Start
    System.Threading.Thread.Sleep (1_000)
    result =! "initial"

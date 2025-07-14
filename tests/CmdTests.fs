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

#if !FABLE_COMPILER

[<Test>]
let ``Cmd.OfTask.either - works`` () =
    let myTask () =
        System.Threading.Tasks.Task.FromResult("Task completed")

    let init () = "initial", Cmd.OfTask.either myTask () OnSuccess OnError
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
let ``Cmd.OfTask.attempt - works`` () =
    let mutable result = ""
    let myTask () =
        // Cmd.OfTask.attempt don't notify about success
        // so we use a mutable variable to check if the task was called
        result <- "Task called"
        System.Threading.Tasks.Task.CompletedTask

    let init () = "initial", Cmd.OfTask.attempt myTask () OnError
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
let ``Cmd.OfTask.perform - works`` () =
    let myTask () =
        System.Threading.Tasks.Task.FromResult("Task completed")

    let init () = "initial", Cmd.OfTask.perform myTask () OnSuccess
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
let ``Cmd.OfTask.either - thrown exception when generating task should be captured`` () =
    let myTask () =
        failwith "Boom!"
        System.Threading.Tasks.Task.FromResult("Task completed")

    let init () = "initial", Cmd.OfTask.either myTask () OnSuccess OnError
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
let ``Cmd.OfTask.attempt - thrown exception when generating task should be captured`` () =
    let myTask () =
        failwith "Boom!"
        System.Threading.Tasks.Task.CompletedTask

    let init () = "initial", Cmd.OfTask.attempt myTask () OnError
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
let ``Cmd.OfTask.perform - thrown exception when generating task should be discarded`` () =
    let myTask () =
        failwith "Boom!"
        System.Threading.Tasks.Task.FromResult("Task completed")

    let init () = "initial", Cmd.OfTask.perform myTask () OnSuccess
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
let ``Cmd.OfTask.either - faulted task should be captured`` () =
    let myTask () =
        System.Threading.Tasks.Task.FromException<string>(System.Exception("Task failed"))

    let init () = "initial", Cmd.OfTask.either myTask () OnSuccess OnError
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
    result =! "Task failed"

[<Test>]
let ``Cmd.OfTask.attempt - faulted task should be captured`` () =
    let myTask () =
        System.Threading.Tasks.Task.FromException(System.Exception("Task failed"))

    let init () = "initial", Cmd.OfTask.attempt myTask () OnError
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
let ``Cmd.OfTask.perform - faulted task should be discarded`` () =
    let myTask () =
        System.Threading.Tasks.Task.FromException<string>(System.Exception("Task failed"))

    let init () = "initial", Cmd.OfTask.perform myTask () OnSuccess
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

#endif

#if !FABLE_COMPILER && !NETSTANDARD2_0
open System.Threading.Tasks
open System

[<Test>]
let ``Cmd.OfValueTask.either - works`` () =
    let myTask () = ValueTask<string>("ValueTask completed")
    let init () = "initial", Cmd.OfValueTask.either myTask () OnSuccess OnError
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
    result =! "ValueTask completed"

[<Test>]
let ``Cmd.OfValueTask.perform - works`` () =
    let myTask () = ValueTask<string>("ValueTask completed")
    let init () = "initial", Cmd.OfValueTask.perform myTask () OnSuccess
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
    result =! "ValueTask completed"

[<Test>]
let ``Cmd.OfValueTask.attempt - works`` () =
    let mutable result = ""
    let myTask () =
        result <- "ValueTask called"
        ValueTask.CompletedTask
    let init () = "initial", Cmd.OfValueTask.attempt myTask () OnError
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
    result =! "ValueTask called"

[<Test>]
let ``Cmd.OfValueTask.either - thrown exception when generating task should be captured`` () =
    let myTask () =
        failwith "Boom!"
        ValueTask<string>("ValueTask completed")
    let init () = "initial", Cmd.OfValueTask.either myTask () OnSuccess OnError
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
let ``Cmd.OfValueTask.attempt - thrown exception when generating task should be captured`` () =
    let myTask () =
        failwith "Boom!"
        ValueTask.CompletedTask
    let init () = "initial", Cmd.OfValueTask.attempt myTask () OnError
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
let ``Cmd.OfValueTask.perform - thrown exception when generating task should be discarded`` () =
    let myTask () =
        failwith "Boom!"
        ValueTask<string>("ValueTask completed")
    let init () = "initial", Cmd.OfValueTask.perform myTask () OnSuccess
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
let ``Cmd.OfValueTask.either - faulted ValueTask should be captured`` () =
    let myTask () = ValueTask<string>(Task.FromException<string>(Exception("ValueTask failed")))
    let init () = "initial", Cmd.OfValueTask.either myTask () OnSuccess OnError
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
    result =! "ValueTask failed"

[<Test>]
let ``Cmd.OfValueTask.attempt - faulted ValueTask should be captured`` () =
    let myTask () = ValueTask(Task.FromException(Exception("ValueTask failed")))
    let init () = "initial", Cmd.OfValueTask.attempt myTask () OnError
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
let ``Cmd.OfValueTask.perform - faulted ValueTask should be discarded`` () =
    let myTask () = ValueTask<string>(Task.FromException<string>(Exception("ValueTask failed")))
    let init () = "initial", Cmd.OfValueTask.perform myTask () OnSuccess
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
#endif

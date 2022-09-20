(**
Cmd
---------
Core abstractions for dispatching messages in Elmish.

*)

namespace Elmish

open System

/// Dispatch - feed new message into the processing loop
type Dispatch<'msg> = 'msg -> unit

/// Effect - return immediately, but may schedule dispatch of a message at any time
type Effect<'msg> = Dispatch<'msg> -> unit

/// Cmd - container for effects that may produce messages
type Cmd<'msg> = Effect<'msg> list

/// Cmd module for creating and manipulating commands
[<RequireQualifiedAccess>]
module Cmd =
    /// Execute the commands using the supplied dispatcher
    let internal exec onError (dispatch: Dispatch<'msg>) (cmd: Cmd<'msg>) =
        cmd |> List.iter (fun call -> try call dispatch with ex -> onError ex)

    /// None - no commands, also known as `[]`
    let none : Cmd<'msg> =
        []

    /// When emitting the message, map to another type
    let map (f: 'a -> 'msg) (cmd: Cmd<'a>) : Cmd<'msg> =
        cmd |> List.map (fun g -> (fun dispatch -> f >> dispatch) >> g)

    /// Aggregate multiple commands
    let batch (cmds: #seq<Cmd<'msg>>) : Cmd<'msg> =
        cmds |> List.concat

    /// Command to call the effect
    let ofEffect (effect: Effect<'msg>) : Cmd<'msg> =
        [effect]

    module OfFunc =
        /// Command to evaluate a simple function and map the result
        /// into success or error (of exception)
        let either (task: 'a -> _) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                try
                    task arg
                    |> (ofSuccess >> dispatch)
                with x ->
                    x |> (ofError >> dispatch)
            [bind]

        /// Command to evaluate a simple function and map the success to a message
        /// discarding any possible error
        let perform (task: 'a -> _) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                try
                    task arg
                    |> (ofSuccess >> dispatch)
                with x ->
                    ()
            [bind]

        /// Command to evaluate a simple function and map the error (in case of exception)
        let attempt (task: 'a -> unit) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                try
                    task arg
                with x ->
                    x |> (ofError >> dispatch)
            [bind]

    module OfAsyncWith =
        /// Command that will evaluate an async block and map the result
        /// into success or error (of exception)
        let either (start: Async<unit> -> unit)
                   (task: 'a -> Async<_>)
                   (arg: 'a)
                   (ofSuccess: _ -> 'msg)
                   (ofError: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task arg |> Async.Catch
                    dispatch (match r with
                             | Choice1Of2 x -> ofSuccess x
                             | Choice2Of2 x -> ofError x)
                }
            [bind >> start]

        /// Command that will evaluate an async block and map the success
        let perform (start: Async<unit> -> unit)
                    (task: 'a -> Async<_>)
                    (arg: 'a)
                    (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task arg |> Async.Catch
                    match r with
                    | Choice1Of2 x -> dispatch (ofSuccess x)
                    | _ -> ()
                }
            [bind >> start]

        /// Command that will evaluate an async block and map the error (of exception)
        let attempt (start: Async<unit> -> unit)
                    (task: 'a -> Async<_>)
                    (arg: 'a)
                    (ofError: _ -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task arg |> Async.Catch
                    match r with
                    | Choice2Of2 x -> dispatch (ofError x)
                    | _ -> ()
                }
            [bind >> start]

    module OfAsync =
#if FABLE_COMPILER
        let start x = Timer.delay 1 (fun _ -> Async.StartImmediate x)
#else
        let inline start x = Async.Start x
#endif
        /// Command that will evaluate an async block and map the result
        /// into success or error (of exception)
        let inline either (task: 'a -> Async<_>)
                          (arg: 'a)
                          (ofSuccess: _ -> 'msg)
                          (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.either start task arg ofSuccess ofError

        /// Command that will evaluate an async block and map the success
        let inline perform (task: 'a -> Async<_>)
                           (arg: 'a)
                           (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.perform start task arg ofSuccess

        /// Command that will evaluate an async block and map the error (of exception)
        let inline attempt (task: 'a -> Async<_>)
                           (arg: 'a)
                           (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.attempt start task arg ofError

    module OfAsyncImmediate =
        /// Command that will evaluate an async block and map the result
        /// into success or error (of exception)
        let inline either (task: 'a -> Async<_>)
                          (arg: 'a)
                          (ofSuccess: _ -> 'msg)
                          (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.either Async.StartImmediate task arg ofSuccess ofError

        /// Command that will evaluate an async block and map the success
        let inline perform (task: 'a -> Async<_>)
                           (arg: 'a)
                           (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.perform Async.StartImmediate task arg ofSuccess

        /// Command that will evaluate an async block and map the error (of exception)
        let inline attempt (task: 'a -> Async<_>)
                           (arg: 'a)
                           (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsyncWith.attempt Async.StartImmediate task arg ofError

#if FABLE_COMPILER
    module OfPromise =
        /// Command to call `promise` block and map the results
        let either (task: 'a -> Fable.Core.JS.Promise<_>)
                   (arg:'a)
                   (ofSuccess: _ -> 'msg)
                   (ofError: #exn -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                (task arg)
                    .``then``(ofSuccess >> dispatch)
                    .catch(unbox >> ofError >> dispatch)
                    |> ignore
            [bind]

        /// Command to call `promise` block and map the success
        let perform (task: 'a -> Fable.Core.JS.Promise<_>)
                   (arg:'a)
                   (ofSuccess: _ -> 'msg) =
            let bind dispatch =
                (task arg)
                    .``then``(ofSuccess >> dispatch)
                    |> ignore
            [bind]

        /// Command to call `promise` block and map the error
        let attempt (task: 'a -> Fable.Core.JS.Promise<_>)
                    (arg:'a)
                    (ofError: #exn -> 'msg) : Cmd<'msg> =
            let bind dispatch =
                (task arg)
                    .catch(unbox >> ofError >> dispatch)
                    |> ignore
            [bind]
#else
    open System.Threading.Tasks
    module OfTask =
        /// Command to call a task and map the results
        let inline either (task: 'a -> Task<_>)
                          (arg:'a)
                          (ofSuccess: _ -> 'msg)
                          (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsync.either (task >> Async.AwaitTask) arg ofSuccess ofError

        /// Command to call a task and map the success
        let inline perform (task: 'a -> Task<_>)
                           (arg:'a)
                           (ofSuccess: _ -> 'msg) : Cmd<'msg> =
            OfAsync.perform (task >> Async.AwaitTask) arg ofSuccess

        /// Command to call a task and map the error
        let inline attempt (task: 'a -> Task<_>)
                           (arg:'a)
                           (ofError: _ -> 'msg) : Cmd<'msg> =
            OfAsync.attempt (task >> Async.AwaitTask) arg ofError
#endif

    /// Command to issue a specific message
    let inline ofMsg (msg:'msg) : Cmd<'msg> =
        [fun dispatch -> dispatch msg]


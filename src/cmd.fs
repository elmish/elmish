(**
Cmd
---------
Core abstractions for dispatching messages in Elmish.

*)

namespace Elmish

open System

/// Dispatch - feed new message into the processing loop
type Dispatch<'msg> = 'msg -> unit

/// Subscription - return immediately, but may schedule dispatch of a message at any time
type Sub<'msg> = Dispatch<'msg> -> unit

/// Cmd - container for subscriptions that may produce messages
type Cmd<'msg> = Sub<'msg> list

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

    /// Command to call the subscriber
    let ofSub (sub: Sub<'msg>) : Cmd<'msg> =
        [sub]

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

        /// Command to issue a specific message
        let result (msg:'msg) : Cmd<'msg> =
            [fun dispatch -> dispatch msg]

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

        /// Command that will evaluate an async block to the message
        let result (start: Async<unit> -> unit) 
                   (task: Async<'msg>) : Cmd<'msg> =
            let bind dispatch =
                async {
                    let! r = task
                    dispatch r
                }
            [bind >> start]

    module OfAsync =
#if FABLE_COMPILER
        let start x = Timer.delay 0 (fun _ -> Async.StartImmediate x)
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

        /// Command that will evaluate an async block to the message
        let inline result (task: Async<'msg>) : Cmd<'msg> =
            OfAsyncWith.result start task

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

        /// Command that will evaluate an async block to the message
        let inline result (task: Async<'msg>) : Cmd<'msg> =
            OfAsyncWith.result Async.StartImmediate task

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

        /// Command to dispatch the `promise` result
        let result (task: Fable.Core.JS.Promise<'msg>) =
            let bind dispatch =
                task.``then`` dispatch
                |> ignore
            [bind]

    [<Obsolete("Use `OfPromise.either` instead")>]
    let inline ofPromise (task: 'a -> Fable.Core.JS.Promise<_>)
                         (arg:'a)
                         (ofSuccess: _ -> 'msg)
                         (ofError: _ -> 'msg) : Cmd<'msg> =
        OfPromise.either task arg ofSuccess ofError                     
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

        /// Command and map the task success
        let inline result (task: Task<'msg>) : Cmd<'msg> =
            OfAsync.result (task |> Async.AwaitTask)

    [<Obsolete("Use OfTask.either instead")>]
    let inline ofTask (task: 'a -> Task<_>)
                      (arg:'a)
                      (ofSuccess: _ -> 'msg)
                      (ofError: _ -> 'msg) : Cmd<'msg> =
        OfTask.either task arg ofSuccess ofError
#endif

    // Synonymous with `OfFunc.result`, may be removed in the future
    let inline ofMsg (msg:'msg) : Cmd<'msg> =
        OfFunc.result msg

    [<Obsolete("Use `Cmd.OfAsync.either` instead")>]
    let inline ofAsync (task: 'a -> Async<_>)
                       (arg: 'a)
                       (ofSuccess: _ -> 'msg)
                       (ofError: _ -> 'msg) : Cmd<'msg> =
        OfAsync.either task arg ofSuccess ofError

    [<Obsolete("Use `Cmd.OfFunc.either` instead")>]
    let inline ofFunc (task: 'a -> _) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
        OfFunc.either task arg ofSuccess ofError

    [<Obsolete("Use `Cmd.OfFunc.perform` instead")>]
    let inline performFunc (task: 'a -> _) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
        OfFunc.perform task arg ofSuccess

    [<Obsolete("Use `Cmd.OfFunc.attempt` instead")>]
    let attemptFunc (task: 'a -> unit) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
        OfFunc.attempt task arg ofError


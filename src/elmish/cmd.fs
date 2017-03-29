namespace Elmish

open System

/// Dispatch - feed new message into the processing loop
type Dispatch<'msg> = 'msg -> unit
/// Subscriber - return immediately, but may schedule dispatch of a message at any time
type Sub<'msg> = Dispatch<'msg> -> unit
/// Cmd - container for subscriptions that may produce messages
type Cmd<'msg> = Sub<'msg> list

/// Cmd module for creating and manipulating commands
[<RequireQualifiedAccess>]
module Cmd =
    /// None - no commands, also known as `[]`
    let none : Cmd<'msg> =
        []

    /// Command to issue a specific message
    let ofMsg (msg:'msg) : Cmd<'msg> =
        [fun dispatch -> dispatch msg]

    /// When emitting the message, map to another type
    let map (f: 'a -> 'msg) (cmd: Cmd<'a>) : Cmd<'msg> =
        cmd |> List.map (fun g -> (fun dispatch -> f >> dispatch) >> g)

    /// Aggregate multiple commands
    let batch (cmds: Cmd<'msg> list) : Cmd<'msg> =
        List.collect id cmds

    /// Map a cmd and batch it with the rest of cmds
    let mapCons (cmds:Cmd<'msg> list) (f:'a -> 'msg) (cmd:Cmd<'a>) : Cmd<'msg> =
        (map f cmd)::cmds |> batch

    /// Command that will evaluate an async block and map the result
    /// into success or error (of exception)
    let ofAsync (task: 'a -> Async<_>) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
        let bind dispatch =
            async {
                let! r = task arg |> Async.Catch
                dispatch (match r with
                         | Choice1Of2 x -> ofSuccess x
                         | Choice2Of2 x -> ofError x)
            }
        [bind >> Async.StartImmediate]

    /// Command to evaluate a simple function and map the result
    /// into success or error (of exception)
    let ofFunc (task: 'a -> _) (arg: 'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
        let bind dispatch =
            try
                task arg
                |> (ofSuccess >> dispatch)
            with x ->
                x |> (ofError >> dispatch)
        [bind]

    /// Command to evaluate a simple function and map the success to a message
    /// discarding any possible error
    let performFunc (task: 'a -> _) (arg: 'a) (ofSuccess: _ -> 'msg) : Cmd<'msg> =
        let bind dispatch =
            try
                task arg
                |> (ofSuccess >> dispatch)
            with x ->
                ()
        [bind]

    /// Command to evaluate a simple function and map the error (in case of exception)
    let attemptFunc (task: 'a -> unit) (arg: 'a) (ofError: _ -> 'msg) : Cmd<'msg> =
        let bind dispatch =
            try
                task arg
            with x ->
                x |> (ofError >> dispatch)
        [bind]

    /// Command to call the subscriber
    let ofSub (sub: Sub<'msg>) : Cmd<'msg> =
        [sub]

    open Fable.PowerPack

    /// Command to call `promise` block and map the results
    let ofPromise (task: 'a -> Fable.Import.JS.Promise<_>) (arg:'a) (ofSuccess: _ -> 'msg) (ofError: _ -> 'msg) : Cmd<'msg> =
        let bind dispatch =
            task arg
            |> Promise.map (ofSuccess >> dispatch)
            |> Promise.catch (ofError >> dispatch)
            |> ignore
        [bind]

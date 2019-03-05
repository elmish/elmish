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
    let internal exec (dispatch:Dispatch<'msg>) (cmd:Cmd<'msg>)=
        cmd |> List.iter (fun sub -> sub dispatch)

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
    let batch (cmds: #seq<Cmd<'msg>>) : Cmd<'msg> =
        cmds |> List.concat

    /// Command that will evaluate an async block and map the result
    /// into success or error (of exception)
    let ofAsync (task: 'a -> Async<_>) 
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
        [bind >> Async.StartImmediate]

    type AsyncValParams<'a, 'b> =
        { Value : Async<'a>
          Success : 'a -> 'b
          Error : Option<exn -> 'b> }

    /// Converts an `Async<'t>` expression to `Cmd<'u>`, mapping the `'t` to `'u` using the Success function or optionally map an exception to `'u` using the Error function if unexpected errors occur. 
    /// 
    /// For example:
    /// 
    /// 
    /// ```
    /// let getDataCmd = 
    ///     Cmd.fromAsync {
    ///         Value = Server.api.getData input
    ///         Success = fun data -> GotData data
    ///         Error = Some (fun ex -> FailedToGetData ex.Message)
    ///     }
    /// ```
    /// 
    /// In case you assume that an `Async` expression does not throw errors, you can set the error handler to None:
    /// 
    /// ```
    /// let getAnswerCmd = 
    ///     Cmd.fromAsync {
    ///         Value = async { return 42 }
    ///         Success = fun answer -> GotAnswer answer
    ///         Error = None
    ///     }
    /// ```
    ///   
    let fromAsync (asyncValue: AsyncValParams<'a, 'b>) : Cmd<'b> = 
        let bind dispatch = 
            async {
                let! result = Async.Catch asyncValue.Value
                match result with 
                | Choice1Of2 value -> dispatch (asyncValue.Success value) 
                | Choice2Of2 error -> 
                    match asyncValue.Error with 
                    | Some errorHandler -> dispatch (errorHandler error)
                    | None -> ()
            } 
        
        [ bind >> Async.StartImmediate ]

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

#if FABLE_COMPILER
    /// Command to call `promise` block and map the results
    let ofPromise (task: 'a -> Fable.Import.JS.Promise<_>) 
                  (arg:'a) 
                  (ofSuccess: _ -> 'msg) 
                  (ofError: _ -> 'msg) : Cmd<'msg> =
        let bind dispatch =
            task arg
            |> Promise.map (ofSuccess >> dispatch)
            |> Promise.catch (ofError >> dispatch)
            |> ignore
        [bind]
#else
    open System.Threading.Tasks

    /// Command to call a task and map the results
    let inline ofTask (task: 'a -> Task<_>) 
                      (arg:'a) 
                      (ofSuccess: _ -> 'msg) 
                      (ofError: _ -> 'msg) : Cmd<'msg> =
        ofAsync (task >> Async.AwaitTask) arg ofSuccess ofError

#endif
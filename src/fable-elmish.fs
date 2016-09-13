module Fable.Elmish

open System

/// Dispatch function - feed new message into the provessing loop 
type Dispatch<'msg> = 'msg -> unit 
/// Message subscriber function 
type Sub<'msg> = 'msg Dispatch -> unit
/// Cmd modu- container for actions that when evaluated 
type Cmd<'msg> = list<'msg Sub>

/// Cmd module creating and manipulating actions 
/// may produce one or more messages message(s)
module Cmd =
    /// None - no commands, also known as `[]`
    let none : Cmd<'msg> =
        []

    /// Command to issue a specific message 
    let ofMsg (msg:'msg) = 
        [fun (dispatch:'msg Dispatch) -> dispatch msg] 
    
    /// When emitting the message, map to another type 
    let map (f:'a -> 'msg) (cmd:Cmd<'a>) : Cmd<'msg> = 
        cmd |> List.map (fun g -> (fun post -> f >> post) >> g)

    /// Aggregate multiple commands
    let batch (cmds:list<'msg Cmd>) : Cmd<'msg> = 
        List.collect id cmds 

    /// Command that will evaluate async block and map the result 
    /// into success or error (of exception) 
    let ofAsync (task:Async<_>) (ofSuccess:_->'msg) (ofError:_->'msg) : Cmd<'msg> =
        let bind dispatch =
            async {
                let! r = task |> Async.Catch
                dispatch (match r with
                         | Choice1Of2 x -> ofSuccess x
                         | Choice2Of2 x -> ofError x)
            }
        [bind >> Async.StartImmediate]

    /// Command to evaluate a simple function and map the result 
    /// into success or error (of exception) 
    let ofFunc (task:unit->_) (ofSuccess:_->'msg) (ofError:_->'msg) : Cmd<'msg> =
        let bind (dispatch:'msg -> unit) =
            try 
                task()
                |> (ofSuccess >> dispatch)
            with x -> 
                x |> (ofError >> dispatch)
        [bind]

    /// Command to call the subscriber 
    let ofSub (sub:Sub<'msg>) =
        [sub]

type Program<'model,'msg> = {
    init : unit -> 'model * Cmd<'msg>
    update : 'msg -> 'model -> 'model * Cmd<'msg>
    subscribe : 'model -> Cmd<'msg>
    trace : string -> ('model*'msg) -> unit 
}

/// Program module - functions to manipulate program instances 
module Program =
    /// Typical program, produces new commands as part of init() and update() as well as the new model
    let mkProgram (init:unit -> 'model * Cmd<'msg>) (update:'msg -> 'model -> 'model * Cmd<'msg>) =
        { init = init
          update = update
          subscribe = fun _ -> Cmd.none
          trace = fun _ -> ignore }
    
    /// Simple program that only produces new model in init() and update().
    /// Good for tutorials
    let mkSimple (init:unit -> 'model) (update:'msg -> 'model -> 'model) =
        { init = init >> fun state -> state,Cmd.none
          update = fun msg -> update msg >> fun state -> state,Cmd.none
          subscribe = fun _ -> Cmd.none
          trace = fun _ -> ignore }

    /// Subscribe to external source of events
    let withSubscription (subscribe : 'model -> Cmd<'msg>) (program:Program<'model,'msg>) = 
        { program with subscribe = subscribe }

    /// Trace all the updates to the console
    let withConsoleTrace (program:Program<'model,'msg>) = 
        let trace text (model:'model,msg:'msg) = 
            Fable.Import.Browser.console.log (text, model, msg)
        { program with trace = trace} 

    /// Trace all the messages as they update the model
    let withTrace (program:Program<'model,'msg>) trace = 
        { program with trace = trace} 

    /// Start the dispatch loop
    let run setState (program:Program<'model,'msg>) : 'msg Dispatch=
        let (model,cmd) = program.init()
        let inbox = MailboxProcessor.Start(fun (mb:MailboxProcessor<'msg>) ->
            let rec loop state = 
                async {
                    try 
                        setState state 
                    with ex -> 
                        Fable.Import.Browser.console.error ("unable to setState:", state, ex)
                    let! msg = mb.Receive()
                    program.trace "Updating: " (state,msg)
                    try 
                        let (model',cmd') = program.update msg state
                        cmd' |> List.iter (fun sub -> sub mb.Post)
                        return! loop model'
                    with ex -> 
                        Fable.Import.Browser.console.error ("unable to update:", ex)
                        return! loop state
                }
            loop model
        )
        program.subscribe model 
        @ cmd |> List.iter (fun sub -> sub inbox.Post)
        inbox.Post

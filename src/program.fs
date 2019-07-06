(**
Program
---------
Core abstractions for creating and running the dispatch loop.

*)

namespace Elmish


/// Program type captures various aspects of program behavior
type Program<'arg, 'model, 'msg, 'view> = private {
    init : 'arg -> 'model * Cmd<'msg>
    update : 'msg -> 'model -> 'model * Cmd<'msg>
    subscribe : 'model -> Cmd<'msg>
    view : 'model -> Dispatch<'msg> -> 'view
    setState : 'model -> Dispatch<'msg> -> unit
    onError : (string*exn) -> unit
    termination : ('msg -> bool) * ('model -> unit)
}

/// Program module - functions to manipulate program instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Program =
    /// Typical program, new commands are produced by `init` and `update` along with the new state.
    let mkProgram 
        (init : 'arg -> 'model * Cmd<'msg>) 
        (update : 'msg -> 'model -> 'model * Cmd<'msg>)
        (view : 'model -> Dispatch<'msg> -> 'view) =
        { init = init
          update = update
          view = view
          setState = fun model -> view model >> ignore
          subscribe = fun _ -> Cmd.none
          onError = Log.onError
          termination = (fun _ -> false), ignore }

    /// Simple program that produces only new state with `init` and `update`.
    let mkSimple 
        (init : 'arg -> 'model) 
        (update : 'msg -> 'model -> 'model)
        (view : 'model -> Dispatch<'msg> -> 'view) =
        { init = init >> fun state -> state,Cmd.none
          update = fun msg -> update msg >> fun state -> state,Cmd.none
          view = view
          setState = fun model -> view model >> ignore
          subscribe = fun _ -> Cmd.none
          onError = Log.onError
          termination = (fun _ -> false), ignore }

    /// Subscribe to external source of events.
    /// The subscription is called once - with the initial model, but can dispatch new messages at any time.
    let withSubscription (subscribe : 'model -> Cmd<'msg>) (program: Program<'arg, 'model, 'msg, 'view>) =
        let sub model =
            Cmd.batch [ program.subscribe model
                        subscribe model ]
        { program with subscribe = sub }

    /// Trace all the updates to the console
    let withConsoleTrace (program: Program<'arg, 'model, 'msg, 'view>) =
        let traceInit (arg:'arg) =
            let initModel,cmd = program.init arg
            Log.toConsole ("Initial state:", initModel)
            initModel,cmd

        let traceUpdate msg model =
            Log.toConsole ("New message:", msg)
            let newModel,cmd = program.update msg model
            Log.toConsole ("Updated state:", newModel)
            newModel,cmd

        { program with
            init = traceInit 
            update = traceUpdate }

    /// Trace all the messages as they update the model
    let withTrace trace (program: Program<'arg, 'model, 'msg, 'view>) =
        let update msg model =
            let state,cmd = program.update msg model
            trace msg state
            state,cmd
        { program
            with update = update }

    /// Handle dispatch loop exceptions
    let withErrorHandler onError (program: Program<'arg, 'model, 'msg, 'view>) =
        { program
            with onError = onError }

    /// Exit criteria and the handler, overrides existing. 
    let withTermination (predicate: 'msg -> bool) (terminate: 'model -> unit) (program: Program<'arg, 'model, 'msg, 'view>) =
        { program
            with termination = predicate, terminate }

    /// Map existing error handler and return new `Program` 
    let mapErrorHandler map (program: Program<'arg, 'model, 'msg, 'view>) =
        { program
            with onError = map program.onError }

    /// Get the current error handler 
    let onError (program: Program<'arg, 'model, 'msg, 'view>) =
        program.onError

    /// Function to render the view with the latest state 
    let withSetState (setState:'model -> Dispatch<'msg> -> unit)
                     (program: Program<'arg, 'model, 'msg, 'view>) =        
        { program
            with setState = setState }

    /// Return the function to render the state 
    let setState (program: Program<'arg, 'model, 'msg, 'view>) =        
        program.setState

    /// Return the view function 
    let view (program: Program<'arg, 'model, 'msg, 'view>) =        
        program.view

    /// Map the program type
    let map mapInit mapUpdate mapView mapSetState mapSubscribe mapTermination
            (program: Program<'arg, 'model, 'msg, 'view>) =
        { init = mapInit program.init
          update = mapUpdate program.update
          view = mapView program.view
          setState = mapSetState program.setState
          subscribe = mapSubscribe program.subscribe
          onError = program.onError
          termination = mapTermination program.termination }

    /// Start the program loop.
    /// syncDispatch: specify how to serialize dispatch calls.
    /// arg: argument to pass to the init() function.
    /// program: program created with 'mkSimple' or 'mkProgram'.
    let runWith (syncDispatch: Dispatch<'msg> -> Dispatch<'msg>) (arg: 'arg) (program: Program<'arg, 'model, 'msg, 'view>) =
        let (model,cmd) = program.init arg
        let toTerminate, terminate = program.termination
        let rb = RingBuffer 10
        let mutable reentered = false
        let mutable state = model
        let mutable terminated = false
        let rec dispatch msg = 
            if terminated then ()
            else 
                if reentered then
                    rb.Push msg
                else
                    reentered <- true
                    let mutable nextMsg = Some msg
                    while Option.isSome nextMsg do
                        let msg = nextMsg.Value
                        if toTerminate msg then
                            terminate state
                            terminated <- true
                        else                        
                            try
                                let (model',cmd') = program.update msg state
                                program.setState model' dispatch'
                                cmd' |> Cmd.exec dispatch'
                                state <- model'
                            with ex ->
                                program.onError (sprintf "Unable to process the message: %A" msg, ex)
                            nextMsg <- rb.Pop()
                    reentered <- false
        and dispatch' = syncDispatch dispatch // serialized dispatch            

        program.setState model dispatch'
        let sub = 
            try 
                program.subscribe model 
            with ex ->
                program.onError ("Unable to subscribe:", ex)
                Cmd.none
        sub @ cmd |> Cmd.exec dispatch'

    /// Start the dispatch loop with `unit` for the init() function.
    let run (program: Program<unit, 'model, 'msg, 'view>) = runWith id () program

(**
Program
---------
Core abstractions for creating and running the dispatch loop.

*)

namespace Elmish

open System

/// Program type captures various aspects of program behavior
type Program<'arg, 'model, 'msg, 'view> = {
    init : 'arg -> 'model * Cmd<'msg>
    update : 'msg -> 'model -> 'model * Cmd<'msg>
    subscribe : 'model -> Cmd<'msg>
    view : 'model -> Dispatch<'msg> -> 'view
    setState : 'model -> Dispatch<'msg> -> unit
    onError : (string*exn) -> unit
}

/// Program module - functions to manipulate program instances
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Program =
    open Fable.Core.JsInterop
    open Fable.Import.JS

    let internal onError (text: string, ex: exn) = console.error (text,ex)

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
          onError = onError }

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
          onError = onError }

    /// Subscribe to external source of events.
    /// The subscription is called once - with the initial model, but can dispatch new messages at any time.
    let withSubscription (subscribe : 'model -> Cmd<'msg>) (program: Program<'arg, 'model, 'msg, 'view>) =
        let sub model =
            Cmd.batch [ program.subscribe model
                        subscribe model ]
        { program with subscribe = sub }

    /// Trace all the updates to the console
    let withConsoleTrace (program: Program<'arg, 'model, 'msg, 'view>) =
        let inline toPlain o = toJson o |> JSON.parse
        let traceInit (arg:'arg) =
            let initModel,cmd = program.init arg
            console.log ("Initial state:", toPlain initModel)
            initModel,cmd

        let traceUpdate msg model =
            console.log ("New message:", toPlain msg)
            let newModel,cmd = program.update msg model
            console.log ("Updated state:", toPlain newModel)
            newModel,cmd

        { program with
            init = traceInit 
            update = traceUpdate }

    /// Trace all the messages as they update the model
    let withTrace trace (program: Program<'arg, 'model, 'msg, 'view>) =
        { program
            with update = fun msg model -> trace msg model; program.update msg model}

    /// Handle dispatch loop exceptions
    let withErrorHandler onError (program: Program<'arg, 'model, 'msg, 'view>) =
        { program
            with onError = onError }

    /// Start the program loop.
    /// arg: argument to pass to the init() function.
    /// program: program created with 'mkSimple' or 'mkProgram'.
    let runWith (arg: 'arg) (program: Program<'arg, 'model, 'msg, 'view>) =
        let (model,cmd) = program.init arg
        let inbox = MailboxProcessor.Start(fun (mb:MailboxProcessor<'msg>) ->
            let rec loop (state:'model) =
                async {
                    let! msg = mb.Receive()
                    let newState =
                        try
                            let (model',cmd') = program.update msg state
                            program.setState model' mb.Post
                            cmd' |> List.iter (fun sub -> sub mb.Post)
                            model'
                        with ex ->
                            program.onError ("Unable to process a message:", ex)
                            state
                    return! loop newState
                }
            loop model
        )
        program.setState model inbox.Post
        program.subscribe model
        @ cmd |> List.iter (fun sub -> sub inbox.Post)

    /// Start the dispatch loop with `unit` for the init() function.
    let run (program: Program<unit, 'model, 'msg, 'view>) = runWith () program


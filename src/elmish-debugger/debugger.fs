namespace Elmish.Debug
open Fable.Import.RemoteDev
open Fable.Core.JsInterop
open Fable.Core

module Debugger =
    type ConnectionOptions =
        | ViaExtension
        | Remote of address:string * port:int
        | Secure of address:string * port:int

    let connect =
        let inline getCase cmd : obj = createObj ["type" ==> unbox cmd?Case
                                                  "fields" ==> cmd?Fields]

        function
        | ViaExtension -> { Options.remote = false; hostname = "localhost"; port = 8000; secure = false; getActionType = Some getCase }
        | Remote (address,port) -> { Options.remote = true; hostname = address; port = port; secure = false; getActionType = None }
        | Secure (address,port) -> { Options.remote = true; hostname = address; port = port; secure = true; getActionType = None }
        >> connectViaExtension

module Program =
    open Elmish

    [<PassGenericsAttribute>]
    let withDebuggerUsing (connection:Connection) (program : Program<'a,'model,'msg,'view>) : Program<'a,'model,'msg,'view> =
        let init a =
            let (model,cmd) = program.init a
            connection.init (model, None)
            model,cmd

        let update msg model : 'model * Cmd<'msg> =
            let (model',cmd) = program.update msg model
            connection.send (msg, model')
            (model',cmd)

        let subscribe model = 
            let sub dispatch =
                function
                | (msg:Msg) when msg.``type`` = MsgTypes.Dispatch ->
                    try
                        match msg.payload.``type`` with
                        | PayloadTypes.JumpToAction
                        | PayloadTypes.JumpToState ->
                            let state = inflate<'model> (extractState msg)
                            program.setState state dispatch
                        | PayloadTypes.ImportState ->
                            let state = msg.payload.nextLiftedState.computedStates |> Array.last
                            program.setState (inflate<'model> state?state) dispatch
                            connection.send(null, msg.payload.nextLiftedState)
                        | _ -> ()
                    with ex ->
                        Fable.Import.Browser.console.error ("Unable to process monitor command", msg, ex)
                | _ -> ()
                |> connection.subscribe
                |> ignore

            Cmd.batch
                [ [sub]
                  program.subscribe model ]

        let onError (text,ex) =
            connection.error (text,ex)

        { program with 
                    init = init
                    update = update
                    subscribe = subscribe
                    onError = onError }


    [<PassGenericsAttribute>]
    let withDebuggerAt options = 
        Debugger.connect options
        |> withDebuggerUsing

    
    [<PassGenericsAttribute>]
    let withDebugger (program : Program<'a,'model,'msg,'view>) : Program<'a,'model,'msg,'view> =
        ((Debugger.connect Debugger.ViaExtension),program)
        ||> withDebuggerUsing

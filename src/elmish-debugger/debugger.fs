namespace Elmish.Debug
open Fable.Import.RemoteDev
open Fable.Core.JsInterop
open Fable.Core

[<RequireQualifiedAccess>]
module Debugger =
    open FSharp.Reflection

    let inline private duName (x:'a) = 
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name

    let inline private getCase cmd : obj =
        createObj ["type" ==> duName cmd
                   "msg" ==> cmd]

    type ConnectionOptions =
        | ViaExtension
        | Remote of address:string * port:int
        | Secure of address:string * port:int

    let connect =
        let serialize = createObj ["replacer" ==> fun _ v -> deflate v]

        let fallback = { Options.remote = true; hostname = "remotedev.io"; port = 443; secure = true; getActionType = Some getCase; serialize = serialize }

        function
        | ViaExtension -> { fallback with remote = false; hostname = "localhost"; port = 8000; secure = false }
        | Remote (address,port) -> { fallback with hostname = address; port = port; secure = false; getActionType = None }
        | Secure (address,port) -> { fallback with hostname = address; port = port; getActionType = None }
        >> connectViaExtension

[<RequireQualifiedAccess>]
module Program =
    open Elmish
    open Fable.Import

    [<PassGenericsAttribute>]
    let withDebuggerUsing (connection:Connection) (program : Program<'a,'model,'msg,'view>) : Program<'a,'model,'msg,'view> =
        let init a =
            let (model,cmd) = program.init a
            // simple looking one liner to do a recursive deflate
            // needed otherwise extension gets F# obj
            let deflated = model |> toJson |> JS.JSON.parse
            connection.init (deflated, None)
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
    let withDebuggerAt options program : Program<'a,'model,'msg,'view> = 
        try
            (Debugger.connect options, program)
            ||> withDebuggerUsing
        with ex -> 
            Fable.Import.Browser.console.error ("Unable to connect to the monitor, continuing w/o debugger", ex)
            program

    
    [<PassGenericsAttribute>]
    let withDebugger (program : Program<'a,'model,'msg,'view>) : Program<'a,'model,'msg,'view> =
        try
            ((Debugger.connect Debugger.ViaExtension),program)
            ||> withDebuggerUsing
        with ex -> 
            Fable.Import.Browser.console.error ("Unable to connect to the monitor, continuing w/o debugger", ex)
            program

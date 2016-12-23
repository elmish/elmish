namespace Elmish.Debug
open Fable.Import.RemoteDev

module Debugger =

    type ConnectionOptions =
        | ViaExtension
        | Remote of address:string * port:int
        | Secure of address:string * port:int

    let connect =
        function
        | ViaExtension -> None
        | Remote (address,port) -> Some { Options.remote = true; hostname = address; port = port; secure = false }
        | Secure (address,port) -> Some { Options.remote = true; hostname = address; port = port; secure = true }
        >> connectViaExtension

module Program =
    open Elmish

    let withDebuggerUsing (connection:Connection) (program : Program<'a,'model,'msg,'view>) : Program<'a,'model,'msg,'view> =
        let init a =
            let (model,cmd) = program.init a
            connection.init (model, None)
            model,cmd

        let update msg model : 'model * Cmd<'msg> =
            connection.send (msg,model)
            program.update msg model

        let subscribe model = 
            let sub dispatch =
                connection.subscribe (fun msg -> let state = extractState msg 
                                                 program.setState state dispatch)
                |> ignore

            Cmd.batch
                [ [sub]
                  program.subscribe model ]

        let onError ex =
            connection.error ex

        { program with 
                    init = init
                    update = update
                    subscribe = subscribe
                    onError = onError }


    let withDebuggerAt options = 
        Debugger.connect options
        |> withDebuggerUsing

    
    let withDebugger (program : Program<'a,'model,'msg,'view>) : Program<'a,'model,'msg,'view> =
        ((Debugger.connect Debugger.ViaExtension),program)
        ||> withDebuggerUsing

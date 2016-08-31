module Fable.Elmish

open System

type Dispatch<'msg> = 'msg -> unit 
type Sub<'msg> = 'msg Dispatch -> unit
type Cmd<'msg> = list<'msg Sub>

module Cmd =
    open Fable.Import.JS

    let none : Cmd<'msg> =
        []

    let ofMsg (msg:'msg) = 
        [fun (dispatch:'msg Dispatch) -> dispatch msg] 
    
    let map (f:'a -> 'msg) (cmd:Cmd<'a>) : Cmd<'msg> = 
        cmd |> List.map (fun g -> (fun post -> f >> post) >> g)

    let batch (cmds:list<'msg Cmd>) : Cmd<'msg> = 
        List.collect id cmds 

    let ofAsync (task:Async<_>) (ofSuccess:_->'msg) (ofError:_->'msg) : Cmd<'msg> =
        let run dispatch =
            async {
                let! r = task |> Async.Catch
                dispatch (match r with
                         | Choice1Of2 x -> ofSuccess x
                         | Choice2Of2 x -> ofError x)
            }
        [run >> Async.Start]

    let ofPromise (task:unit->Promise<_>) (ofSuccess:_->'msg) (ofError:_->'msg) : Cmd<'msg> =
        let bind (dispatch:'msg -> unit) =
            task()
            |> Fable.Extras.Promise.onSuccess (ofSuccess >> dispatch)
            |> Fable.Extras.Promise.onFail (ofError >> dispatch)
            |> ignore
        [bind]

    let ofSub (sub:Sub<'msg>) =
        [sub]

type Program<'model,'msg> = {
    init : unit -> 'model * Cmd<'msg>
    update : 'msg -> 'model -> 'model * Cmd<'msg>
    subscribe : 'model -> Cmd<'msg>
    trace : string -> ('model*'msg) -> unit 
}

module Program =
    let mkProgram (init:unit -> 'model * Cmd<'msg>) (update:'msg -> 'model -> 'model * Cmd<'msg>) =
        { init = init
          update = update
          subscribe = fun _ -> Cmd.none
          trace = fun _ -> ignore }

    let mkSimple (init:unit -> 'model) (update:'msg -> 'model -> 'model) =
        { init = init >> fun state -> state,Cmd.none
          update = fun msg -> update msg >> fun state -> state,Cmd.none
          subscribe = fun _ -> Cmd.none
          trace = fun _ -> ignore }

    let withSubscription (subscribe : 'model -> Cmd<'msg>) (program:Program<'model,'msg>) = 
        { program with subscribe = subscribe }

    let withTrace (program:Program<'model,'msg>) = 
        let trace text (model:'model,msg:'msg) = 
            Fable.Import.Browser.console.log (text, model, msg)
        { program with trace = trace} 

    let run setState (program:Program<'model,'msg>) : 'msg Dispatch=
        let (model,cmd) = program.init()
        let inbox = MailboxProcessor.Start(fun (mb:MailboxProcessor<'msg>) ->
            let rec loop state = 
                async {
                    let! msg = mb.Receive()
                    program.trace "Updating: " (state,msg)
                    let (model',cmd') = program.update msg state
                    setState model' 
                    cmd' |> List.iter (fun sub -> sub mb.Post)
                    return! loop model'
                }
            loop model
        )
        program.subscribe model 
        @ cmd |> List.iter (fun sub -> sub inbox.Post)
        inbox.Post

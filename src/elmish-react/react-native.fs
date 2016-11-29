namespace Elmish.ReactNative

open System
open Fable.Import.React
open Fable.Core
open Elmish

module Components =

    type SetState<'model,'msg> = ('model->'msg Dispatch->unit)->unit

    type [<Pojo>] AppProps<'model,'msg> = {
        setState : SetState<'model,'msg>
        view : 'model->'msg Dispatch->ReactElement
    }

    type [<Pojo>] AppState<'model,'msg> = {
        dispatch : 'msg Dispatch
        model : 'model
    }

    type App<'model,'msg>(props:AppProps<'model,'msg>) as this =
        inherit Component<AppProps<'model,'msg>, AppState<'model,'msg>>(props)
        do
            props.setState (fun m d -> this.setInitState { dispatch = d; model = m })

        member this.componentDidMount() =
            props.setState (fun m d -> this.setState { dispatch = d; model = m })
        
        member this.render () =
            props.view this.state.model this.state.dispatch

[<RequireQualifiedAccess>]
module Program =
    open Fable.Core.JsInterop
    open Elmish.React
    open Components

    type Globals =
        [<Import("default","renderApplication")>] 
        static member renderApplication(rootComponent:ComponentClass<'P>, initialProps:'P, rootTag:obj) : obj = failwith "JS only"

    /// Setup rendering of root ReactNative component
    let toRunnable run (program:Program<_,_,_,_>) =
        let mutable lastState = Option.None
        let mutable setState = fun model dispatch -> lastState <- Some (model,dispatch) 
        let props = { view = program.view
                      setState = fun f -> match lastState with
                                          | Some (model,dispatch) -> f model dispatch
                                          | _ -> ()
                                          setState <- f }
        run { program with setState = setState }
        fun (appParameters:obj) -> Globals.renderApplication(unbox typeof<Components.App<_,_>>, props, appParameters?rootTag)

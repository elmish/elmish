namespace Elmish.React

open System
open Fable.Import.React
open Fable.Helpers.React
open Fable.Core
open Elmish

type MkView<'model> = ('model->unit) -> ('model->ReactElement<obj>)

type AppProps<'model> = {
    main:MkView<'model>
}

type LazyProps<'model> = {
    equal:'model->'model->bool
    view:'model->ReactElement<obj>
    mutable state:'model option
    nextState:'model
}

module Components =
    let mutable internal mounted = false

    type App<'model>(props:AppProps<'model>) as this =
        inherit Component<obj,'model>()
        do
            mounted <- false
        
        let safeState state =
            match mounted with 
            | false -> this.state <- state
            | _ -> this.setState state

        let view = props.main safeState

        member this.componentDidMount() =
            mounted <- true

        member this.render () =
            view this.state 

    
    type LazyView<'model>(props:LazyProps<'model>) =
        inherit Component<LazyProps<'model>,'model>(props)

        member this.shouldComponentUpdate(nextProps, nextState, nextContext) =
            Fable.Import.Browser.console.log("shouldComponentUpdate", nextProps, nextState, nextContext)
            match props.state with
            | Some previous -> not <| props.equal previous props.nextState
            | _ -> true

        member this.render () =
            props.state <- Some this.state
            props.view props.nextState 

[<AutoOpen>]
module Common =
    /// Make props for the root React App component
    let internal toAppProps run (program:Program<'arg,'model,'msg,_>) =
        { main = fun setState -> 
                    let dispatch = run setState program
                    fun model -> program.view model dispatch }

    let lazyView (view:'model->'msg Dispatch->ReactElement<_>) (dispatch:'msg Dispatch) =
        let mutable lastState = None
        fun state -> { view = fun model -> view model dispatch
                       state = lastState
                       nextState = state
                       equal = (=) } |> Components.LazyView
                
    let lazyViewWith (equal:'model->'model->bool) (view:'model->'msg Dispatch->ReactElement<_>) (dispatch:'msg Dispatch) =
        let mutable lastState = None
        fun state -> { view = fun model -> view model dispatch
                       state = lastState
                       nextState = state
                       equal = equal } |> Components.LazyView


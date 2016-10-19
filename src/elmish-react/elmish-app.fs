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
    view:('model->ReactElement<obj>)
    state:'model option ref
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

    type LazyApp<'model>(props:LazyProps<'model>) as this =
        inherit Component<LazyProps<'model>,'model>(props)
        member this.shouldComponentUpdate(nextProps, nextState, nextContext) =
            match props.state with
            | Some previous -> previous <> props.nextState
            | _ -> true
        member this.render () =
            props.state := Some state
            view state dispatch 

module Props =
    /// Make props for the root React App component
    let ofProgram run (program:Program<'arg,'model,'msg,_>) =
        { main = fun setState -> 
                    let dispatch = run setState program
                    fun model -> program.view model dispatch }

module Views =
    let lazy2 (view:'model->'msg Dispatch->unit) (dispatch:'msg Dispatch) =
        let mutable lastState = None
        fun state ->
            


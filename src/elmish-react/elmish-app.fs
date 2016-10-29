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
    state:'model
    render:unit->ReactElement<obj>
    equal:'model->'model->bool 
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

    type LazyView<'model>(props) =
        inherit Component<LazyProps<'model>,obj>(props)

        member this.shouldComponentUpdate(nextProps, nextState, nextContext) =
            not <| this.props.equal nextProps.state this.props.state

        member this.render () =
            this.props.render ()

[<AutoOpen>]
module Common =
    /// Make props for the root React App component
    let internal toAppProps run (program:Program<'arg,'model,'msg,_>) =
        { main = fun setState -> 
                    let dispatch = run setState program
                    fun model -> program.view model dispatch }
                
    /// Avoid rendering the view unless the model has changed.
    /// equal: function the compare the previous and the new states
    /// view: function to render the model
    /// state: new state to render
    let lazyViewWith (equal:'model->'model->bool) 
                     (view:'model->ReactElement<_>) 
                     (state:'model) =
        com<Components.LazyView<_>,_,_> 
            { render = fun () -> view state
              equal = equal
              state = state }
            []

    /// Avoid rendering the view unless the model has changed.
    /// equal: function the compare the previous and the new states
    /// view: function to render the model using the dispatch
    /// state: new state to render
    /// dispatch: dispatch function
    let lazyView2With (equal:'model->'model->bool) 
                             (view:'model->'msg Dispatch->ReactElement<_>) 
                             (state:'model) 
                             (dispatch:'msg Dispatch) =
        com<Components.LazyView<_>,_,_> 
            { render = fun () -> view state dispatch
              equal = equal
              state = state }
            []

    /// Avoid rendering the view unless the model has changed.
    /// equal: function the compare the previous and the new model (a tuple of two states)
    /// view: function to render the model using the dispatch
    /// state1: new state to render
    /// state2: new state to render
    /// dispatch: dispatch function
    let lazyView3With (equal:_->_->bool) (view:_->_->_->ReactElement<_>) state1 state2 (dispatch:'msg Dispatch) =
        com<Components.LazyView<_>,_,_> 
            { render = fun () -> view state1 state2 dispatch
              equal = equal
              state = (state1,state2) }
            []

    /// Avoid rendering the view unless the model has changed.
    /// view: function of model to render the view
    let inline lazyView (view:'model->ReactElement<_>) =
        lazyViewWith (=) view

    /// Avoid rendering the view unless the model has changed.
    /// view: function of two arguments to render the model using the dispatch
    let inline lazyView2 (view:'model->'msg Dispatch->ReactElement<_>) =
        lazyView2With (=) view

    /// Avoid rendering the view unless the model has changed.
    /// view: function of three arguments to render the model using the dispatch
    let inline lazyView3 (view:_->_->_->ReactElement<_>) =
        lazyView3With (=) view
                     


namespace Elmish.React

open System
open Fable.Import.React
open Fable.Core
open Elmish

type MkView<'model> = ('model->unit) -> ('model->ReactElement<obj>)
type Props<'model> = {
    main:MkView<'model>
}

module Components =
    let mutable internal mounted = false

    type App<'model>(props:Props<'model>) as this =
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

module Props =
    /// Make props for the root React App component
    let ofProgram run (program:Program<'arg,'model,'msg,_>) =
        { main = fun setState -> 
                    let dispatch = run setState program
                    fun model -> program.view model dispatch }

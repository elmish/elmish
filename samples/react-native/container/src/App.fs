module AwesomeApp

open Fable.Import
open Elmish

module C = Container

let program =
    Program.mkProgram C.init C.update
    |> Program.withConsoleTrace

type App() as this =
    inherit React.Component<obj, C.Model>()

    let safeState state =
        match unbox this.props with
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        this.props <- true

    member this.render() =
        C.view this.state dispatch

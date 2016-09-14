module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open AwesomeTypes

open Fable.Elmish

let program = Program.mkProgram AwesomeState.init AwesomeState.update
              |> Program.withSubscription AwesomeState.subscribe
              |> Program.withConsoleTrace

type App() as this =
    inherit React.Component<obj,AwesomeTypes.Model>()
    
    let safeState state =
        match unbox this.props with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        this.props <- true

    member this.render () =
        navigator [
            InitialRoute (createRoute("Start",0))
            RenderScene (Func<_,_,_>(fun route navigator ->
                match route.id with
                | _ ->
                    MainScene.render this.state.MainState (Msg.MainMsg >> dispatch)
            ))
        ]

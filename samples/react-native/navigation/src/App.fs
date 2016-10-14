module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Types
open Types.App
open State
open Scenes

open Elmish

let program = Program.mkProgram App.init App.update
              |> Program.withSubscription App.subscribe
              |> Program.withConsoleTrace

let mutable mounted = false

type App() as this =
    inherit React.Component<obj,App.Model>()
    do
        mounted <- false
    
    let safeState state =
        match mounted with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        mounted <- true

    member this.render () =
        let jumpTo n =
            fun _ -> Navigate.JumpTo n |> (NavigationMsg >> dispatch)

        let scene (ps:NavigationTransitionProps) =
            match ps.scene.route.key with
            | Routes.Current -> Current.render this.state.CurrentState (CurrentMsg >> dispatch)
            | Routes.Retrieved -> Retrieved.render this.state.RetrievedState (RetrievedMsg >> dispatch)
            | _ -> Main.render this.state.MainState (MainMsg >> dispatch)
        
        let header (ps:NavigationTransitionProps) =
            navigationHeader [NavigationHeaderProps.RenderTitleComponent (fun _ -> text [TextProperties.Style [TextAlign TextAlignment.Center]] (string ps.scene.route.title))
                              NavigationHeaderProps.RenderLeftComponent (fun _ -> touchableOpacity [OnPress <| jumpTo (ps.navigationState.index-1)] [text [] "<-"])
                              NavigationHeaderProps.RenderRightComponent (fun _ -> touchableOpacity [OnPress <| jumpTo (ps.navigationState.index+1)] [text [] "->"])]
                             ps
        
        navigationCardStack this.state.NavigationState  
                            scene 
                            [NavigationCardStackProps.RenderHeader header]

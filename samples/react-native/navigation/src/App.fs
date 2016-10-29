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
open Elmish.React
open Elmish.ReactNative

let view model dispatch =
    let jumpTo n =
        fun _ -> Navigate.JumpTo n |> (NavigationMsg >> dispatch)

    let scene (ps:NavigationTransitionProps) =
        match ps.scene.route.key with
        | Routes.Current -> lazyView2 Current.render model.CurrentState (CurrentMsg >> dispatch)
        | Routes.Retrieved -> lazyView2 Retrieved.render model.RetrievedState (RetrievedMsg >> dispatch)
        | _ -> lazyView2 Main.render model.MainState (MainMsg >> dispatch)
    
    let header (ps:NavigationTransitionProps) =
        navigationHeader [ NavigationHeaderProps.RenderTitleComponent (fun _ -> text [TextProperties.Style [TextAlign TextAlignment.Center]] (string ps.scene.route.title))
                           NavigationHeaderProps.RenderLeftComponent (fun _ -> touchableOpacity [OnPress <| jumpTo (ps.navigationState.index-1)] [text [] "<-"])
                           NavigationHeaderProps.RenderRightComponent (fun _ -> touchableOpacity [OnPress <| jumpTo (ps.navigationState.index+1)] [text [] "->"])]
                         ps
    
    navigationCardStack model.NavigationState  
                        scene 
                        [NavigationCardStackProps.RenderHeader header]

let runnable :obj->obj = 
    Program.mkProgram App.init App.update view
    |> Program.withSubscription App.subscribe
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run

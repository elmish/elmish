module internal Scenes.Retrieved

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Types.Retrieved

let render (state:State) dispatch =
      view [ Styles.sceneBackground ] 
        [ text [ Styles.titleText ] "timeapi.org says:"
          text [ Styles.titleText ] state.value
          Styles.button "retrieve" (fun _ -> dispatch GetTime) ]
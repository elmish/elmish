module internal MainScene

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open MainTypes

let render (state:MainState) dispatch =
      view [ Styles.sceneBackground ] 
        [ text [ Styles.titleText ] state.retrieved
          Styles.button "Get Time" (fun _ -> dispatch GetTime)
          text [ Styles.titleText ] state.current ]
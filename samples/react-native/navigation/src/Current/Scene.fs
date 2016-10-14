module internal Scenes.Current

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Types.Current

let render (state:State) dispatch =
      view [ Styles.sceneBackground ] 
        [ text [ Styles.titleText ] "Current system time:"
          text [ Styles.titleText ] state.value ]
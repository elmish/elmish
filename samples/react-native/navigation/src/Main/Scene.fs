module internal Scenes.Main

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Types.Main

let render (state:State) _ =
      view [ Styles.sceneBackground ] 
        [ text [ Styles.titleText ] "Go <- left to see current system time"
          text [ Styles.titleText ] "Go right -> to see timeapi.org/utc/ time" ]
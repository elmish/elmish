module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

open Elmish

// MODEL

type Model = int


type Msg =
  | Increment
  | Decrement


let init () =
  0


// UPDATE

let update (msg:Msg) count =
  match msg with
  | Increment ->
      count + 1

  | Decrement ->
      count - 1



// rendering views with ReactNative
module R = Fable.Helpers.ReactNative

let view count (dispatch:Dispatch<Msg>) =
  let onClick msg =
    fun () -> msg |> dispatch 

  R.view [Styles.sceneBackground]
    [ Styles.button "-" (onClick Decrement)
      R.text [] (string count)
      Styles.button "+" (onClick Increment)
    ]

open Elmish.ReactNative
open Elmish.Debug

// App
Program.mkSimple init update view
|> Program.withReactNative "counter"
|> Program.withDebuggerAt (Debugger.Remote ("localhost",8000))
|> Program.run

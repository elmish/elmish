module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

open Elmish

// MODEL

type Model = {count:int}


type Msg =
  | Increment
  | Decrement


let init () =
  { count = 0 }


// UPDATE

let update (msg:Msg) {count = count} =
  match msg with
  | Increment ->
      { count = count + 1 }

  | Decrement ->
      { count = count - 1 }



// rendering views with ReactNative
module R = Fable.Helpers.ReactNative

let view {count = count} (dispatch:Dispatch<Msg>) =
  let onClick msg =
    fun () -> msg |> dispatch 

  R.view [Styles.sceneBackground]
    [ Styles.button "-" (onClick Decrement)
      R.text [] (string count)
      Styles.button "+" (onClick Increment)
    ]

open Elmish.ReactNative
// App
let runnable:obj->obj = 
    Program.mkSimple init update view
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run

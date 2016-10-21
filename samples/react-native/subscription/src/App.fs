module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

open Elmish

// Model

type Msg =
  | Increment
  | Decrement

type Model = 
  { count:int }

let init () =
  { count = 0 }, []

// Update

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
  match msg with
  | Increment ->
    { model with
        count = model.count + 1 }, []
  | Decrement ->
    { model with
        count = model.count - 1 }, []

// Subscriptions
let timerTick dispatch =
  let t = new Timers.Timer 1000.
  t.Elapsed.Subscribe(fun _ -> dispatch Increment) |> ignore
  t.Enabled <- true

let subscription _ =
  Cmd.ofSub timerTick


// rendering views with ReactNative
module R = Fable.Helpers.ReactNative

let view {count = count} dispatch =
  let onClick msg =
    fun _ -> msg |> dispatch 

  R.view []
    [ Styles.button "-" (onClick Decrement)
      R.text [] (string count)
      Styles.button "+" (onClick Increment)
    ]

open Elmish.ReactNative
// App
let runnable : obj->obj =
    Program.mkProgram init update view
    |> Program.withSubscription subscription
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run


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
  | NoOp

type Model = 
  { count:int
    currentMsg:Msg }

let init () =
  { count = 0
    currentMsg = NoOp }, []

// Update

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
  match msg with
  | Increment ->
    { model with
        count = model.count + 1
        currentMsg = Increment }, []
  | Decrement ->
    { model with
        count = model.count - 1
        currentMsg = Decrement }, []
  | NoOp ->
      model, []

// Subscriptions
let timerTick model dispatch =
  let t = new Timers.Timer 1000.
  t.Elapsed.Subscribe(fun _ -> dispatch Increment) |> ignore
  t.Enabled <- true

let subscription model =
  Cmd.ofSub (timerTick model)


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

// App
let program =
    Program.mkProgram init update
    |> Program.withSubscription subscription
    |> Program.withConsoleTrace

type App() as this =
    inherit React.Component<obj, Model>()

    let safeState state =
        match unbox this.props with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        this.props <- true

    member this.render() =
        view this.state dispatch

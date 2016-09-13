module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

open Fable.Elmish

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
    Program.mkSimple init update
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
        
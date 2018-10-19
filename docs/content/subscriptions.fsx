(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/bin/Debug/netstandard2.0"
#r "Fable.Elmish.dll"

(** Working with external sources of events
---------
Sometimes we have a source of events that doesn't depend on the current state of the model, like a timer.
We can setup forwarding of those events to be processed by our `update` function like any other change.

Let's define our `Model` and `Msg` types. `Model` will hold the current state and `Msg` will tell us the nature of the change that we need to apply to the current state.
*)

open System


type Model =
    { current : DateTime }

type Msg = 
    | Tick of DateTime


(** 
This time we'll define the "simple" version of `init` and `update` functions, that don't produce commands:
*)

let init () =
    { current = DateTime.Now }

let update msg model =
    match msg with
    | Tick current ->
        { model with current = current }

(** 

Note that "simple" is not a requirement and is just a matter of convenience for the purpose of the example!

Now lets define our timer subscription:

*)
open Elmish
open Fable.Import.Browser

let timer initial =
    let sub dispatch = 
        window.setInterval(fun _ -> 
            dispatch (Tick DateTime.Now)
            , 1000) |> ignore
    Cmd.ofSub sub

Program.mkSimple init update (fun model _ -> printf "%A\n" model)
|> Program.withSubscription timer
|> Program.run


(** 
In this example program initialization will call our subscriber (once) with inital `Model` state, passing the `dispatch` function to be called whenever an event occurs.
However, any time you need to issue a message (for example from a callback) you can use `Cmd.ofSub`.
*)

(** 
### Aggregating multiple subscribers
If you need to aggregate multiple subscriptions follow the same pattern as when implementing `init`, `update`, and `view` functions - delegate to child components to setup their own subscriptions.

For example:
*)

module Second =
    type Msg =
        | Second of int
    
    type Model = int

    let subscribe initial =
        let sub dispatch = 
            window.setInterval(fun _ -> 
                dispatch (Second DateTime.Now.Second)
                , 1000) |> ignore
        Cmd.ofSub sub

    let init () =
        0

    let update (Second seconds) model =
        seconds

module Hour =
    type Msg =
        | Hour of int

    type Model = int

    let init () =
        0

    let update (Hour hours) model =
        hours

    let subscribe initial =
        let sub dispatch = 
            window.setInterval(fun _ -> 
                dispatch (Hour DateTime.Now.Hour)
                , 1000*60) |> ignore
        Cmd.ofSub sub

module App =
    type Model =
        { seconds : Second.Model
          hours : Hour.Model }

    type Msg = 
        | SecondMsg of Second.Msg
        | HourMsg of Hour.Msg

    let init () =
        { seconds = Second.init()
          hours = Hour.init() }

    let update msg model =
        match msg with
        | HourMsg msg -> 
            { model with hours = Hour.update msg model.hours }
        | SecondMsg msg -> 
            { model with seconds = Second.update msg model.seconds }

    let subscription model =
        Cmd.batch [ Cmd.map HourMsg (Hour.subscribe model.hours)
                    Cmd.map SecondMsg (Second.subscribe model.seconds) ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscription
    |> Program.run

(**
---
layout: standard
title: Subscriptions
---
**)

(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use
// it to define helpers that you do not want to show in the documentation.

#I "../../src/bin/Debug/netstandard2.0"
#r "Fable.Elmish.dll"
#r "nuget: Fable.Core"

(**

### Working with external sources of events

Sometimes we have a source of events that doesn't depend on the current state of the model, like a timer.
We can setup forwarding of those events to be processed by our `update` function like any other change.

Let's define our `Model` and `Msg` types. `Model` will hold the current state and `Msg` will tell us the nature of the change that we need to apply to the current state.
*)

open Elmish
open Fable.Core
open System


module Example1 =

    type Model =
        {
            current : DateTime
        }

    type Msg =
        | Tick of DateTime

(**
This time we'll define the "simple" version of `init` and `update` functions, that don't produce commands:
*)

    let init () =
        {
            current = DateTime.MinValue
        }

    let update msg model =
        match msg with
        | Tick current ->
            { model with
                current = current
            }

(**

Note that "simple" is not a requirement and is just a matter of convenience for the example!

Now lets define our timer subscription:

*)
    let timer onTick =
        let subId = ["timer"]
        let start dispatch =
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    1000
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }
        subId, start

    let subscribe model =
        [ timer Tick ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run


(**
`subscribe` answers the question: "Which subscriptions should be running?" It has the current program state, `model`, to use for decisions.
When the model changes, `subscribe` is called. Elmish then starts or stops subscriptions to match what should be running.

Here, the `timer` subscription is always returned, so it will stay running as long as the program is running.

Let's look at an example where the timer can be turned off. First up, add the field `enabled`.
*)

module Example2 =

    type Model =
        {
            current : DateTime
            enabled : bool
        }

    type Msg =
        | Tick of DateTime

    let init () =
        {
            current = DateTime.MinValue
            enabled = true
        }

(**
`update` and `timer` are the same as before.
*)
    let update msg model =
        match msg with
        | Tick current ->
            { model with
                current = current
            }

    let timer onTick =
        let subId = ["timer"]
        let start dispatch =
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    1000
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }
        subId, start

(**
Next, change the subscribe function to check `enabled` before including the `timer` subscription.
*)
    let subscribe model =
        [ if model.enabled then
            timer Tick ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run

(**
All that's left is to add user interaction to change `enabled`. The timer will stop or start accordingly.
*)

(**
### Aggregating multiple subscribers
If you need to aggregate multiple subscriptions follow the same pattern as when implementing `init`, `update`, and `view` functions - delegate to child components to setup their own subscriptions.

For example:
*)

module Sub =

    // a reusable subscription
    let timer intervalMs onTick =
        let subId = ["timer"]
        let start dispatch =
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    intervalMs
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }
        subId, start


module Second =
    type Msg =
        | Second of int

    type Model = int

    let init () =
        0

    let update (Second seconds) model =
        seconds

    let subscribe model =
        [ Sub.timer 1000 (fun now -> Second now.Second) ]

module Hour =
    type Msg =
        | Hour of int

    type Model = int

    let init () =
        0

    let update (Hour hour) model =
        hour

    let subscribe model =
        [ Sub.timer (60*1000) (fun now -> Hour now.Hour) ]

module App =
    type Model =
        {
            seconds : Second.Model
            hours : Hour.Model
        }

    type Msg =
        | SecondMsg of Second.Msg
        | HourMsg of Hour.Msg

    let init () =
        {
            seconds = Second.init()
            hours = Hour.init()
        }

    let update msg model =
        match msg with
        | HourMsg msg ->
            { model with
                hours = Hour.update msg model.hours
            }

        | SecondMsg msg ->
            { model with
                seconds = Second.update msg model.seconds
            }

    let subscribe model =
        Sub.batch [
            Sub.map "hour" HourMsg (Hour.subscribe model.hours)
            Sub.map "second" SecondMsg (Second.subscribe model.seconds)
        ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run

(**
Notice `Sub.map` takes an id prefix as its first parameter. This helps keep subscriptions distinct from each other.

Before Sub.map, Second and Hour have the same ID: `["timer"]`. After Sub.map, their IDs are: `["hour"; "timer"]` and `["second"; "timer"]`.
*)

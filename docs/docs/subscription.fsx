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
        let start dispatch =
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    1000
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }
        start

    let subscribe model =
        [ ["timer"], timer Tick ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run


(**
`subscribe` answers the question: "Which subscriptions should be running?" It has the current program state, `model`, to use for decisions.
When the model changes, `subscribe` is called. Elmish then starts or stops subscriptions to match what should be running.

A subscription has an ID, `["timer"]` here, and a start function. You're probably wondering why the ID is a list.
It allows you to include dependencies. Later we will use this to make the timer change intervals.

Here, the timer subscription is always returned, so it will stay running as long as the program is running.

Let's look at an example where the timer can be turned off. First we add the field `enabled`.
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
        let start dispatch =
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    1000
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }
        start

(**
Next, change the subscribe function to check `enabled` before including the `timer` subscription.
*)
    let subscribe model =
        [ if model.enabled then
            ["timer"], timer Tick ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run

(**
All that's left is to add user interaction to change `enabled`. The timer will stop or start accordingly.

Note that above I used `let` to define the start fn, then return it. You will often see a function return written like this:
*)

module Example3 =

    let timer onTick =
        fun dispatch ->
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    1000
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }

(**
This returns an anonymous function. Defining the function with let made it more understandable for me. Use whichever style fits your brain better.
*)

(**
### Aggregating multiple subscribers
If you need to aggregate multiple subscriptions follow the same pattern as when implementing `init`, `update`, and `view` functions - delegate to child components to setup their own subscriptions.

For example:
*)

module Sub =

    // a reusable subscription
    let timer intervalMs onTick =
        let start dispatch =
            let intervalId = 
                JS.setInterval
                    (fun _ -> dispatch (onTick DateTime.Now))
                    intervalMs
            { new IDisposable with
                member _.Dispose() = JS.clearInterval intervalId }
        start


module Second =
    type Msg =
        | Second of int

    type Model = int

    let init () =
        0

    let update (Second seconds) model =
        seconds

    let subscribe model =
        [ ["timer"], Sub.timer 1000 (fun now -> Second now.Second) ]

module Hour =
    type Msg =
        | Hour of int

    type Model = int

    let init () =
        0

    let update (Hour hour) model =
        hour

    let subscribe model =
        [ ["timer"], Sub.timer (60*1000) (fun now -> Hour now.Hour) ]

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

(**
### Subscription reusability with Effect

> 📌 **Effect**
> The `Effect` type was known as `Sub` in v3. With the change to subscriptions, Sub would have been an overloaded and confusing term.
> Effect works exactly the same as before. Only the names have been changed to protect the innocent.

In the last section, we increased reusability of `timer` by taking the interval as a parameter.
We can use Effect to factor out the hard-coded DateTime.Now behavior for even more reuse.

First let's turn DateTime.Now into an Effect.
*)

module Example4 =

    module Time =

        let now (tag: DateTime -> 'msg) : Effect<'msg> =
            let effectFn dispatch =
                dispatch (tag DateTime.Now)
            effectFn

(**
> Strange as it sounds, DateTime.Now _is_ a side effect. This property reads from a clock and may return a different value each time. Code that uses it becomes nondeterministic.

Now let's change timer to run an Effect when the interval fires.
*)

    module Sub =

        let timer intervalMs (effect: Effect<'msg>) =
            let start dispatch =
                let intervalId = 
                    JS.setInterval (fun _ -> effect dispatch) intervalMs
                { new IDisposable with
                    member _.Dispose() = JS.clearInterval intervalId }
            start

(**
And finally, here is the updated `subscribe` function.
*)

    type Model =
        {
            current : DateTime
            intervalMs : int
        }

    type Msg =
        | Tick of DateTime

    let subscribe model =
        [ ["timer"], Sub.timer model.intervalMs (Time.now Tick) ]

(**
The timer subscription can now run any kind of effect. Calling an API, playing a sound, whatever.

`Time.now` is also usable for Cmds.
*)

    let init () =
        {
            current = DateTime.MinValue
            intervalMs = 1000
        }
        , Cmd.ofEffect (Time.now Tick)

(**
### IDs and dependencies

I mentioned earlier that ID is a list so that you can add dependencies to it. What does this mean?

In the last example, the timer's interval was pulled from the model:

`Sub.timer model.intervalMs (Time.now Tick)`

But nothing happens to the subscription if `model.intervalMs` changes. Let's fix that.
*)

module IdDeps1 =

    open Example4 // reuse last example's code

    let subscribe model =
        [ ["timer"; string model.intervalMs], Sub.timer model.intervalMs (Time.now Tick) ]

(**
Now that intervalMs is part of the ID, an interesting thing happens. When the interval changes, the timer will stop then restart with the new interval.

How does it work? It's taking advantage of ID uniqueness.

Let's say that `intervalMs` is initially 1000. The sub ID is `["timer"; "1000"]`, Elmish starts the subscription.
Then `intervalMs` changes to 2000. The sub ID becomes `["timer"; "2000"]`.
Elmish sees that `["timer"; "1000"]` is no longer active and stops it. Then it starts the "new" subscription `["timer"; "2000"]`.

To Elmish each interval is a different subscription. But to `subscribe` this is a single timer that changes intervals.

Let's look at another example: multiple timers.
*)

module IdDeps2 =

    // type alias to make the model more legible, in theory
    type TimerId = int
    type Interval = int

    type Model =
        {
            timers : Map<TimerId, Interval>
            nextId : TimerId // incrementing ID
        }

    type Msg =
        | AddTimer of interval: int
        | RemoveTimer of timerId: int

    let init () =
        {
            timers = Map.empty
            nextId = 1
        }

    let update msg model =
        match msg with
        | AddTimer interval ->
            {
                timers = Map.add model.nextId interval model.timers
                nextId = model.nextId + 1
            }
        | RemoveTimer timerId ->
            { model with
                timers = Map.remove timerId model.timers
            }

    let printEffect timerId interval =
        fun _ ->
            printfn "timerId = %i, interval = %i, now = %A" timerId interval DateTime.Now

    let subscribe model =
        [ for (timerId, interval) in Map.toList model.timers do
            ["timer"; string timerId], Sub.timer interval (printEffect timerId interval) ]

(**
This example supports adding and removing timers. Each timer will have its own subscription.
*)
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
#r "nuget: Feliz"

(**

> Subscriptions changed in v4. For v3 and earlier subscription, see [Subscriptions (v3)](./subscriptionv3.html).
> Or see [Migrating from v3](#migrating-from-v3) below.

## Working with external sources of events

Sometimes we have a source of events that runs independently of Elmish, like a timer.
We can use subscriptions control when those sources are running, and forward its events to our `update` function.

Let's define our `Model` and `Msg` types. `Model` will hold the current state and `Msg` will tell us the nature of the change that we need to apply to the current state.
*)

open Elmish
open Fable.Core
open System


module BasicTimer =

    type Model =
        {
            current : DateTime
        }

    type Msg =
        | Tick of DateTime

(**
Now let's define `init` and `update`.
*)

    let init () =
        {
            current = DateTime.MinValue
        }, []

    let update msg model =
        match msg with
        | Tick current ->
            { model with
                current = current
            }, []

(**
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

    Program.mkProgram init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run


(**
`subscribe` answers the question: "Which subscriptions should be running?" `subscribe` is provided the current program state, `model`, to use for decisions.
When the model changes, `subscribe` is called. Elmish then starts or stops subscriptions to match what is returned.

A subscription has an ID, `["timer"]` here, and a start function. The ID needs to be unique within that page.

> **ID is a list?**
> 
> This allows us to include dependencies. Later we will use this to change the timer's interval.


## Conditional subscriptions

In the above example, the timer subscription is always returned from `subscribe`, so it will stay running as long as the program is running.
Let's look at an example where the timer can be turned off.

First we add the field `enabled` and a msg `Toggle` to change it.
*)

module ToggleTimer =

    type Model =
        {
            current : DateTime
            enabled : bool
        }

    type Msg =
        | Tick of now: DateTime
        | Toggle of enabled: bool

    let init () =
        {
            current = DateTime.MinValue
            enabled = true
        }, []

(**
Now let's handle the `Toggle` message.
*)
    let update msg model =
        match msg with
        | Tick now ->
            { model with
                current = now
            }, []
        | Toggle enabled ->
            { model with
                enabled = enabled
            }, []

(**
`timer` is the same as before.
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

(**
Next, we change the subscribe function to check `enabled` before including the timer subscription.
*)
    let subscribe model =
        [ if model.enabled then
            ["timer"], timer Tick ]

(**
Now let's add an HTML view to visualize and control the timer.
*)

    open Feliz

    let view model dispatch =
        let timestamp = model.current.ToString("yyyy-MM-dd HH:mm:ss.ffff")
        Html.div [
            Html.div [Html.text timestamp]
            Html.div [
                Html.label [
                    prop.children [
                        Html.input [
                            prop.type' "checkbox"
                            prop.isChecked model.enabled
                            prop.onCheckedChange (fun b -> dispatch (Toggle b))
                        ]
                        Html.text " enabled"
                    ]
                ]
            ]
        ]


    Program.mkProgram init update view
    |> Program.withSubscription subscribe
    |> Program.run

(**
Here is the running program.

![demo of user toggling checkbox to stop and start time updates](../static/img/timer-with-enable.gif)
*)

(**
## Aggregating multiple subscribers
If you need to aggregate multiple subscriptions follow the same pattern as when implementing `init`, `update`, and `view` functions - delegate to child components to setup their own subscriptions.

For example:
*)

module Sub =

    // now usable for different intervals
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
        0, []

    let update (Second seconds) model =
        seconds, []

    let subscribe model =
        [ ["timer"], Sub.timer 1000 (fun now -> Second now.Second) ]

module Hour =

    type Msg =
        | Hour of int

    type Model = int

    let init () =
        0, []

    let update (Hour hour) model =
        hour, []

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
        let seconds, secondsCmd = Second.init ()
        let hours, hoursCmd = Hour.init ()
        {
            seconds = seconds
            hours = hours
        }, Cmd.batch [Cmd.map SecondMsg secondsCmd; Cmd.map HourMsg hoursCmd]

    let update msg model =
        match msg with
        | HourMsg msg ->
            let hours, hoursCmd = Hour.update msg model.hours
            { model with
                hours = hours
            }, Cmd.map HourMsg hoursCmd

        | SecondMsg msg ->
            let seconds, secondsCmd = Second.update msg model.seconds
            { model with
                seconds = seconds
            }, Cmd.map SecondMsg secondsCmd

    let subscribe model =
        Sub.batch [
            Sub.map "hour" HourMsg (Hour.subscribe model.hours)
            Sub.map "second" SecondMsg (Second.subscribe model.seconds)
        ]

    Program.mkProgram init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription subscribe
    |> Program.run

(**
Notice `Sub.map` takes an id prefix as its first parameter. This helps keep subscriptions distinct from each other.

Before `Sub.map`, Second and Hour have the same ID: `["timer"]`. After `Sub.map`, their IDs are: `["hour"; "timer"]` and `["second"; "timer"]`.

It is common for parent pages to have one active child page. This partial example shows `subscribe` in that case.
*)

(*** hide ***)
module ActiveChildPage =
    open App

(**
*)

    type Msg =
        | SecondMsg of Second.Msg
        | HourMsg of Hour.Msg

    type Page =
        | Second of Second.Model
        | Hour of Hour.Model

    type Model =
        {
            page: Page
        }

    let subscribe model =
        match model.page with
        | Second model_ ->
            Sub.map "second" SecondMsg (Second.subscribe model_)
        | Hour model_ ->
            Sub.map "hour" HourMsg (Hour.subscribe model_)

(**
When the active page changes, the old page's subscriptions are stopped and the new page's subscriptions are started.
*)

(**
## Subscription reusability

> 📌 **Effect**
> 
> The `Effect` type was known as `Sub` in v3. With the change to subscriptions, `Sub` would have been an overloaded and confusing term.
> Helper functions have also been renamed. For example, `Cmd.ofSub` is now `Cmd.ofEffect`.
> This is a change in name only -- `Effect` works exactly the same as v3 `Sub`.

In the last section, we increased reusability of the timer by taking the interval as a parameter.
We can use Effect to factor out the hard-coded `DateTime.Now` behavior for even more reuse.

First let's turn `DateTime.Now` into an Effect.
*)

module EffectTimer =

    module Time =

        let now (tag: DateTime -> 'msg) : Effect<'msg> =
            let effectFn dispatch =
                dispatch (tag DateTime.Now)
            effectFn

(**
> Strange as it seems, `DateTime.Now` is a side effect. This property reads from a clock and may return a different value each time. Code that uses it becomes nondeterministic.

Now let's change timer to run an `Effect` when the interval fires.
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
The timer subscription can now run any kind of effect. Calling an API, playing a sound, etc.

`Time.now` is also usable for Cmds.
*)

    let init () =
        {
            current = DateTime.MinValue
            intervalMs = 1000
        }
        , Cmd.ofEffect (Time.now Tick)

(**
## IDs and dependencies

Earlier we noted that ID is a list so that you can add dependencies to it. We'll use that to improve the last example.

In that example, the timer's interval came from the model:

```fsharp
Sub.timer model.intervalMs (Time.now Tick)
```

But nothing happens to the subscription if `model.intervalMs` changes. Let's fix that.
*)

(*** hide ***)
module IdDeps =

    open EffectTimer // reuse last example's code

(**
*)
    let subscribe model =
        let subId = ["timer"; string model.intervalMs]
        [ subId, Sub.timer model.intervalMs (Time.now Tick) ]

(**
Now that `intervalMs` is part of the ID, the timer will stop the old interval then start with the new interval whenever the interval changes.

How does it work? It is taking advantage of ID uniqueness.
Let's say that `intervalMs` is initially 1000. The sub ID is `["timer"; "1000"]`, Elmish starts the subscription.
Then `intervalMs` changes to 2000. The sub ID becomes `["timer"; "2000"]`.
Elmish sees that `["timer"; "1000"]` is no longer active and stops it. Then it starts the "new" subscription `["timer"; "2000"]`.

To Elmish each interval is a different subscription. But to `subscribe` this is a single timer that changes intervals.

Let's look at another example: multiple timers. This one will be a full-fledged example including UI forms to take user input.
*)

module MultipleTimers =

    module Time =

        let now (tag: DateTime -> 'msg) : Effect<'msg> =
            let effectFn dispatch =
                dispatch (tag DateTime.Now)
            effectFn

    module Sub =

        let timer intervalMs (effect: Effect<'msg>) =
            let start dispatch =
                let intervalId = 
                    JS.setInterval (fun _ -> effect dispatch) intervalMs
                { new IDisposable with
                    member _.Dispose() = JS.clearInterval intervalId }
            start

(**
This includes the same `Sub.timer` subscription and `Time.now` effect from the last example.

Now let's create a reusable form that accepts timer interval changes and validates them.
*)

    // type aliases to make the model more legible, in theory
    type TimerId = int
    type IntervalMs = int

    type FormState =
        | Unchanged
        | NewValue of IntervalMs
        | Invalid of error: string

    type IntervalForm =
        {
            current : IntervalMs
            userEntry : string
            state : FormState
        }

    module IntervalForm =
        let [<Literal>] NoInterval = -1

        let empty =
            {
                current = NoInterval
                userEntry = ""
                state = Unchanged
            }

        let create intervalMs =
            {
                current = intervalMs
                userEntry = string intervalMs
                state = Unchanged
            }

        let reset form =
            let userEntry =
                if form.current = NoInterval then
                    ""
                else
                    string form.current
            {form with userEntry = userEntry; state = Unchanged}

        let validate form =
            match Int32.TryParse form.userEntry with
            | false, _ when form.current = NoInterval && form.userEntry = "" ->
                {form with state = Unchanged}
            | false, _ when form.userEntry = "" ->
                {form with state = Invalid "cannot be blank"}
            | false, _ ->
                {form with state = Invalid "must be an integer"}
            | true, interval when interval <= 0 ->
                {form with state = Invalid "must be greater than zero"}
            | true, interval when interval = form.current ->
                {form with state = Unchanged}
            | true, interval ->
                {form with state = NewValue interval}

        let userEntry text form =
            validate {form with userEntry = text}

(**
Now let's create the overall model to manage timers.
*)


    type Model =
        {
            // lookup by TimerId, last tick and interval settings
            timers : Map<TimerId, DateTime * IntervalForm>
            // incrementing ID
            nextId : TimerId
            // form for the user to add a new interval
            addForm : IntervalForm
        }

    type Msg =
        | AddFormReset
        | AddFormUserEntry of text: string
        | ChangeFormReset of timerId: int
        | ChangeFormUserEntry of timerId: int * text: string
        | AddTimer of intervalMs: int
        | RemoveTimer of timerId: int
        | ChangeInterval of timerId: int * intervalMs: int
        | Tick of timerId: int * now: DateTime

     module Model =

        let updateTimer timerId f model =
            { model with
                timers =
                    model.timers
                    |> Map.change timerId (Option.map f)
            }

        let updateForm timerId f model =
            model |> updateTimer timerId (fun (now, form) -> now, f form)

        let updateNow timerId newNow model =
            model |> updateTimer timerId (fun (now, form) -> newNow, form)

    let init () =
        {
            timers = Map.empty
            nextId = 1
            addForm = IntervalForm.empty
        }, []

    let update msg model =
        match msg with
        // form changes
        | AddFormReset ->
            { model with
                addForm = IntervalForm.reset model.addForm
            }, []
        | AddFormUserEntry text ->
            { model with
                addForm = IntervalForm.userEntry text model.addForm
            }, []
        | ChangeFormReset timerId ->
            Model.updateForm timerId IntervalForm.reset model, []
        | ChangeFormUserEntry (timerId, text) ->
            Model.updateForm timerId (IntervalForm.userEntry text) model, []
        // timer changes
        | AddTimer intervalMs ->
            { model with
                timers =
                    let form = IntervalForm.create intervalMs
                    Map.add model.nextId (DateTime.MinValue, form) model.timers
                nextId = model.nextId + 1
                addForm = IntervalForm.empty
            }, [Time.now (fun now -> Tick (model.nextId, now))]
        | RemoveTimer timerId ->
            { model with
                timers = Map.remove timerId model.timers
            }, []
        | ChangeInterval (timerId, intervalMs) ->
            let form = IntervalForm.create intervalMs
            Model.updateForm timerId (fun _ -> form) model, []
        | Tick (timerId, now) ->
            Model.updateNow timerId now model, []

    let subscribe model =
        [
            for timerId, (_now, {current = intervalMs}) in Map.toList model.timers do
                let subId = ["timer"; string timerId; string intervalMs]
                let tick now = Tick (timerId, now)
                subId, Sub.timer intervalMs (Time.now tick)
        ]

(**
In the subscription IDs, we included `timerId` and `intervalMs`. Each serves a slightly different purpose.

We can't use just "timer" because it won't be unique. `timerId` provides a unique (auto-incrementing) ID, so it satisfies that requirement.
Any data we add to the ID beyond that, like `intervalMs` will cause the subscription to restart when that data changes. This is perfect for settings that affect the subscription runtime behavior, like interval changes.

Note that "timer" isn't needed in the ID here. Since timerId is providing uniqueness, "timer" is only a prefix now.
A prefix might still be useful if there are other subscriptions on that page.

Let's finish things off with view functions.
*)

    open Feliz

    let formView resetTag userEntryTag saveTag (saveText: string) form dispatch =
        Html.div [
            Html.text " "
            Html.input [
                prop.type'.text
                prop.value form.userEntry
                prop.onChange (fun (text: string) -> dispatch (userEntryTag text))
            ]
            Html.text " "
            Html.button [
                prop.type'.button
                prop.text saveText
                match form.state with
                | Unchanged | Invalid _ ->
                    prop.disabled true
                | NewValue intervalMs ->
                    prop.onClick (fun _ -> dispatch (saveTag intervalMs))
            ]
            Html.text " "
            Html.a [
                prop.text "Reset"
                prop.style [style.userSelect.none]
                match form.state with
                | Unchanged ->
                    prop.disabled true
                    prop.style [style.color.gray; style.pointerEvents.none; style.userSelect.none]
                | _ ->
                    prop.onClick (fun _ -> dispatch resetTag)
            ]
            Html.text " "
            Html.br []
            match form.state with
            | Unchanged | NewValue _ ->
                Html.span [
                    prop.style [style.visibility.hidden]
                    prop.text "no error"
                ]
            | Invalid error ->
                Html.span [
                    prop.style [style.color.orangeRed; style.fontSize (length.rem 0.8)]
                    prop.text error
                ]
        ]

    let view model dispatch =
        Html.div [
            prop.style [style.padding (length.px 12)]
            prop.children [
                Html.style
                    """
                    table {border-collapse: collapse; border-spacing: 0; box-sizing: border-box; border: 1px solid gray}
                    table th, table td {padding: 3px 12px}
                    """
                Html.h4 "Add a timer"
                formView AddFormReset AddFormUserEntry AddTimer "Add" model.addForm dispatch
                Html.h4 "Timers"
                Html.table [
                    Html.thead [
                        Html.tr [
                            Html.th " "
                            Html.th " Timer ID "
                            Html.th " Last Tick "
                            Html.th " Interval (ms) "
                        ]
                    ]
                    Html.tbody [
                        for timerId, (now, form) in Map.toList model.timers do
                            Html.tr [
                                prop.key timerId
                                prop.children [
                                    Html.td [
                                        Html.text " "
                                        Html.button [
                                            prop.type' "button"
                                            prop.onClick (fun _ -> dispatch (RemoveTimer timerId))
                                            prop.text " X "
                                        ]
                                        Html.text " "
                                    ]
                                    Html.td [
                                        prop.style [style.textAlign.right]
                                        prop.text (" " + string timerId + " ")
                                    ]
                                    Html.td [
                                        Html.div [
                                            prop.key (string now.Ticks)
                                            let timestampStr = now.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                                            prop.text timestampStr
                                        ]
                                    ]
                                    Html.td [
                                        let reset = ChangeFormReset timerId
                                        let userEntry text = ChangeFormUserEntry (timerId, text)
                                        let save intervalMs = ChangeInterval (timerId, intervalMs)
                                        formView reset userEntry save "Save" form dispatch
                                    ]
                                ]
                            ]
                    ]
                ]
            ]
        ]

    Program.mkProgram init update view
    |> Program.withSubscription subscribe
    |> Program.run

(**
Here is a demo.

![demo of user adding updating and removing timers in a table](../static/img/timer-multi.gif)

## Migrating from v3

Migrating from Elmish v3 is fairly simple. First we will look at what we have from V3.

### v3 example
*)

// from v3 docs
module V3Sub =
    open System

    type Model =
        {
            current : DateTime
        }

    type Msg =
        | Tick of DateTime

    let init () =
        {
            current = DateTime.Now
        }

    let update msg model =
        match msg with
        | Tick current ->
            { model with
                current = current
            }

(**
Here is the main part we are concerned with: the subscription. Type annotations have been added.
*)

    open Elmish
    open Fable.Core

    let timer (initial: Model) : Cmd<Msg> =
        let sub dispatch =
            JS.setInterval
                (fun _ ->
                    dispatch (Tick DateTime.Now)
                )
                1000
                |> ignore
        Cmd.ofSub sub

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription timer
    |> Program.run


(**
### v4 conversion

First, let's see the v4-migrated `timer` function. Then we'll go through the differences.
*)

(*** hide ***)
module V4Sub =
    open V3Sub
(**
*)
    let timer (model: Model) : (SubId * Subscribe<Msg>) list =
        let sub dispatch : IDisposable =
            JS.setInterval
                (fun _ ->
                    dispatch (Tick DateTime.Now)
                )
                1000
                |> ignore
            {new IDisposable with member _.Dispose() = ()}
        [ ["timer"], sub ]

    Program.mkSimple init update (fun model _ -> printf "%A\n" model)
    |> Program.withSubscription timer
    |> Program.run

(*** hide ***)
module V4SubExplained =
    open V3Sub
(**
### Differences

First, the function signature is different.
*)
                             //vvvvvvvvvvvvvvvvvvvvvvvvvvvvv
    let timer (model: Model) : (SubId * Subscribe<Msg>) list =
(**
Instead of returning `Cmd<Msg>`, the subscription now returns a list containing `SubId`s and their associated `Subscribe` functions in a tuple.

To convert `sub` into a `Subscribe<Msg>` function, the only change is that it returns an `IDisposable` instead of `unit`.
*)
                         //vvvvvvvvvvv
        let sub dispatch : IDisposable =
            JS.setInterval
                (fun _ ->
                    dispatch (Tick DateTime.Now)
                )
                1000
                |> ignore
            {new IDisposable with member _.Dispose() = ()}
          //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
(**
The `IDisposable` is meant to provide a way to stop the subscription. We used an empty `IDisposable` because v3 subscriptions did not have "stop" functionality. So this matches v3 behavior.

> If "stop" functionality was needed in v3, it had to be manually coded. By behaving like v3, the above remains compatible with any such code.
> Then the full v4 subscription functionality can be implemented when and if it is convenient.

Now let's examine the return value.
*)
        [ ["timer"], sub ]

(**
The return value is a list containing a single element: a combination of `["timer"]` (the subscription ID) and `sub` (the subscribe function) in a tuple.
The subscription ID should be unique among other subscriptions in the list. For more info on why the ID is a list, see the section [IDs and dependencies](#ids-and-dependencies).

This is all that's necessary to migrate v3 subs to v4 to get an existing code base working.
Subscriptions in v4 offer new functionality, such as automatically stopping or restarting subscriptions when the model changes.
For more information, please consult the previous sections in this document. They offer a step-by-step guide to the new features.
*)


(**

## Style used in this guide

This guide uses a named fn, `start`, inside subscriptions for increased explicitness. The pattern looks like this:
*)

(*** hide ***)
module Style2 =

(**
*)
    let timer intervalMs onTick =
        let start dispatch = // define start fn
            ((* . . . *))
        start // return start fn

(**
In the source code of Elmish libraries, you will typically find anonymous function returns instead. Like this:
*)

(*** hide ***)
module Style1 =

(**
*)
    let timer intervalMs onTick =
        fun dispatch ->
            ((* . . . *))

(**
Both styles are equivalent in functionality. Feel free to use whichever you prefer.
*)

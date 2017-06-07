(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/bin/Debug/netstandard1.6"
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
    | Time of DateTime


(** 
Now we define the `init` function that will produce initial state once the program starts running.
It can take any arguments, but we'll just use `unit`. We'll need the [`Cmd`](cmd.html) type, so we'll open Elmish for that:
*)

let init () =
    { current = DateTime.Now }

(** 
Notice that we return a tuple. The first field of the tuple tells the program the initial state. The second field holds the command to issue an `Increment` message.

The `update` function will receive the change required by `Msg`, and the current state. It will produce a new state and potentially new command(s).

*)

let update msg model =
    match msg with
    | Time current ->
        { model with current = current }

(** 

Now lets define our timer subscription:

*)
open Elmish

let timer initial =
  Cmd.ofSub (fun dispatch -> 
    let t = new Timers.Timer 1000.
    t.Elapsed.Subscribe(fun _ -> dispatch (Time DateTime.Now)) |> ignore
    t.Enabled <- true)


Program.mkSimple init update (fun model _ -> printf "%A\n" model)
|> Program.withSubscription timer
|> Program.run


(** 
In this example program initialization will call our subscriber (once) with inital `Model` state, passing the `dispatch` function to be called whenever an event occurs.
However, any time you need to issue a message (for example from a callback) you can use `Cmd.ofSub`.
*)


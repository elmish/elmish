(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/bin/Debug/netstandard2.0"
#r "Fable.Elmish.dll"

(** Parent-child composition
---------
This is an example of nesting logic, where each child looks like an individual app.
It knows nothing about what contains it or how it's run, and that's a good thing, as it allows for great flexibility in how things are put together.

Let's define our `Counter` module to hold child logic:
*)

open Elmish

module Counter =

    type Model =
        { count : int }

    let init() =
        { count = 0 }, Cmd.none // no initial command

    type Msg =
        | Increment
        | Decrement

    let update msg model =
        match msg with
        | Increment -> 
            { model with count = model.count + 1 }, Cmd.none
        
        | Decrement -> 
            { model with count = model.count - 1 }, Cmd.none


(** 
Now we'll define types to hold two counters `top` and `bottom`, and message cases for each counter instance:
*)

type Model =
  { top : Counter.Model
    bottom : Counter.Model }

type Msg =
  | Reset
  | Top of Counter.Msg
  | Bottom of Counter.Msg

(** 
And our initialization logic, where we ask for two counters to be initialized:
*)

let init() =
    let top, topCmd = Counter.init()
    let bottom, bottomCmd = Counter.init()
    { top = top
      bottom = bottom }, 
    Cmd.batch [ Cmd.map Top topCmd
                Cmd.map Bottom bottomCmd ]

(** 
`Cmd.map` is used to "elevate" the Counter message into the container type, using corresponding `Top`/`Bottom` case constructors as the mapping function.
We batch the commands together to produce a single command for our entire container.

Note that even though we've implemented the counter as not issuing any commands, 
in a real application we still may want to map the commands to facilitate encapsulation - if at any point the child does emit some messages, we'll be in a position to handle them correctly.

And finally our update function:
*)


let update msg model : Model * Cmd<Msg> =
  match msg with
  | Reset -> 
    let top, topCmd = Counter.init()
    let bottom, bottomCmd = Counter.init()
    { top = top
      bottom = bottom }, 
    Cmd.batch [ Cmd.map Top topCmd
                Cmd.map Bottom bottomCmd ]
  | Top msg' ->
    let res, cmd = Counter.update msg' model.top
    { model with top = res }, Cmd.map Top cmd

  | Bottom msg' ->
    let res, cmd = Counter.update msg' model.bottom
    { model with bottom = res }, Cmd.map Bottom cmd


(** 
Here we see how pattern matching is used to extract counter message from `Top` and `Bottom` cases into `msg'` and it's routed to the appropriate child.
And again, we map the command issued by the child back to the container `Msg` type.

This may seem like a lot of work, but what we've done is recruited the compiler to make sure that our parent-child relationship is correctly established!
*)

(** 
And finally, we execute this as an Elmish program:

*)

Program.mkProgram init update (fun model _ -> printf "%A\n" model)
|> Program.run



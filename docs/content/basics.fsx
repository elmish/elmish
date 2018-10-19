(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/bin/Debug/netstandard2.0"

(** The basics
---------
This is a very basic example of an Elmish app - it simply prints the current state in the Console.
First, let's import our dependencies. In a real application, these imports will be in your project file and/or `paket.references`.
*)

#r "Fable.Elmish.dll"


(** 
Let's define our `Model` and `Msg` types. `Model` will hold the current state and `Msg` will tell us the nature of the change that we need to apply to the current state.
*)

type Model =
    { x : int }

type Msg = 
    | Increment
    | Decrement


(** 
Now we define the `init` function that will produce initial state once the program starts running.
It can take any arguments, but we'll just use `unit`. We'll need the [`Cmd`](cmd.html) type, so we'll open Elmish for that:
*)
open Elmish

let init () =
    { x = 0 }, Cmd.ofMsg Increment

(** 
Notice that we return a tuple. The first field of the tuple tells the program the initial state. The second field holds the command to issue an `Increment` message.

The `update` function will receive the change required by `Msg`, and the current state. It will produce a new state and potentially new command(s).

*)

let update msg model =
    match msg with
    | Increment when model.x < 3 -> 
        { model with x = model.x + 1 }, Cmd.ofMsg Increment
    
    | Increment -> 
        { model with x = model.x + 1 }, Cmd.ofMsg Decrement
    
    | Decrement when model.x > 0 -> 
        { model with x = model.x - 1 }, Cmd.ofMsg Decrement
    
    | Decrement -> 
        { model with x = model.x - 1 }, Cmd.ofMsg Increment

(** 
Again we return a tuple: new state, command. 

If we execute this as Elmish program, it will keep updating the model from 0 to 3 and back, printing the current state to the console:

*)

Program.mkProgram init update (fun model _ -> printf "%A\n" model)
|> Program.run



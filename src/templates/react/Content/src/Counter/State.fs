module State.Counter

open Elmish
open Types.Counter

let init () : Model * Cmd<Msg> =
  0, []

let update msg model =
  match msg with
  | Increment ->
      model + 1, []
  | Decrement ->
      model - 1, []
  | Reset ->
      0, []

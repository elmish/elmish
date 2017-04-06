module State.Home

open Elmish
open Types.Home

let init () : Model * Cmd<Msg> =
  "", []

let update msg model : Model * Cmd<Msg> =
  match msg with
  | ChangeStr str ->
      str, []

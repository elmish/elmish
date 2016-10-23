module Container

open Elmish

module c = Counter

// Model
type Model =
  { Top : c.Model
    Bottom : c.Model }

let init() =
  { Top = c.init()
    Bottom = c.init() }, Cmd.none

// Update
type Msg =
  | Reset
  | Top of c.Msg
  | Bottom of c.Msg

let update msg model : Model*Cmd<Msg>=
  match msg with
  | Reset -> {Top = c.init(); Bottom = c.init()}, []
  | Top cmd ->
    let res = c.update cmd model.Top
    {model with Top = res}, []
  | Bottom cmd ->
    let res = c.update cmd model.Bottom
    {model with Bottom = res}, []

// View
module R = Fable.Helpers.ReactNative

let view model (dispatch: Dispatch<Msg>) =
  R.view [Styles.sceneBackground]
    [ c.view model.Top (Top >> dispatch)
      c.view model.Bottom (Bottom >> dispatch) ]


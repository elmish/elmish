module Counter

open Fable.Elmish

// Model
type Model =
  {count:int
   visible:bool}

let init(visible : bool) =
  {count=0
   visible=visible}

// Update
type Msg =
  | Increment
  | Decrement

let update msg model =
  match msg with
  | Increment -> {model with count = model.count + 1}
  | Decrement -> {model with count = model.count - 1}

// View
module R = Fable.Helpers.ReactNative

let view model (dispatch:Dispatch<Msg>) =
  let onClick msg =
    fun _ -> msg |> dispatch
  if model.visible then
    R.view []
      [ Styles.button "-" (fun _ -> dispatch Decrement)
        R.text [] (string model.count)
        Styles.button "+" (fun _ -> dispatch Increment) ]
  else
    R.view [] []


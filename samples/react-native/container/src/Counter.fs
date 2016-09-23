module Counter

open Fable.Elmish

// Model
type Model =
  {count:int }

let init() =
  {count=0 }

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
  R.view []
    [ Styles.button "-" (fun _ -> dispatch Decrement)
      R.text [] (string model.count)
      Styles.button "+" (fun _ -> dispatch Increment) ]


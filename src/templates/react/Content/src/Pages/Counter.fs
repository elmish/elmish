namespace FableElmishReactTemplate

open Elmish
open Fable.Core

module Counter =

  type Model = int

  type Msg =
    | Increment
    | Decrement
    | Reset

  let update msg model =
    match msg with
    | Increment ->
        model + 1, Cmd.Empty
    | Decrement ->
        model - 1, Cmd.Empty
    | Reset ->
        0, Cmd.Empty

  open Fable.Helpers.React
  open Fable.Helpers.React.Props

  let simpleButton txt action dispatch =
    div
      [ ClassName "column is-narrow"
      ]
      [ a
          [ ClassName "button"
            OnClick (fun _ -> action |> dispatch)
          ]
          [ unbox txt ]
      ]

  let view model dispatch =
    div
      [ ClassName "columns is-vcentered"
      ]
      [ div [ ClassName "column" ] [ ]
        div
          [ ClassName "column is-narrow"
            Style
              [ CSSProp.Width (U2.Case2 "170px") ]
          ]
          [ unbox (sprintf "Counter value: %i" model) ]
        simpleButton "+1" Increment dispatch
        simpleButton "-1" Decrement dispatch
        simpleButton "Reset" Reset dispatch
        div [ ClassName "column" ] [ ]
      ]

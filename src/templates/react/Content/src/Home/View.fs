module Views.Home

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types.Home

let root model dispatch =
  div
    [ ]
    [ p
        [ ClassName "control" ]
        [ input
            [ ClassName "input"
              Type "text"
              Placeholder "Type your name"
              DefaultValue (U2.Case1 model)
              AutoFocus true
              OnChange (fun ev -> !!ev.target?value |> ChangeStr |> dispatch ) ] [ ] ]
      br [ ] [ ]
      span
        [ ]
        [ unbox (sprintf "Hello %s" model) ] ]

module Calc.View

open Calc.Types
open Calc.Styles


let inputToString = function 
    | Plus -> "+"
    | Minus -> "-"
    | Times -> "*"
    | Div -> "/"
    | Equals -> "="
    | Clear -> "CE"
    | Const n -> string n

let modelToString (InputStack xs) = 
    xs 
    |> Seq.map inputToString
    |> String.concat " " 

open Fable.Helpers.React
open Fable.Helpers.React.Props

let digitBtn n dispatch = 
    let message = PushInput (Const n)
    div 
      [ digitStyle;  OnClick (fun _ -> dispatch message) ]
      [ str (string n) ]

let operationBtn input dispatch = 
    let message = PushInput input
    div 
        [ opButtonStyle; OnClick (fun _ -> dispatch message) ] 
        [ str (inputToString input) ]

let tableRow xs = tr [] [ for x in xs -> td [] [x] ]

let view model dispatch =
    let digit n = digitBtn n dispatch
    let opBtn op = operationBtn op dispatch
    div
      [ calcStyle ]
      [
        h2 
          [ Style [ Height 40; MarginLeft 20 ] ]
          [ str (modelToString model) ]
        br [] []
        table 
         []
         [
             tableRow [digit 1; digit 2; digit 3; opBtn Plus]
             tableRow [digit 4; digit 5; digit 6; opBtn Minus]
             tableRow [digit 7; digit 8; digit 9; opBtn Times]
             tableRow [opBtn Input.Clear; digit 0; opBtn Equals; opBtn Div]
         ]
      ]




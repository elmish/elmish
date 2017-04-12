module Calc.Types

type Input = 
    | Const of int
    | Plus
    | Minus 
    | Times 
    | Div
    | Clear
    | Equals

type Model =  InputStack of Input list

type Messages = PushInput of Input
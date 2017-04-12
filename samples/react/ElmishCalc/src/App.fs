module Calc.App

open Calc.State
open Calc.View

open Elmish
open Elmish.React

// App
Program.mkSimple initialState update view
|> Program.withReact "elmish-app"
|> Program.run
module FifteenPuzzle.App

open FifteenPuzzle.State
open FifteenPuzzle.View

open Elmish
open Elmish.React

// App
Program.mkSimple initialState update view
|> Program.withReact "elmish-app"
|> Program.run
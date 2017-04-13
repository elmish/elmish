module MemoryGame.App

open MemoryGame.State
open MemoryGame.View

open Elmish
open Elmish.React


// App
Program.mkSimple initialModel update view
|> Program.withReact "elmish-app"
|> Program.run

module AwesomeApp

open Fable.Import
open Elmish
open Elmish.ReactNative

module C = Container

Program.mkProgram C.init C.update C.view
|> Program.withReactNative "counter"
|> Program.withConsoleTrace
|> Program.run

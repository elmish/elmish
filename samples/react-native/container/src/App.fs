module AwesomeApp

open Fable.Import
open Elmish
open Elmish.ReactNative
open Elmish.Debug

module C = Container

Program.mkProgram C.init C.update C.view
|> Program.withReactNative "counter"
|> Program.withDebugger
|> Program.run

module AwesomeApp

open Fable.Import
open Elmish
open Elmish.ReactNative

module C = Container

let runnable:obj->obj = 
    Program.mkProgram C.init C.update C.view
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run

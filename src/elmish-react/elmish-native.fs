namespace Elmish.ReactNative

open System
open Fable.Import.React
open Fable.Core

[<RequireQualifiedAccess>]
module Program =
    open Fable.Core.JsInterop
    open Elmish.React

    type Globals =
        [<Import("default","renderApplication")>] 
        static member renderApplication(rootComponent:ComponentClass<'P>, initialProps:'P, rootTag:obj) : obj = failwith "JS only"

    /// Return a function suitable for the native runnable registration
    let toRunnable run program =
        let props = toAppProps run program
        fun (appParameters:obj) -> Globals.renderApplication(unbox typeof<Components.App<_,_>>, props, appParameters?rootTag)

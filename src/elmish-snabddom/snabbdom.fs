namespace Elmish.Snabbdom

open System
open Fable.Import
open Fable.Import.Browser
open Fable.Import.Snabbdom
open Fable.Core

[<RequireQualifiedAccess>]
module Program =

    /// Setup rendering of Snabbdom vnode inside html element identified by placeholderId
    /// Use this starter if you need more modules than the default
    let withSnabbdomAndModules modules placeholderId (program:Elmish.Program<_,_,_,_>) =
        let node = Browser.document.getElementById(placeholderId) :> Browser.Element
        let patch = Snabbdom.Globals.init modules
        // Store the VNode returned by Snabbdom
        // We init it as the selected element
        let mutable viewState: U2<VNode, Browser.Element> = Case2 node
        let mutable lastRequest = None

        let setState model dispatch =
            match lastRequest with
            | Some r -> window.cancelAnimationFrame r
            | None -> ()

            lastRequest <- Some (window.requestAnimationFrame (fun _ ->
                let vnode = program.view model dispatch
                viewState <- patch.Invoke(viewState, vnode) |> Case1
            ))

        { program with setState = setState }

    /// Setup rendering of Snabbdom vnode inside html element identified by placeholderId
    /// This starter start Snabbdom with basics modules (Class, EventListeners, Props, Style)
    let withSnabbdom placeholderId (program:Elmish.Program<_,_,_,_>) =
        withSnabbdomAndModules
            [|
                Snabbdom.Modules.Attributes
                Snabbdom.Modules.Class
                Snabbdom.Modules.EventListeners
                Snabbdom.Modules.Props
                Snabbdom.Modules.Style
            |]
            placeholderId
            program

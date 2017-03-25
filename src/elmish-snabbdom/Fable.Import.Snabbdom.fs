namespace Fable.Import

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.JS
open Fable.Import.Browser

module Snabbdom =

    type Key = U2<string, float>

    and VNode =
        abstract sel: string option with get, set
        abstract data: obj with get, set
        abstract children: U2<VNode, string> array with get, set
        abstract elm: Node option with get, set
        abstract text: string option with get, set
        abstract key: Key with get, set

    and [<Erase>] Module = Module

    and Modules =
        [<Import("default","snabbdom/modules/class")>] static member Class : Module = jsNative
        [<Import("default","snabbdom/modules/props")>] static member Props : Module = jsNative
        [<Import("default","snabbdom/modules/style")>] static member Style : Module = jsNative
        [<Import("default","snabbdom/modules/eventlisteners")>] static member EventListeners : Module = jsNative
        [<Import("default","snabbdom/modules/attributes")>] static member Attributes : Module = jsNative
        [<Import("default","snabbdom/modules/hero")>] static member Hero : Module = jsNative
        [<Import("default","snabbdom/modules/dataset")>] static member Dataset : Module = jsNative

    and [<Import("*","snabbdom/snabbdom")>] Globals =
        static member init(modules: Module array): Func<U2<VNode, Element>, VNode, VNode> = jsNative

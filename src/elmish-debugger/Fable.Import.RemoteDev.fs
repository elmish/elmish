namespace Fable.Import

open System
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

[<Erase>]
[<AutoOpen>]
module RemoteDev =
    type Options = { remote:bool; port:int; hostname:string; secure:bool }

    type Msg = { payload:obj; action:obj; ``type``:string }

    type Listener<'msg> = 'msg -> unit
    
    type Unsubscribe = unit -> unit

    type Connection =
        abstract init: obj * obj Option -> unit
        abstract subscribe: Listener<Msg> -> Unsubscribe
        abstract unsubscribe: Unsubscribe
        abstract send: obj * obj -> unit
        abstract error: obj -> unit

    [<Import("connect","remotedev")>]
    let connect : Options option -> Connection = jsNative

    [<Import("connectViaExtension","remotedev")>]
    let connectViaExtension : Options option -> Connection = jsNative
    
    [<Import("extractState","remotedev")>]
    let extractState<'state> : Msg -> 'state = jsNative

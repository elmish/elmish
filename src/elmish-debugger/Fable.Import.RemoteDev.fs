namespace Fable.Import

open System
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

module RemoteDev =
    type Options = { remote:bool; port:int; hostname:string; secure:bool; getActionType:(obj->obj) option  }
    
    module MsgTypes =
        [<Literal>]
        let Start = "START"
        [<Literal>]
        let Action = "ACTION"
        [<Literal>]
        let Dispatch = "DISPATCH"

    type Msg = { state:string; action:obj; ``type``:string}

    type Listener = Msg -> unit
    
    type Unsubscribe = unit -> unit

    type Connection =
        abstract init: obj * obj Option -> unit
        abstract subscribe: Listener -> Unsubscribe
        abstract unsubscribe: Unsubscribe
        abstract send: obj * obj -> unit
        abstract error: obj -> unit

    [<Import("connect","remotedev")>]
    let connect : Options -> Connection = jsNative

    [<Import("connectViaExtension","remotedev")>]
    let connectViaExtension : Options -> Connection = jsNative
    
    [<Import("extractState","remotedev")>]
    let extractState : obj -> obj = jsNative

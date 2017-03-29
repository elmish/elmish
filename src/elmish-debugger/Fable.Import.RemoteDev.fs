namespace Fable.Import

open System
open Fable.Core
open Fable.Import.JS
open Fable.Import.Browser

module RemoteDev =
    module MsgTypes =
        [<Literal>]
        let Start = "START"
        [<Literal>]
        let Action = "ACTION"
        [<Literal>]
        let Dispatch = "DISPATCH"

    module PayloadTypes =
        [<Literal>]
        let ImportState = "IMPORT_STATE"
        [<Literal>]
        let JumpToState = "JUMP_TO_STATE"
        [<Literal>]
        let JumpToAction = "JUMP_TO_ACTION"

    type Options =
        { remote : bool
          port : int
          hostname : string
          secure : bool
          getActionType : (obj->obj) option
          serialize : obj }
    
    type Action = 
        { ``type``: string
          fields : obj array }

    type LiftedState =
        { actionsById : Action array
          computedStates : obj array
          currentStateIndex : int
          nextActionId : int }
    
    type Payload =
        { nextLiftedState : LiftedState
          ``type``: string }

    type Msg =
        { state : string
          action : obj
          ``type`` : string
          payload : Payload }

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

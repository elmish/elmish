module CounterWs.Messages

open Fable.Core.JsInterop

type WsMessage =
    | Incr of ms:int
    | Decr of ms:int
    | Stop
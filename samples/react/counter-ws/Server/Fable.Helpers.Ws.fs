module Fable.Helpers.Ws
    
open Fable.Core
open Fable.Import.JS
open Fable.Import.Ws
open Fable.Core.JsInterop
open Fable.Import.Node

    type [<KeyValueList>] ClientOptions =
        | Protocol of string
        | Agent of http_types.Agent
        | Headers of obj
        | ProtocolVersion of obj
        | Host of string
        | Origin of string
        | Pfx of obj
        | Key of obj
        | Passphrase of string
        | Cert of obj
        | Ca of ResizeArray<obj>
        | Ciphers of string
        | RejectUnauthorized of bool
        interface IClientOptions

    and [<KeyValueList>] PerMessageDeflateOptions =
        | ServerNoContextTakeover of bool
        | ClientNoContextTakeover of bool
        | ServerMaxWindowBits of float
        | ClientMaxWindowBits of float
        | MemLevel of float
        interface IPerMessageDeflateOptions

    and [<KeyValueList>] ServerOptions =
        | Host of string
        | Port of float
        | Server of U2<http_types.Server, https_types.Server>
        | VerifyClient of U2<VerifyClientCallbackAsync, VerifyClientCallbackSync>
        | HandleProtocols of obj
        | Path of string
        | NoServer of bool
        | DisableHixie of bool
        | ClientTracking of bool
        | PerMessageDeflate of U2<bool, IPerMessageDeflateOptions>
        interface IServerOptions

[<Import("*","ws")>]
let ws: WebSocket = jsNative


[<Import("*","ws")>]
let Server: Globals = jsNative
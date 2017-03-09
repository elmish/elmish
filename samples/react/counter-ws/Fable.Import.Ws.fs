namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Import.Node

module Ws =
    type [<AllowNullLiteral>] [<Import("*","WebSocket")>] WebSocket = //(address: string, ?protocols: U2<string, ResizeArray<string>>, ?options: IClientOptions) =
        inherit NodeJS.EventEmitter
        abstract CONNECTING: int with get
        abstract OPEN: int with get
        abstract CLOSING: int with get
        abstract CLOSED: int with get
        abstract bytesReceived: int with get
        abstract readyState: int with get
        abstract protocolVersion: string with get
        abstract url: string with get
//        abstract supports: obj with get
        abstract upgradeReq: http_types.IncomingMessage with get
        abstract protocol: string with get
        abstract onopen: obj -> unit
        abstract onerror: Error -> Unit
        abstract onclose: obj -> unit
        abstract onmessage: obj -> unit
        abstract close: ?code: float * ?data: obj -> unit
        abstract pause: unit -> unit
        abstract resume: unit -> unit
        abstract ping: ?data: obj * ?options: obj * ?dontFail: bool -> unit
        abstract pong: ?data: obj * ?options: obj * ?dontFail: bool -> unit
        abstract send: data: obj * ?cb: (Error -> unit) -> unit
        abstract send: data: obj * options: obj * ?cb: (Error -> unit) -> unit
        abstract stream: options: obj * ?cb: (Error -> bool -> unit) -> unit
        abstract stream: ?cb:  (Error -> bool -> unit) -> unit
        abstract terminate: unit -> unit
        [<Emit("$0.addEventListener('message',$1...)")>] abstract addEventListener_message: cb: (obj -> unit) -> unit
        [<Emit("$0.addEventListener('close',$1...)")>] abstract addEventListener_close: cb: (obj -> unit) -> unit
        [<Emit("$0.addEventListener('error',$1...)")>] abstract addEventListener_error: cb: (Error -> unit) -> unit
        [<Emit("$0.addEventListener('open',$1...)")>] abstract addEventListener_open: cb: (obj -> unit) -> unit
        abstract addEventListener: ``event``: string -> listener: (obj -> unit) -> unit
        [<Emit("$0.on('error',$1...)")>] abstract on_error: cb: (Error -> unit) -> unit
        [<Emit("$0.on('close',$1...)")>] abstract on_close: cb: (obj -> unit) -> unit
        [<Emit("$0.on('message',$1...)")>] abstract on_message: cb: (obj -> unit) -> unit
        [<Emit("$0.on('ping',$1...)")>] abstract on_ping: cb: (obj -> bool -> bool -> unit) -> unit
        [<Emit("$0.on('pong',$1...)")>] abstract on_pong: cb: (obj -> bool -> bool -> unit) -> unit
        [<Emit("$0.on('open',$1...)")>] abstract on_open: cb: (obj -> unit) -> unit
        abstract on: ``event``: string * listener: (obj -> unit) -> unit
//        [<Emit("$0.addListener('error',$1...)")>] abstract addListener_error(cb: Func<Error, unit>): obj = failwith "JS only"
//        [<Emit("$0.addListener('close',$1...)")>] abstract addListener_close(cb: Func<float, string, unit>): obj = failwith "JS only"
//        [<Emit("$0.addListener('message',$1...)")>] abstract addListener_message(cb: Func<obj, obj, unit>): obj = failwith "JS only"
//        [<Emit("$0.addListener('ping',$1...)")>] abstract addListener_ping(cb: Func<obj, obj, unit>): obj = failwith "JS only"
//        [<Emit("$0.addListener('pong',$1...)")>] abstract addListener_pong(cb: Func<obj, obj, unit>): obj = failwith "JS only"
//        [<Emit("$0.addListener('open',$1...)")>] abstract addListener_open(cb: Func<unit>): obj = failwith "JS only"
//        abstract addListener(``event``: string, listener: Func<unit>): obj = failwith "JS only"


    and VerifyClientCallbackSync = Func<obj, bool>

    and VerifyClientCallbackAsync = Func<obj, Func<bool, unit>, unit>

    and IClientOptions = interface end
    and IPerMessageDeflateOptions = interface end
    and IServerOptions = interface end

//    and [<AllowNullLiteral>] IClientOptions =
//        abstract protocol: string option with get, set
//        abstract agent: http_types.Agent option with get, set
//        abstract headers: obj option with get, set
//        abstract protocolVersion: obj option with get, set
//        abstract host: string option with get, set
//        abstract origin: string option with get, set
//        abstract pfx: obj option with get, set
//        abstract key: obj option with get, set
//        abstract passphrase: string option with get, set
//        abstract cert: obj option with get, set
//        abstract ca: ResizeArray<obj> option with get, set
//        abstract ciphers: string option with get, set
//        abstract rejectUnauthorized: bool option with get, set
//
//    and [<AllowNullLiteral>] IPerMessageDeflateOptions =
//        abstract serverNoContextTakeover: bool option with get, set
//        abstract clientNoContextTakeover: bool option with get, set
//        abstract serverMaxWindowBits: float option with get, set
//        abstract clientMaxWindowBits: float option with get, set
//        abstract memLevel: float option with get, set
//
//    and [<AllowNullLiteral>] IServerOptions =
//        abstract host: string option with get, set
//        abstract port: float option with get, set
//        abstract server: U2<http_types.Server, https_types.Server> option with get, set
//        abstract verifyClient: U2<VerifyClientCallbackAsync, VerifyClientCallbackSync> option with get, set
//        abstract handleProtocols: obj option with get, set
//        abstract path: string option with get, set
//        abstract noServer: bool option with get, set
//        abstract disableHixie: bool option with get, set
//        abstract clientTracking: bool option with get, set
//        abstract perMessageDeflate: U2<bool, IPerMessageDeflateOptions> option with get, set

    and [<AllowNullLiteral>] [<Import("Server","WebSocket")>] Server =
        inherit NodeJS.EventEmitter
        abstract options: IServerOptions with get,set //: IServerOptions = failwith "JS only" and set(v: IServerOptions): unit = failwith "JS only"
        abstract path: string with get,set //: string = failwith "JS only" and set(v: string): unit = failwith "JS only"
        abstract clients: ResizeArray<WebSocket> with get,set //: ResizeArray<WebSocket> = failwith "JS only" and set(v: ResizeArray<WebSocket>): unit = failwith "JS only"
        abstract close: callback:obj -> unit //(?cb: Func<obj, unit>): unit = failwith "JS only"
        abstract handleUpgrade: request: http_types.IncomingMessage * socket: net_types.Socket * upgradeHead: Buffer * callback: (WebSocket -> unit) -> unit //: unit = failwith "JS only"
        [<Emit("$0.on('error',$1...)")>] abstract on_error: (Error -> unit) -> unit
        [<Emit("$0.on('headers',$1...)")>] abstract on_headers: (ResizeArray<string> -> unit) -> unit
        [<Emit("$0.on('connection',$1...)")>] abstract on_connection: (WebSocket -> unit) -> unit
        abstract on: ``event``: string * listener: (obj -> unit) -> unit
//        [<Emit("$0.addListener('error',$1...)")>] abstract addListener_error: Error -> unit
//        [<Emit("$0.addListener('headers',$1...)")>] abstract addListener_headers: ResizeArray<string> -> unit
//        [<Emit("$0.addListener('connection',$1...)")>] abstract addListener_connection: WebSocket -> unit
//        abstract addListener: string -> obj -> unit

    and Globals =
        [<Emit("new $0.Server($1)")>]
        abstract createServer: options: IServerOptions list -> Server
        [<Emit("new $0.Server($1...)")>]
        abstract createServer: options: IServerOptions list * ?connectionListener: (WebSocket -> unit) -> Server
//        abstract connect(address: string, ?openListener: Function): unit = failwith "JS only"
//        abstract createConnection(address: string, ?openListener: Function): unit = failwith "JS only"



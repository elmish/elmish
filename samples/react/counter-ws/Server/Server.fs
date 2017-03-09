module CounterWs.Server

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Fable.Helpers.Ws
open Fable.Core.JsInterop
open Fable.Import.Node
open Messages
open Fable.Import.JS

// Helper types for running an express server

type ExpressApp =
    abstract ``use``: string -> unit

type Express =
    [<Emit("$0()")>]
    abstract create: unit -> ExpressApp
    abstract ``static``: string -> string

type HttpServer =
    inherit http_types.Server
    abstract on: string * obj -> unit

type HttpServerFactory =
    abstract createServer: unit -> HttpServer

//type Util =
//    abstract toString: obj -> string
//let util = importDefault<Util>("fable-core/umd/Util")

// Create Express server
let serv = importDefault<HttpServerFactory>("http").createServer()
let express = importDefault<Express>("express")
let app = express.create()

let path = __dirname + "/../../"

console.log(sprintf "Server content from %s" path)

app.``use``(express.``static``(path))

// Create Websocket server
let opts = [ ServerOptions.Server <| unbox serv ]
let wsServer = Server.createServer(unbox opts)

wsServer.on_connection(fun ws ->
    //ws.send something    
    console.log("Client connected")

    let mutable ct = new System.Threading.CancellationTokenSource()
    let rec repeatSend msg s = 
        async {
            do! Async.Sleep s
            msg |> JSON.stringify |> ws.send
            return! repeatSend msg s
        }

    let start msg s = 
        ct.Cancel()
        ct <- new System.Threading.CancellationTokenSource()
        Async.StartImmediate(repeatSend msg s, ct.Token)

    ws.on_message <| fun msg -> 
        console.log("got msg")
        console.log(msg)
        let msg' = JSON.parse (string msg) :?> WsMessage
        match msg' with
        | Incr ms -> console.log("Incrementing every " + (unbox ms)); start DoIncr ms
        | Decr ms -> console.log("Decrementing every " + (unbox ms)); start DoDecr ms
        | Stop -> console.log("Stopping"); ct.Cancel()
        | _ -> ()


    ws.on_close(fun _ ->
        console.log("Client disconnected")
    )
)

serv.on("request", app)
serv.listen(8080, unbox <| fun () -> console.log("Listening on http://localhost:8080")) |> ignore

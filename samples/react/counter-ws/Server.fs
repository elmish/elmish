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

type Util =
    abstract toString: obj -> string
let util = importDefault<Util>("fable-core/umd/Util")

// Create Express server
let serv = importDefault<HttpServerFactory>("http").createServer()
let express = importDefault<Express>("express")
let app = express.create()
app.``use``(express.``static``(__dirname + "/../"))

// Create Websocket server
let opts = [ ServerOptions.Server <| unbox serv ]
let wsServer = Server.createServer(unbox opts)

wsServer.on_connection(fun ws ->
    //ws.send something    
    console.log("Client connected")

//    let ct = new System.Threading.CancellationTokenSource()
//    let rec repeatSend msg s = 
////        let ct = new System.Threading.CancellationTokenSource()
//        async {
//            do! Async.Sleep s
//            ws.send msg
//            return! repeatSend msg s
//        }
//
//    let start msg s = 
//        Async.Start(repeatSend msg s, ct.Token)

    ws.on_message <| fun msg -> 
        console.log("got msg")
        console.log(msg)
        let msg' = JSON.parse (string msg) :?> WsMessage
        match msg' with
        | Incr ms -> console.log("Incrementing every " + (unbox ms))//; start DoIncr ms
        | Decr ms -> console.log("Decrementing every " + (unbox ms))//; start DoDecr ms
        | Stop -> console.log("Stopping")//; ct.Cancel()
        | _ -> ()


    ws.on_close(fun _ ->
        console.log("Client disconnected")
    )
)


serv.on("request", app)
serv.listen(8080, unbox <| fun () -> console.log("Listening on http://localhost:8080")) |> ignore

//var WebSocketServer = require('ws').Server;
//var express = require('express');
//var path = require('path');
//var app = express();
//var server = require('http').createServer();
//
//app.use(express.static(__dirname));
//
//var wss = new WebSocketServer({server: server});
//wss.on('connection', function (ws) {
//  var id = setInterval(function () {
//    ws.send(JSON.stringify(process.memoryUsage()), function () { /* ignore errors */ });
//  }, 100);
//  console.log('started client interval');
//  ws.on('close', function () {
//    console.log('stopping client interval');
//    clearInterval(id);
//  });
//});
//
//server.on('request', app);
//server.listen(8080, function () {
//  console.log('Listening on http://localhost:8080');
//});

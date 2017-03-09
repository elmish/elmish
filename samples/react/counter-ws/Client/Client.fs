module CounterWs.Client

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Messages
open Fable.Core.JsInterop
open Fable.Import.JS

// MODEL

type Msg =
    | Increment
    | Decrement
    | Send of WsMessage

let ws = WebSocket.Create("ws://localhost:8080")
console.log(ws.readyState);

let onMessage dispatch =
    fun (msg: MessageEvent) ->
        let msg' = msg.data |> string |> JSON.parse :?> WsMessage
        match msg' with
        | DoIncr -> dispatch Increment
        | DoDecr -> dispatch Decrement
        | _ -> console.log(sprintf "Not handling unknown message: %s" (string msg.data))
        unbox None

let subscription dispatch =
    ws.onmessage <- unbox (onMessage dispatch)

let subscribe model = Cmd.ofSub subscription

ws.onopen <- fun _ ->
    console.log("ws open")

//    ws.onmessage <- fun msg ->
//        console.log("Got msg")  
//        console.log(msg.data)
//        unbox None

    unbox None

let send msg =
    let m = JSON.stringify msg
//    console.log("Sending msg")
//    console.log(m)
    ws.send m

let init () = 0

// UPDATE

let update (msg:Msg) count =
    match msg with
    | Increment -> count + 1
    | Decrement -> count - 1
    | Send m -> send m; count

// rendering views with React
module R = Fable.Helpers.React
open Fable.Helpers.React.Props

let view count dispatch =
    let onClick msg =
        OnClick <| fun _ -> msg |> dispatch
    R.div [] [
        R.button [ onClick Decrement ] [ unbox "-" ]
        R.div [] [ unbox (string count) ]
        R.button [ onClick Increment ] [ unbox "+" ] 
        R.div [ Style [ Display "flex"; FlexDirection "row"; MarginTop 40 ]] [
            R.button [ onClick <| Send (Incr 1000) ] [ unbox "Auto +" ]
            R.button [ onClick <| Send (Decr 1000) ] [ unbox "Auto -" ]
            R.button [ onClick <| Send Stop ] [ unbox "Stop it!" ]
        ] 
    ]

open Elmish.React

// App
Program.mkSimple init update view
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.withSubscription subscribe
|> Program.run
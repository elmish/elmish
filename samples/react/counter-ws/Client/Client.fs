module CounterWs.Client

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Messages
open Fable.Core.JsInterop
open Fable.Import.JS

// MODEL

type Model =
    { count: int
      connected: bool }

type Msg =
    | Noop of unit
    | Error of exn
    | Increment
    | Decrement
    | Send of WsMessage
    | Connected of bool

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

let onOpen dispatch = fun _ -> Connected true |> dispatch
let onClose dispatch = fun _ -> Connected false |> dispatch

let subscription dispatch =
    ws.onmessage <- unbox (onMessage dispatch)
    ws.onopen <- unbox (onOpen dispatch)
    ws.onclose <- unbox (onClose dispatch)

let subscribe model = Cmd.ofSub subscription

ws.onopen <- fun _ ->
    console.log("ws open")

    unbox None

let send msg =
    let m = JSON.stringify msg
    ws.send m

let init () = { count = 0; connected = false }, []

// UPDATE

let update (msg:Msg) model =
    match msg with
    | Noop _ -> model,[]
    | Increment -> { model with count = model.count + 1 },[]
    | Decrement -> { model with count = model.count - 1 },[]
    | Send m -> model, Cmd.ofFunc send m Noop Error
    | Error ex ->
        console.error("Error: ", ex)
        model, []
    | Connected c -> { model with connected = c },[]

// rendering views with React
module R = Fable.Helpers.React
open Fable.Helpers.React.Props

let view model dispatch =
    let onClick msg =
        OnClick <| fun _ -> msg |> dispatch
    let disable = not model.connected
    R.div [] [
        R.button [ onClick Decrement ] [ unbox "-" ]
        R.div [] [ unbox (string model.count) ]
        R.button [ onClick Increment ] [ unbox "+" ] 
        R.div [ Style [ MarginTop 40 ]] [
            R.div [] [ unbox <| if model.connected then "Server, count for me:" else "Server disconnected :(" ]
            R.div [ Style [ Display "flex"; FlexDirection "row" ]] [
                R.button [ Disabled disable; onClick <| Send (Incr 1000) ] [ unbox "Auto +" ]
                R.button [ Disabled disable; onClick <| Send (Decr 1000) ] [ unbox "Auto -" ]
                R.button [ Disabled disable; onClick <| Send Stop ] [ unbox "Stop it!" ]
            ]         
        ]
    ]

open Elmish.React

// App
Program.mkProgram init update view
|> Program.withSubscription subscribe
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.run
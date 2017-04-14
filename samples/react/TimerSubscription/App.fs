module App

open System
open Fable.Import.Browser
open Fable.Core.JsInterop

// Types
type Model = CurrentTime of DateTime
type Messages = Tick of DateTime

open Elmish
open Elmish.React

// state
let initialState() = CurrentTime DateTime.Now, Cmd.none

let update (Tick next) (CurrentTime time) = CurrentTime next, Cmd.none
    

/// This is a function that can call disptach and thus
/// send a message to the program loop at will
/// in this case, it will disptach message every second
let timerSubscription dispatch = 
    window.setInterval(fun () ->
        let message = Tick DateTime.Now
        dispatch message
    , 1000) |> ignore

/// create a command from subscription
let tickCommand _ = Cmd.ofSub timerSubscription


// View
open Fable.Import.React
open Fable.Helpers.React
open Fable.Helpers.React.Props

let view (CurrentTime time) dispatch = 
    h1 [ ] 
       [ str (sprintf "%d:%d:%d" time.Hour time.Minute time.Second) ]



// App
Program.mkProgram initialState update view
|> Program.withReact "elmish-app"
|> Program.withSubscription tickCommand
|> Program.run

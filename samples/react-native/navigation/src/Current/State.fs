module State.Current

open Types.Current 
open Fable.Core
open Elmish

let init () : State * Cmd<Msg> = 
    { value = "" }, []

let timerTick dispatch =
    let timer = new System.Timers.Timer 1000.
    timer.Elapsed.Subscribe (fun _ -> dispatch (System.DateTime.Now |> (string >> Current))) |> ignore
    timer.Enabled <- true

let subscribe model =
    Cmd.ofSub timerTick

let update msg model : State * Cmd<Msg> =
    match msg with
    | Current str -> { model with value = str }, Cmd.none
 
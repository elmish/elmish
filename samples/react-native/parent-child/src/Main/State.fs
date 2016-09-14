module MainState

open MainTypes 
open Fable.Import.Fetch
open Fable.Core
open Fable.Extras
open Fable.Elmish

let getTime () = 
    promise {
        let! r = GlobalFetch.fetch "http://www.timeapi.org/utc/now"
        return! r.text()
    }

let init () : MainState * Cmd<MainMsg> = 
    { retrieved = ""; current = "" }, Cmd.ofMsg GetTime

let timerTick dispatch =
    let timer = new System.Timers.Timer 1000.
    timer.Elapsed.Subscribe (fun _ -> dispatch (System.DateTime.Now |> (string >> Current))) |> ignore
    timer.Enabled <- true

let subscribe model =
    Cmd.ofSub timerTick

let update msg model : MainState * Cmd<MainMsg> =
    match msg with
    | GetTime -> model, (Cmd.ofPromise getTime Retrieved (string >> Error))
    | Error str
    | Retrieved str -> { model with retrieved = str }, Cmd.none
    | Current str -> { model with current = str }, Cmd.none
 
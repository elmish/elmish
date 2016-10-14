module State.Retrieved

open Types.Retrieved 
open Fable.Import.Fetch
open Fable.Core
open Elmish

let getTime () = 
    promise {
        let! r = GlobalFetch.fetch "http://www.timeapi.org/utc/now"
        return! r.text()
    }

let init () : State * Cmd<Msg> = 
    { value = "" }, Cmd.ofMsg GetTime

let update msg model : State * Cmd<Msg> =
    match msg with
    | GetTime -> model, (Cmd.ofPromise getTime () Retrieved (string >> Error))
    | Error str
    | Retrieved str -> { model with value = str }, Cmd.none
 
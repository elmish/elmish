module State.Retrieved

open Fable.Core
open Fable.PowerPack
open Elmish
open Types.Retrieved 

let getTime () = 
    promise {
        let! r = Fetch.fetch "http://www.timeapi.org/utc/now" []
        return! r.text()
    }

let init () : State * Cmd<Msg> = 
    { value = "" }, Cmd.ofMsg GetTime

let update msg model : State * Cmd<Msg> =
    match msg with
    | GetTime -> model, (Cmd.ofPromise getTime () Retrieved (string >> Error))
    | Error str
    | Retrieved str -> { model with value = str }, Cmd.none
 
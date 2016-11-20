/// Elmish Cmd extension for promises
[<RequireQualifiedAccess>]
module Elmish.Cmd

open Fable.PowerPack

/// Command to call `promise` block and map the results
let ofPromise (task:'a->Fable.Import.JS.Promise<_>) (arg:'a) (ofSuccess:_->'msg) (ofError:_->'msg) : Cmd<'msg> =
    let bind (dispatch:'msg -> unit) =
        task arg
        |> Promise.map (ofSuccess >> dispatch)
        |> Promise.catch (ofError >> dispatch)
        |> ignore
    [bind]

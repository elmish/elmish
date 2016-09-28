module AwesomeState

open AwesomeTypes
open Fable.Core
open Elmish

let init ():Model*Cmd<Msg> =
    let (mainProps, mainCmd) = MainState.init()
    { MainState = mainProps }, Cmd.map MainMsg mainCmd 

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | MainMsg msg -> 
        let (mainProps, mainCmd) = MainState.update msg model.MainState
        { MainState = mainProps }, Cmd.batch [Cmd.none; Cmd.map MainMsg mainCmd]
    | _ -> (model,Cmd.none)

let subscribe (model:Model) =
    MainState.subscribe model.MainState |> Cmd.map MainMsg
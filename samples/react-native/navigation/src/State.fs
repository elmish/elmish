module State.App

open Types
open Types.App
open Fable.Core
open Elmish
open Fable.Helpers.ReactNative

let init ():Model*Cmd<Msg> =
    let (main, mainCmd) = Main.init()
    let (current, currentCmd) = Current.init()
    let (retrieved, retrievedCmd) = Retrieved.init()
    let nav = navigationState 1 [ navigationRoute Routes.Current (Some "Current")
                                  navigationRoute Routes.Main (Some "")
                                  navigationRoute Routes.Retrieved (Some "Retrived")]
    { MainState = main 
      CurrentState = current 
      RetrievedState = retrieved
      NavigationState = nav }, 
    Cmd.batch [Cmd.map MainMsg mainCmd 
               Cmd.map CurrentMsg currentCmd 
               Cmd.map RetrievedMsg retrievedCmd] 

let update (msg:Msg) (model:Model) : Model*Cmd<Msg> =
    match msg with
    | MainMsg msg -> 
        let (state, cmd) = Main.update msg model.MainState
        { model with MainState = state }, Cmd.map MainMsg cmd
    | CurrentMsg msg -> 
        let (state, cmd) = Current.update msg model.CurrentState
        { model with CurrentState = state }, Cmd.map CurrentMsg cmd
    | RetrievedMsg msg -> 
        let (state, cmd) = Retrieved.update msg model.RetrievedState
        { model with RetrievedState = state }, Cmd.map RetrievedMsg cmd
    | NavigationMsg msg -> 
        let state = match msg with
                    | JumpTo index ->
                        let index = if index < 0 then 2 else index
                        let index = if index > 2 then 0 else index
                        RN.NavigationExperimental.StateUtils.jumpToIndex(model.NavigationState,index)
                    | _ -> model.NavigationState
        { model with NavigationState = state }, []
    | _ -> (model,Cmd.none)

let subscribe (model:Model) =
    Current.subscribe model.CurrentState |> Cmd.map CurrentMsg

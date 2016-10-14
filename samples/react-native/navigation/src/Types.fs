module Types.App

open Fable.Import.ReactNative

type Model = {
    MainState : Main.State
    CurrentState : Current.State
    RetrievedState : Retrieved.State
    NavigationState : NavigationState
}

type Route = string

type Navigate =
    | Push of key:Route * title:string option
    | Pop of unit
    | JumpTo of int

type Msg = 
    | MainMsg of Main.Msg
    | CurrentMsg of Current.Msg
    | RetrievedMsg of Retrieved.Msg
    | NavigationMsg of Navigate
    | Noop

module Routes =
    [<Literal>] 
    let Main = "main"
    [<Literal>] 
    let Current = "current"
    [<Literal>] 
    let Retrieved = "retrieved"
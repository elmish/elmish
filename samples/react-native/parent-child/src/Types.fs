module AwesomeTypes

open MainTypes

type Model = {
    MainState : MainState
}

type Msg = 
    | MainMsg of MainMsg
    | Noop
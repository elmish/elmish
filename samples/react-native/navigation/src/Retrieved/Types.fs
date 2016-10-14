module Types.Retrieved

type State = { value : string}

type Msg =
    | Retrieved of string
    | Error of string
    | GetTime
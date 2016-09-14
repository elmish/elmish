module MainTypes

type MainState = {
    retrieved : string
    current : string
}

type MainMsg =
    | Current of string
    | Retrieved of string
    | Error of string
    | GetTime
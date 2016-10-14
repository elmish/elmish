module Types.Current

type State = { value : string }

type Msg =
    | Current of string

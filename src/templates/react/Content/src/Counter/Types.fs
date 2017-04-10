module Types.Counter

type Model = int

type Msg =
  | Increment
  | Decrement
  | Reset

module FifteenPuzzle.Types

type Position = { X: int; Y: int }

type Slot = Position * string

type AppState = { Slots : Slot list;  FreePos : Position }

type Messages = 
    | StartNewGame
    | SelectSlot of Slot
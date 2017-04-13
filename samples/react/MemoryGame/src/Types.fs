module MemoryGame.Types

type Card = {
    Id : int
    ImgUrl : string
    Selected : bool
    MatchFound : bool
}

type Model = {
    Cards : Card list
    FirstSelection : int option
    SecondSelection : int option
}

type Actions =
    | SelectCard of int 
    | StartNewGame
    | NoOp
module MemoryGame.State

open MemoryGame.Types
open System
let random = new Random();

let getCards() = 
    let images = [ "violin"; "electric-guitar"; "headphones"; "piano"; "saxophone";  "trumpet";"turntable";"bass-guitar" ]
    images 
    |> List.append images
    |> List.sortBy (fun img -> random.Next())
    |> List.map (sprintf "images/%s.png")
    |> List.mapi (fun index img -> { Id = index; ImgUrl = img; Selected = false; MatchFound = false})

    
    
let initialModel() = {
    Cards = getCards()
    FirstSelection = None
    SecondSelection = None
}


let cardsEqual id1 id2 (cards: Card list) =
    let card1 = cards |> List.find (fun c -> c.Id = id1)
    let card2 = cards |> List.find (fun c -> c.Id = id2)
    card1.ImgUrl = card2.ImgUrl


let cardSelected id (cards: Card list) = 
    let card = List.find (fun c -> c.Id = id) cards
    card.Selected

let gameCleared (model: Model) =  
    List.forall (fun card -> card.MatchFound) model.Cards

let rec update action model  =
    match action with
    | StartNewGame -> initialModel()
    | SelectCard index -> 
        match model.FirstSelection, model.SecondSelection with
        | None, None -> 
            let cards = 
                model.Cards 
                |> List.map (fun card ->
                     if card.Id = index 
                     then { card with Selected = true }
                     else card
                )
            { model with Cards = cards; FirstSelection = Some index }
        | Some id, None when id = index -> model
        | Some id, None when cardsEqual id index (model.Cards) ->
            let cards =
                model.Cards              
                |> List.map (fun card ->
                     if card.Id = index || card.Id = id
                     then { card with Selected = true; MatchFound = true }
                     else card
                )
            { model with Cards = cards; FirstSelection = None; SecondSelection = None } 
        | Some id, None when id <> index ->  
            let cards =
                 model.Cards   
                 |> List.map (fun card ->
                      if card.Id = index || card.Id = id
                      then { card with Selected = true }
                      else card
                 )
            { model with Cards = cards; SecondSelection = Some index } 
        | Some id, Some id' when cardsEqual id' index (model.Cards) -> 
            let cards = 
                model.Cards 
                 |> List.map (fun card ->
                      if (card.Id = id && not card.MatchFound) 
                      then { card with Selected = false }
                      elif (card.Id = id' || card.Id = index) 
                      then { card with Selected = true; MatchFound = true }
                      else card
                 )
            let model' = { model with Cards = cards; FirstSelection = None; SecondSelection = None }
            update action model'
        | Some id, Some id' -> 
            let cards = 
                model.Cards 
                 |> List.map (fun card ->
                      if (card.Id = id || card.Id = id') && not card.MatchFound
                      then { card with Selected = false }
                      elif card.Id = index 
                      then { card with Selected = true }
                      else card
                 )
            let model' = { model with Cards = cards; FirstSelection = None; SecondSelection = None }
            update action model' 
        | _, _ -> failwith "Cannot happen :)"
    | NoOp -> model




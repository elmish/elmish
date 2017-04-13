module MemoryGame.View

open MemoryGame.Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

let cardClicked (card: Card) dispatch  = 
   if not (card.MatchFound) && not (card.Selected)
   then dispatch (SelectCard card.Id)
   else dispatch (NoOp)

let classList xs =  
    xs
    |> List.filter snd
    |> List.map fst
    |> String.concat " "
    |> ClassName

let viewCard (card: Card) dispatch = 
    div 
      [
          classList [ "card-container", true; "match-found", card.MatchFound] 
          OnClick (fun _ -> cardClicked card dispatch)
      ] 
      [
          img [ Src (if card.Selected then card.ImgUrl else "images/fable.png") ] []
      ]

let gameCleared (model: Model) =  
    List.forall (fun card -> card.MatchFound) model.Cards

let view model dispatch =
    if gameCleared model
    then h1 
          [ ClassName "winner centered"; 
            Style [ Padding 20; unbox ("width", "500px") ]
            OnClick (fun _ -> dispatch StartNewGame ) ] 
          [ str "You win, Click me to play again" ]
    else
    div [ ClassName "container centered"; Style [ unbox ("width", "500px") ] ] 
        [ for card in model.Cards -> viewCard card dispatch ]
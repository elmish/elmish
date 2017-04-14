module FifteenPuzzle.View

open FifteenPuzzle.Types
open FifteenPuzzle.Styles


open Fable.Helpers.React
open Fable.Helpers.React.Props

let correctSlotsOrder = 
  [ for x in [0 .. 3] do
    for y in [0 .. 3] do
    yield { X = x; Y = y } ]
  |> Seq.mapi (fun i pos -> pos, string (i + 1))

let inCorrectPosition slot = 
  let (slotPos, slotTag) = slot
  correctSlotsOrder
  |> Seq.find (fun (pos, tag) -> pos = slotPos)
  |> fun (pos, tag) -> tag = slotTag

let gameCleared (state: AppState) = 
   state.Slots
   |> List.filter (fun (pos, tag) -> pos <> state.FreePos)
   |> List.forall inCorrectPosition 

let slotView (slot: Slot) freePos dispatch = 
  let (slotPos, slotTag) = slot
  if freePos = slotPos then 
    div 
      [ freeSlotStyle ]
      [  ]
  elif inCorrectPosition slot then
    div 
      [ redSlotStyle ; OnClick (fun _ -> dispatch (SelectSlot slot)) ]
      [ str slotTag ]
  else
    div 
      [ greenSlotStyle ; OnClick (fun _ -> dispatch (SelectSlot slot)) ]
      [ str slotTag ]

let tableRow xs = tr [] [ for x in xs -> td [] [x] ]

let view (state: AppState) dispatch =
    let slots = Array.ofSeq state.Slots
    let freePos = state.FreePos
    let freeSlot = freePos, "16"
    if gameCleared state 
    then
      h1 
        [ headerStyle; OnClick (fun _ -> dispatch StartNewGame) ]
        [ str "You win, click here to play again" ]
    else 
      div
        [ puzzleStyle ]
        [
          table 
           []
           [  
             tableRow [ for slot in slots.[0 .. 3]   do
                        yield slotView slot freePos dispatch ]
             tableRow [ for slot in slots.[4 .. 7]   do
                        yield slotView slot freePos dispatch ]
             tableRow [ for slot in slots.[8 .. 11]  do
                        yield slotView slot freePos dispatch ]
             tableRow [ for slot in slots.[12 .. 15] do
                        yield slotView slot freePos dispatch ]
           ]
        ]
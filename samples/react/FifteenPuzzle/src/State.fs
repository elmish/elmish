module FifteenPuzzle.State

open FifteenPuzzle.Types
open Fable.Import.Browser

let random = System.Random()

let initialState() : AppState = 
    let randomTags = List.sortBy (fun _ -> random.Next()) [1 .. 16]
    // generate slot positions
    [ for x in 0 .. 3 do 
      for y in 0 .. 3 do 
      yield { X = x; Y = y }  ]
    // give each position a tag, making it a slot
    |> List.mapi (fun i pos -> pos, string (List.item i randomTags))
    // scramble 
    |> fun slots -> 
        // find the free slot, it has tag "16"
        let (pos, _) = Seq.find (fun (p, tag) -> tag = "16") slots
        // return initial state
        { Slots = slots; FreePos = pos }


let update appMessage (state: AppState) = 
    match appMessage with
    | StartNewGame -> initialState()
    | SelectSlot selectedSlot ->
        let (selectedPos, selectedTag) = selectedSlot
        let freePos = state.FreePos
        let dx = abs (freePos.X - selectedPos.X)
        let dy = abs (freePos.Y - selectedPos.Y)
        // check whether or not the selected slot can be replaced 
        // with the free slot
        // let canMove (dx = 1 && dy <> 1) || (dx <> 1 && dy = 1) 
        let cantMove = dx + dy > 1
        if cantMove then state 
        else 
        let slots = 
            state.Slots  
            |> List.map (fun (pos, tag) -> 
                if pos = freePos then pos, selectedTag
                else (pos, tag))
        { Slots = slots; FreePos = selectedPos } 

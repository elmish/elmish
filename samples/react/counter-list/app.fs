module App

(**
 - title: Counter-List
 - tagline: The famous Increment/Decrement list sample ported from Elm
*)

open Fable.Core
open Fable.Import
open Elmish

// MODEL

type ID = int

type Model = {
    Counters : Counter.Model list
}

type Msg = 
| Insert
| Remove
| Modify of ID * Counter.Msg

let init() : Model =
    { Counters = [] }

// UPDATE

let update (msg:Msg) (model:Model) =

    match msg with
    | Insert ->
        let newCounter = Counter.init()
        { Counters = newCounter :: model.Counters }

    | Remove ->
        { Counters = List.tail model.Counters }

    | Modify (id, counterMsg) ->
        { model with
            Counters =
                model.Counters
                |> List.mapi (fun i counterModel -> 
                    if i = id then
                        Counter.update counterMsg counterModel
                    else
                        counterModel) }

open Fable.Core.JsInterop
open Fable.Helpers.React.Props
module R = Fable.Helpers.React

// VIEW (rendered with React)

let view model dispatch =
    let remove = R.button [ OnClick (fun _ -> dispatch Remove) ] [ R.str "Remove" ]

    let insert = R.button [ OnClick (fun _ -> dispatch Insert) ] [ R.str "Add" ]

    let counterDispatch i msg = dispatch (Modify (i, msg))

    let counters = List.mapi (fun i c -> Counter.view c (counterDispatch i)) model.Counters
    
    R.div [] [ 
        yield remove
        yield insert 
        yield! counters ]

open Elmish.React

// App
Program.mkSimple init update view
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.run
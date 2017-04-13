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
        { Counters = Counter.init() :: model.Counters }

    | Remove ->
        { Counters = 
            match model.Counters with
            | [] -> []
            | x :: rest -> rest }

    | Modify (id, counterMsg) ->
        { Counters =
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
    let counterDispatch i msg = dispatch (Modify (i, msg))

    let counters = 
        model.Counters
        |> List.mapi (fun i c -> Counter.view c (counterDispatch i))
    
    R.div [] [ 
        yield R.button [ OnClick (fun _ -> dispatch Remove) ] [ R.str "Remove" ]
        yield R.button [ OnClick (fun _ -> dispatch Insert) ] [ R.str "Add" ] 
        yield! counters ]

open Elmish.React

// App
Program.mkSimple init update view
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
|> Program.run
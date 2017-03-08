(**
 - title: Counter
 - tagline: The famous Increment/Decrement ported from Elm
*)


#r "../node_modules/fable-core/Fable.Core.dll"
#r "../node_modules/fable-powerpack/Fable.PowerPack.dll"
#r "../node_modules/fable-elmish/Fable.Elmish.dll"
//#r "../../../src/elmish-snabddom/npm/Fable.Elmish.Snabbdom.dll"
#r "../node_modules/fable-elmish-snabbdom/Fable.Elmish.Snabbdom.dll"

open Fable.Core
open Fable.Import
open Elmish

// MODEL
type Msg =
  | Increment
  | Decrement

let init () = 0

// UPDATE
let update (msg:Msg) count =
  match msg with
  | Increment -> count + 1
  | Decrement -> count - 1

// rendering views with React
open Fable.Helpers.Snabbdom
open Fable.Helpers.Snabbdom.Props

let view count dispatch =
  let onClick msg =
    fun _ -> dispatch msg
    |> OnClick

  div []
    [ button
        [ Events [
            onClick Decrement
          ]
        ]
        [ unbox "-" ]
      div
        []
        [ unbox (string count) ]
      button
        [ Events [
            onClick Increment
          ]
        ]
        [ unbox "+" ]
    ]

open Elmish.Snabbdom

// App
Program.mkSimple init update view
|> Program.withConsoleTrace
|> Program.withSnabbdom "elmish-app"
|> Program.run

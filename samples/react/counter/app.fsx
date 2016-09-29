(**
 - title: Counter
 - tagline: The famous Increment/Decrement ported from Elm
*)


#r "node_modules/fable-core/Fable.Core.dll"
#load "node_modules/fable-import-react/Fable.Import.React.fs"
#load "node_modules/fable-import-react/Fable.Helpers.React.fs"
#load "node_modules/fable-elmish/elmish.fs"

open Fable.Core
open Fable.Import
open Elmish



// MODEL

type Model = {count:int}


type Msg =
  | Increment
  | Decrement


let init () =
  { count = 0 }


// UPDATE

let update (msg:Msg) {count = count} =
  match msg with
  | Increment ->
      { count = count + 1 }

  | Decrement ->
      { count = count - 1 }


// rendering views with React
module R = Fable.Helpers.React
open Fable.Core.JsInterop
open Fable.Helpers.React.Props

let view {count = count} dispatch =
  let onClick msg =
    OnClick <| fun _ -> msg |> dispatch 

  R.div []
    [ R.button [ onClick Decrement ] [ unbox "-" ]
      R.div [] [ unbox (string count) ]
      R.button [ onClick Increment ] [ unbox "+" ]
    ]

// App
let program = 
    Program.mkSimple init update
    |> Program.withConsoleTrace

type App() as this =
    inherit React.Component<obj, Model>()
    
    let safeState state =
        match unbox this.props with 
        | false -> this.state <- state
        | _ -> this.setState state

    let dispatch = program |> Program.run safeState

    member this.componentDidMount() =
        this.props <- true

    member this.render() =
        view this.state dispatch

ReactDom.render(
        R.com<App,_,_> () [],
        Browser.document.getElementsByClassName("elmish-app").[0]
    ) |> ignore
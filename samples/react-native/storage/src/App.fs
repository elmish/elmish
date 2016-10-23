module AwesomeApp

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

open Elmish

// MODEL

type Model = { 
  status: string
  count: int option 
}


type Msg =
  | Load
  | Increment
  | Decrement
  | Loaded of int option
  | ErrorStoring of exn
  | NotPresent
  | Saved
  | Erase

let init () =
  { status = "loading..."
    count = None }, Cmd.ofMsg Load 

// UPDATE

let update (load,save,erase) (msg:Msg) model =
  match msg with
  | Saved -> 
      { model with status = "saved" }, Cmd.none

  | Load -> 
      model, load()

  | Erase -> 
      model, erase()

  | Loaded (Some stored) -> 
      { model with count = Some stored
                   status = "loaded" }, Cmd.none
  | Loaded _ -> 
      { model with count = Some 0
                   status = "new" }, Cmd.none

  | NotPresent -> 
      { model with count = Some 0
                   status = "new" }, Cmd.none

  | ErrorStoring err -> 
      { model with status = (string err) }, Cmd.none

  | Increment ->
      let model' = { model with count = model.count |> Option.map ((+) 1) }
      model', save model'.count

  | Decrement ->
      let model' = { model with count = model.count |> Option.map (fun v -> v - 1) }
      model', save model'.count



// rendering views with ReactNative
open Fable.Core.JsInterop
module R = Fable.Helpers.ReactNative

let view (state:Model) (dispatch:Dispatch<Msg>) =
  let onClick msg =
    fun () -> msg |> dispatch 

  R.view [Styles.sceneBackground]
    [ R.text [TextProperties.Style [TextAlign TextAlignment.Center]] (string state.status)
      Styles.button "-" (onClick Decrement)
      R.text [TextProperties.Style [TextAlign TextAlignment.Center]] (string state.count)
      Styles.button "+" (onClick Increment)
      Styles.button "Erase" (onClick Erase) ]

// storage
open Fable.Helpers.ReactNativeSimpleStore
let load () =
  Cmd.ofAsync DB.get<int option> 0 Loaded (fun _ -> NotPresent)

let save value =
  Cmd.ofAsync DB.update<int option> (0,value) (fun _ -> Saved) ErrorStoring

let erase () =
  Cmd.ofAsync DB.clear<int option> () (fun _ -> NotPresent) ErrorStoring

let updatePersistence msg model =
  update (load,save,erase) msg model

// App
open Elmish.ReactNative
let runnable:obj->obj = 
    Program.mkProgram init updatePersistence view
    |> Program.withConsoleTrace
    |> Program.toRunnable Program.run

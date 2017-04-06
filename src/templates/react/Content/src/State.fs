module State.App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import.Browser
open Global
open Types.App

let pageParser: Parser<Page->Page,Page> =
  oneOf [
    map About (s "about")
    map Counter (s "counter")
    map Home (s "home")
  ]

let urlUpdate (result: Option<Page>) model =
  match result with
  | None ->
    console.error("Error parsing url")
    ( model,Navigation.modifyUrl (toHash model.currentPage) )
  | Some page ->
      match page with
      | Counter ->
          let (counter, counterCmd) = State.Counter.init ()
          { model with
              currentPage = page
              counter = counter }, Cmd.map CounterMsg counterCmd
      | Home ->
          let (home, homeCmd) = State.Home.init ()
          { model with
              currentPage = page
              home = home }, Cmd.map HomeMsg homeCmd
      | _ -> { model with currentPage = page }, []

let initialModel = {
    currentPage = Home
    counter = 0
    home = ""
  }

let initApp result =
  urlUpdate result initialModel

let update msg model =
  match msg with
  | CounterMsg msg ->
      let (counter, counterCmd) = State.Counter.update msg model.counter
      { model with counter = counter }, Cmd.map CounterMsg counterCmd
  | HomeMsg msg ->
      let (home, homeCmd) = State.Home.update msg model.home
      { model with home = home }, Cmd.map HomeMsg homeCmd

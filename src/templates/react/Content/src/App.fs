namespace FableElmishReactTemplate

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open FableElmishReactTemplate.Common

module App =

  importAll "../sass/main.sass"

  type SubModels =
    {
      Home: Home.Model
      Counter: Counter.Model
    }

    static member Initial =
      {
        Home = ""
        Counter = 0
      }

  type Model =
    {
      CurrentPage: Page
      SubModels: SubModels
    }

    static member Initial =
      {
        CurrentPage = Home
        SubModels = SubModels.Initial
      }

  let urlUpdate (result: Option<Page>) model =
    match result with
    | None ->
      Browser.console.error("Error parsing url")
      ( model, Navigation.modifyUrl (toHash model.CurrentPage) )
    | Some page ->
        { model with CurrentPage = page }, []

  let init result =
    urlUpdate result Model.Initial

  type Msg =
    | CounterMsg of Counter.Msg
    | HomeMsg of Home.Msg
    | NavigateTo of Page

  let update msg model =
    match msg with
    | CounterMsg msg ->
        let (res, _) = Counter.update msg model.SubModels.Counter
        let localUpdate counter = { model.SubModels with Counter = counter }
        { model with SubModels = localUpdate res }, Cmd.Empty
    | HomeMsg msg ->
        let (res, _) = Home.update msg model.SubModels.Home
        let localUpdate home = { model.SubModels with Home = home }
        { model with SubModels = localUpdate res }, Cmd.Empty
    | NavigateTo page ->
        { model with CurrentPage = page }, Navigation.newUrl (toHash page)

  open Fable.Helpers.React
  open Fable.Helpers.React.Props

  let menuItem label page currentPage dispatch =
      li
        [ ]
        [ a
            [ classList [ "is-active", page = currentPage ]
              OnClick (fun _ -> NavigateTo page |> dispatch)
            ]
            [ str label ]
        ]

  let menu currentPage dispatch =
    aside
      [ ClassName "menu"
      ]
      [ p
          [ ClassName "menu-label"
          ]
          [ str "General" ]
        ul
          [ ClassName "menu-list"
          ]
          [ menuItem "Home" Home currentPage dispatch
            menuItem "Counter sample" Counter currentPage dispatch
            menuItem "About" Page.About currentPage dispatch
          ]
      ]

  let view model dispatch =

    let pageHtml =
      function
      | Page.About -> About.view
      | Counter -> Counter.view model.SubModels.Counter (CounterMsg >> dispatch)
      | Home -> Home.view model.SubModels.Home (HomeMsg >> dispatch)

    div
      []
      [ div
          [ ClassName "navbar-bg"
          ]
          [ div
              [ ClassName "container"
              ]
              [ Navbar.view
              ]
          ]
        div
          [ ClassName "section"
          ]
          [ div
              [ ClassName "container"
              ]
              [ div
                  [ ClassName "columns"
                  ]
                  [
                    div
                      [ ClassName "column is-3"
                      ]
                      [ menu model.CurrentPage dispatch ]
                    div
                      [ ClassName "column"
                      ]
                      [ pageHtml model.CurrentPage ]
                  ]
              ]
          ]
      ]

  open Elmish.React
  open Elmish.Debug

  // App
  Program.mkProgram init update view
  |> Program.toNavigable (parseHash pageParser) urlUpdate
  |> Program.withReact "elmish-app"
  |> Program.withDebugger
  |> Program.run

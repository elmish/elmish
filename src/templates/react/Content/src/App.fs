module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Types.App
open State.App
open Global
open Global.Helpers

importAll "../sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props

let menuItem label page currentPage dispatch =
    li
      [ ]
      [ a
          [ classList [ "is-active", page = currentPage ]
            Href (toHash page) ]
          [ unbox label ] ]

let menu currentPage dispatch =
  aside
    [ ClassName "menu" ]
    [ p
        [ ClassName "menu-label" ]
        [ unbox "General" ]
      ul
        [ ClassName "menu-list" ]
        [ menuItem "Home" Home currentPage dispatch
          menuItem "Counter sample" Counter currentPage dispatch
          menuItem "About" Page.About currentPage dispatch ] ]

let root model dispatch =

  let pageHtml =
    function
    | Page.About -> Views.About.root
    | Counter -> Views.Counter.root model.counter (CounterMsg >> dispatch)
    | Home -> Views.Home.root model.home (HomeMsg >> dispatch)

  div
    []
    [ div
        [ ClassName "navbar-bg" ]
        [ div
            [ ClassName "container" ]
            [ Views.Navbar.root ] ]
      div
        [ ClassName "section" ]
        [ div
            [ ClassName "container" ]
            [ div
                [ ClassName "columns" ]
                [ div
                    [ ClassName "column is-3" ]
                    [ menu model.currentPage dispatch ]
                  div
                    [ ClassName "column" ]
                    [ pageHtml model.currentPage ] ] ] ] ]

open Elmish.React
open Elmish.Debug

// App
Program.mkProgram initApp update root
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withReact "elmish-app"
|> Program.withDebugger
|> Program.run

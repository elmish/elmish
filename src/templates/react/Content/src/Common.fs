namespace FableElmishReactTemplate

open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

module Common =

  type Page =
    | Home
    | Counter
    | About

  let toHash =
    function
    | About -> "#about"
    | Counter -> "#counter"
    | Home -> "#home"

  let pageParser: Parser<Page->Page,Page> =
    oneOf [
      map About (s "about")
      map Counter (s "counter")
      map Home (s "home")
    ]

  open Fable.Helpers.React.Props

  let internal classList classes =
    classes
    |> List.fold (fun complete -> function | (name,true) -> complete + " " + name | _ -> complete) ""
    |> ClassName

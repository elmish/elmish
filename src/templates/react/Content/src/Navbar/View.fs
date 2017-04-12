module Navbar.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

let navButton classy href faClass txt =
  a
    [ ClassName (sprintf "button %s" classy)
      Href href ]
    [ span
        [ ClassName "icon" ]
        [ i
            [ ClassName (sprintf "fa %s" faClass) ]
            [ ] ]
      span
        [ ]
        [ str txt ] ]

let navButtons =
  span
    [ ClassName "nav-item" ]
    [ navButton "twitter" "https://twitter.com/FableCompiler" "fa-twitter" "Twitter"
      navButton "github" "https://github.com/fable-compiler/fable-elmish" "fa-github" "Fork me"
      navButton "github" "https://gitter.im/fable-compiler/Fable" "fa-comments" "Gitter" ]

let root =
  nav
    [ ClassName "nav" ]
    [ div
        [ ClassName "nav-left" ]
        [ h1
            [ ClassName "nav-item is-brand title is-4" ]
            [ str "Elmish" ] ]
      navButtons ]

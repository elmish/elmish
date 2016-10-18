Elmish: Elm-like abstractions for F# applications targeting [Fable](https://fable-compiler.github.io/).
=======

Elmish implements core abstractions that can be used to build applications following [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.
The library however does not model any "view" and is intended for use in conjuction with a DOM/renderer, like React/ReactNative or VirtualDOM.  
For those familiar with Redux, the Fable applications targeting React or ReactNative may find Elmish a more natural fit than Redux allowing one to stay completely in idiomatic F#. 

Elimsh abstractions have been carefully designed to resemble Elm's "look and feel" and anyone familiar with post-Signal Elm terminology will find themselves at right home.

## Basic dispatch.
> Include source: `fable-elmish.fs`

Usage:
```fsharp
open Elmish
```

## Promise: Integrate JS promises.
> Include source: `elmish-promise.fs`

Usage:
```fsharp
open Elmish
```

## Result: Ok/Error type and operators.
> Include source: `elmish-result.fs`

Usage:
```fsharp
open Elmish
```

## UrlParser: Combinator for parsing browser's location url.
> Include source: `elmish-parser.fs`

Usage:
```fsharp
open Elmish.UrlParser
```

## Navigation: Integrate with browser's location and history.
> Include source: `elmish-browser-nav.fs`

Usage:
```fsharp
open Elmish.Browser.Navigation

Program.mkProgram init update
|> Program.runWithNavigation parser urlUpdate setState

```
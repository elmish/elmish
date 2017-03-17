Elmish: Elm-like abstractions for F# applications targeting [Fable](https://fable-compiler.github.io/).
=======

Elmish implements core abstractions that can be used to build applications following [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.
The library however does not model any "view" and is intended for use in conjuction with a DOM/renderer, like React/ReactNative or VirtualDOM.
For those familiar with Redux, the Fable applications targeting React or ReactNative may find Elmish a more natural fit than Redux allowing one to stay completely in idiomatic F#.

Elmish abstractions have been carefully designed to resemble Elm's "look and feel" and anyone familiar with post-Signal Elm terminology will find themselves at right home.

## Installation

In a project directory with `dotnet-fable` CLI tools installed, type:

```shell
dotnet fable add fable-powerpack fable-elmish
```

## Basic dispatch
> Note that by itself the following program expects the `view` to carry out both DOM construction *and* DOM rendering.
 
Usage:
```fsharp
open Elmish

Program.mkProgram init update view
|> Program.run
```

## Async: schedule a command transforming the results of an `async<_>` function
Usage:
```fsharp
open Elmish

Cmd.ofAsync asyncFunction arg ofSuccess ofError
```

## Promise: schedule a command transforming the results of a `promise<_>` function
Usage:
```fsharp
open Elmish

Cmd.ofPromise promiseFunction arg ofSuccess ofError
```

## Result: Ok/Error type and operators.
> This type is standard in F# 4.1 and will be removed from this lib when MS releases it.

Usage:
```fsharp
open Elmish

```

## UrlParser: Combinator for parsing browser's location url
Usage:
```fsharp
open Elmish.UrlParser
```

## Navigation: Integrate with browser's location and history
Usage:
```fsharp
open Elmish.Browser.Navigation

Program.mkProgram init update view
|> Program.toNavigable parser urlUpdate 
|> Program.run

```

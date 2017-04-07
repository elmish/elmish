Elmish-browser: browser extras for Elmish apps.
=======

Elmish-browser implements routing and navigation for elmish apps targeting browser (React) renderers.

## Installation

In a project directory with `dotnet-fable` CLI tools installed, type:

```shell
dotnet fable add fable-powerpack fable-elmish fable-elmish-browser.
```

## Porting from previous version of the parser
In addition to providing query parsing capabilities, this port from Elm/url-parser makes a few changes to the API:
- `format` has been renamed `map`
- `Result` return type has been replaced with `Option`
- `parseHash` is already provided, just pass your parser as its arg
- new `parsePath` works with entire url, not just the hash portion

If you've been using `Result` type for other purposes, it is now available in F# 4.1 Core and is still available from Fable-PowerPack.


## Routing: Combinators for parsing browser's url into a route
Usage:
```fsharp
open Elmish.Browser.Navigation

Program.mkProgram init update view
|> Program.toNavigable parser urlUpdate
|> Program.run
```

## Navigation: Manipulate the browser's navigation and history
Usage:
```fsharp
open Elmish.Browser.Navigation

let update model msg =
    model, Navigation.newUrl "some_other_location"
```


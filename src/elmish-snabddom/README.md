Elmish-Snabbdom: Snabbdom extensions for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
=======

## Installation

```shell
npm install --save snabbdom
npm install --save-dev fable-core fable-powerpack fable-elmish fable-elmish-snabbdom
```

Add a reference to the assemblies in the package folders (e.g. `node_modules/fable-elmish/Fable.Elmish.dll`).

## App component

Snabbdom application needs a root component to be rendered at the specified placeholder:

Usage:

```fsharp
open Elmish.Snabbdom

Program.mkProgram init update view
|> Program.withSnabbdom "placehoder"
|> Program.run
```

## Choose your modules

Snabbdom use modules to provide functionalities.

`Program.withSnabbdom` will load this modules:
* Snabbdom.Modules.Attributes
* Snabbdom.Modules.Class
* Snabbdom.Modules.EventListeners
* Snabbdom.Modules.Props
* Snabbdom.Modules.Style

You can choose which module to load by using `Program.withSnabbdomAndModules`.

Example:

```fsharp
Program.mkProgram init update view
|> Program.withSnabbdomAndModules
        [|
            Snabbdom.Modules.EventListeners
            Snabbdom.Modules.Props
        |]
        "placeholder"
|> Program.run
```

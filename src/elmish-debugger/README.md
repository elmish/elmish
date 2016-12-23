Elmish-Debugger: [Remotedev tools](https://github.com/zalmoxisus/remotedev) integration for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
=======

## Installation

```shell
npm install --save-dev remotedev
npm install --save-dev fable-core fable-powerpack fable-elmish fable-debugger
```

Add a reference to the assemblies in the package folders (e.g. `node_modules/fable-elmish/Fable.Elmish.Debugger.dll`).

Follow the monitor installation instructions at [Remotedev tools](https://github.com/zalmoxisus/remotedev) site.


## Program module functions
Augument your program instance with a debugger:

Usage:
```fsharp
open Elmish.Debug

Program.mkProgram init update view
|> Program.withDebugger // connect to a devtools monitor via Chrome extension if available
|> Program.run

```

or:

Usage:
```fsharp
open Elmish.Debug

Program.mkProgram init update view
|> Program.withDebuggerAt (Remote("localhost",8000)) // connect to a monitor running on localhost:8000
|> Program.run


or, using a custom connection:

Usage:
```fsharp
open Elmish.Debug

let connection = Debugger.connect (Secure("localhost",8080)) // connect to a monitor over a secure socket

Program.mkProgram init update view
|> Program.withDebuggerUsing connection
|> Program.run


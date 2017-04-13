Elmish-Debugger: [Remotedev tools](https://github.com/zalmoxisus/remotedev) integration for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
=======

## Installation

```shell
npm install --save remotedev
dotnet fable add fable-elmish-debugger fable-elmish
```

Add a reference to the assemblies in the package folders (e.g. `node_modules/fable-elmish/Fable.Elmish.Debugger.dll`).

Follow the monitor installation instructions at [Remotedev tools](https://github.com/zalmoxisus/remotedev) site.


## Program module functions
Augment your program instance with a debugger, make sure it's the last item in the `Program` pipeline:

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
|> Program.withDebuggerAt (Remote("localhost",8000)) // connect to a server running on localhost:8000
|> Program.run


or, using a custom connection:

Usage:
```fsharp
open Elmish.Debug

let connection = Debugger.connect (Remote("localhost",8080)) // obtain the connection, for example if sending some information directly

Program.mkProgram init update view
|> Program.withDebuggerUsing connection
|> Program.run


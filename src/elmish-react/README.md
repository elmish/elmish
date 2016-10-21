Elmish-React: React extensions for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
=======
Implements boilerplate to setup a root component and get the dispatch loop started.

> Reference: `#r "node_modules/fable-elmish-react/bin/Elmish.React.dll`
> fableconfig/refs: "Elmish.React": "fable-elmish-react/es2015"

## App component.
This component is used internally by the bootstrap code for React and ReactNative, shown bellow.

## React (Html) helpers.
Construct your program as you normally would then pass it along with the chosen `run` function and the placeholder `div` id where the program should be rendered:

Usage:
```fsharp
open Elmish.React

Program.toHtml Program.run "elmish-app" program

```

## React (Native) helpers.
Construct your program as you normally would then convert it to `runnable` let binding, which would be referenced from the `index.js`:

Usage:
```fsharp
open Elmish.ReactNative

let runnable : obj->obj =
    Program.toRunnable Program.run program

```

in index.ios.js and index.android.js:

```js
import {AppRegistry} from 'react-native';
import {runnable} from './out/App';

AppRegistry.registerRunnable('awesome', runnable);
```

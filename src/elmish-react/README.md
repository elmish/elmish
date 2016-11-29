Elmish-React: React extensions for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
=======

## Installation

```shell
npm install --save react react-dom
npm install --save-dev fable-core fable-powerpack fable-elmish fable-react fable-elmish-react
```

Add a reference to the assemblies in the package folders (e.g. `node_modules/fable-elmish/Fable.Elmish.dll`).
## App component.
React application needs a root component to be rendered at the specified placeholder:

Usage:
```fsharp
open Elmish.React

Program.mkProgram init update view
|> Program.withReact "placehoder"
|> Program.run

```

ReactNative application needs a `runnable` function:

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



## Lazy views.
Rendering of any view can be optimizied by avoiding the DOM reconciliation and skipping the DOM construction entierly if there are no changes in the model.
Can be used by both React and ReactNative applications.

`lazyView` can be used with equattable models (most F# core types: records, tuples,etc).

`lazyViewWith` can be used with types that don't implement `equality` constraint, such us types/instances coming from JS libraries, by passing the custom `equal` function that compares the previous and the new models.

Usage:
```fsharp
open Elmish.React

lazyView view model
lazyView2 view model dispatch
lazyView3 view model1 model2 dispatch

```

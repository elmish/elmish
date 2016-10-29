Elmish-React: React extensions for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
=======


## App component.
> Include source: `elmish-app.fs`

This file is used internally by the rest of the library and must be included first.

## Lazy views.
> Include source: `elmish-app.fs`

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

## React (Html) helpers.
> Include source: `elmish-react.fs`

Implements boilerplate to setup a root component and get the dispatch loop started for React application.

Usage:
```fsharp
open Elmish.React

Program.toHtml Program.run "elmish-app" program

```

## React (Native) helpers.
> Include source: `elmish-native.fs`

Implements boilerplate to setup a root component and get the dispatch loop started for ReactNative application.

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

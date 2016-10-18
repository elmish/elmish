Elmish-React: React extensions for [fable-elmish](https://github.com/fable-compiler/fable-elmish) applications.
Implements boilerplate to setup a root component and get the dispatch loop started.
=======


## App component.
> Include source: `elmish-app.fs`

## React (Html) helpers.
> Include source: `elmish-react.fs`

Usage:
```fsharp
open Elmish.React

Program.toHtml Program.run "elmish-app" program

```

## React (Html) helpers.
> Include source: `elmish-native.fs`

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

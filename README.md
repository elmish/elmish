Elmish: Elm-like abstractions for F# applications targeting Fable.
=======

[![npm version](https://badge.fury.io/js/fable-elmish.svg)](https://badge.fury.io/js/fable-elmish)
[![Gitter](https://badges.gitter.im/gitterHQ/gitter.svg)](https://gitter.im/fable-compiler/Fable)
[![Windows Build status](https://ci.appveyor.com/api/projects/status/fdb2fxf2h9bd719r?svg=true)](https://ci.appveyor.com/project/et1975/fable-elmish)
[![Mono Build Status](https://travis-ci.org/fable-compiler/fable-elmish.svg "Mono Build Status")](https://travis-ci.org/fable-compiler/fable-elmish)


Elmish implements core abstractions that can be used to build applications following [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.
The library however does not model any "view" and is intended for use in conjuction with a DOM/renderer, like React/ReactNative or VirtualDOM.
For those familiar with Redux, the [Fable](https://github.com/fable-compiler) applications targeting React or ReactNative may find Elmish a more natural fit than Redux allowing one to stay completely in idiomatic F#.

Elmish abstractions have been carefully designed to resemble Elm's "look and feel" and anyone familiar with post-Signal Elm terminology will find themselves right at home.

## Concepts

Elm architecture operates using following concepts, as they translate to Elmish:

### Model
This is a snapshot of your applications state, defined as an immutable data structure.

### Message
This an event representing a change (delta) in state of your applications, defined as a discriminated union.

### Init
This is a pure function that produces inital state of your application and, optionally, message(s) to process.

### Update
This is a pure function that produces new state of your application given the previous state and, optionally, new message(s) to process.

### View
This is a pure function that produces new UI layout/content given the current state, defined as a F# function that uses a renderer (such as React) to declaratively build a UI.

### Program
This is an opaque data structure that combines all of the above + your `setState` function to start a dispatch loop.


## Advanced Concepts
A dispatch loop is responsible for running the [update cycle](http://www.elm-tutorial.org/en/02-elm-arch/04-flow.html).

### Commands
Command is an opaque data structure that when evaluated may produce one or more new messages.

### Tasks
Task is any code that will execute when a command is evalutated, like reading a database, defined in Elmish as `async` or `promise` blocks or just a plain function.

### Subscriptions
These are external sources of events to process, defined as a F# functions that can dispatch new messages as they happen.

## Parent-child composition
Please refer to [Elm's diagrams](https://www.elm-tutorial.org/en-v01/02-elm-arch/08-composing-3.html) for an overview.

Application state is composed of the state of all its children, events buble up from the children all the way to the top update function where they are distributed back to the children in an explicit fashion.
The views delegate portions of the model to the appropriate child views to produce the new UI.


Extras
=======

## Elmish-Browser: browser-specific features, like working with address bar.
`dotnet fable add fable-elmish-browser`

For more information see the [readme](https://github.com/fable-compiler/fable-elmish/blob/master/src/elmish-browser/README.md)

## Elmish-React: boilerplate to get the React/Native root components registered and start the dispatch loop.
`dotnet fable add fable-elmish-react`

For more information see the [readme](https://github.com/fable-compiler/fable-elmish/blob/master/src/elmish-react/README.md)

For more information see the [readme](https://github.com/fable-compiler/fable-elmish/blob/master/src/elmish-snabddom/README.md)


## Elmish-Debugger: RemoteDev tools integration adds support for time-travelling debugger and import/export.
`dotnet fable add fable-elmish-debugger`

For more information see the [readme](https://github.com/fable-compiler/fable-elmish/blob/master/src/elmish-debugger/README.md)


Building Elmish
=======
As part of Fable 1.x ecosystem, elmish depends on [dotnet SDK](https://www.microsoft.com/net/download/core).
We also use `yarn`, it can be installed as your usual OS package or via `npm`, make sure it's in the PATH.
> `npm install -g yarn`

Then run fake:
> `./build.sh` or `build`

And/or build samples:
> `build Samples`

Elmish: Elm-like abstractions for F# applications targeting Fable.
=======

Elmish implements core abstractions that can be used to build applications following [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.
The library however does not model any "view" and is intended for use in conjuction with a DOM/renderer, like React/ReactNative or VirtualDOM.  
For those familiar with Redux, the Fable applications targeting React or ReactNative may find Elmish a more natural fit than Redux allowing one to stay completely in idiomatic F#. 

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
Tasks produce commands from side-effects, like reading a database, defined in Elmish as `async` or `promise` blocks, or just a plain function.

### Subscriptions
These are external sources of events to process, defined as a F# functions that can dispatch new messages as they happen.

## Parent-child composition
Please refer to [Elm's diagrams](http://www.elm-tutorial.org/en/02-elm-arch/08-composing-3.html) for an overview.

Application state is composed of the state of all its children, events buble up from the children all the way to the top update function where they are distributed back to the children in an explicit fashion.
The views delegate portions of the model to the appropriate child views to produce the new UI.



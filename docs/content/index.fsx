(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/bin/Debug"

(** Elmish
======================
Elmish implements core abstractions that can be used to build Fable applications following [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.

>`-ish`
>  a suffix used to convey the sense of “having the characteristics of”


Elm architecture operates using following concepts, as they translate to Elmish:

* Model <br />
  This is a snapshot of your applications state, defined as an immutable data structure.

* Message <br />
  This an event representing a change (delta) in state of your applications, defined as a discriminated union.

* Init <br />
  This is a pure function that produces inital state of your application and, optionally, message(s) to process.

* Update <br />
  This is a pure function that produces new state of your application given the previous state and, optionally, new message(s) to process.

* View <br />
  This is a pure function that produces new UI layout/content given the current state, defined as a F# function that uses a renderer (such as React) to declaratively build a UI.

* Program <br />
  This is an opaque data structure that combines all of the above + a `setState` function to produce a view from the model.


Dispatch loop
---------------
![flow](https://www.elm-tutorial.org/en/02-elm-arch/04-flow.png)

Once started, Program runs a dispatch loop, producing new Model given the current state and an input Message.


View
---------------
The core is not dependent on any particular technolgy, instead relying on a renderer library to implement `setState` in any way seen fit.
In fact, an elmish app can run entirely without a UI!

At the moment, there are two UI technologies the rendering has been implemented for: React and React Native.
For details please see [fable-elmish-react](https://fable-elmish.github.io/react).


Observing the state changes
---------------
Every message going through the dispatch loop can be traced, along with the current state of the app.
For more advanced debugging capabilities please see [fable-elmish-debugger](https://fable-elmish.github.io/debugger).


Interacting with a browser
---------------
Larger elmish applications for the browser may benefit from advanced features like routing and explicit navigation control.
For information about these features please see [fable-elmish-browser](https://fable-elmish.github.io/debugger).

*)




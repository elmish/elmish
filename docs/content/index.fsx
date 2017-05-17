(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../src/bin/Debug"

(** Elmish
======================
>`-ish` <br />
>  a suffix used to convey the sense of “having some characteristics of”

Elmish implements core abstractions that can be used to build Fable applications following the [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.


The goal of the architecture is to provide a solid UI-independent core to build the rest of the functionality around.
Elm architecture operates using the following concepts, as they translate to Elmish:

* **Model** <br />
  This is a snapshot of your application's state, defined as an immutable data structure.

* **Message** <br />
  This an event representing a change (delta) in the state of your application, defined as a discriminated union.

* **Init** <br />
  This is a pure function that produces the inital state of your application and, optionally, message(s) to process.

* **Update** <br />
  This is a pure function that produces a new state of your application given the previous state and, optionally, new message(s) to process.

* **View** <br />
  This is a pure function that produces a new UI layout/content given the current state, defined as an F# function that uses a renderer (such as React) to declaratively build a UI.

* **Program** <br />
  This is an opaque data structure that combines all of the above plus a `setState` function to produce a view from the model.
  See the [`Program`](program.html) module for more details.


Dispatch loop
---------------
![flow](https://www.elm-tutorial.org/en/02-elm-arch/04-flow.png)

Once started, Program runs a dispatch loop, producing a new Model given the current state and an input Message.


Parent-child composition and user interaction
---------------
Parent-child hierarchy is made explicit by wrapping model and message types of the child with those of the parent.

![flow](https://www.elm-tutorial.org/en-v01/02-elm-arch/06-composing_001.png)

1. User clicks on the increase button
2. `Widget.view` dispatches an `Increase` message
3. `Main.view` has augemented the `dispatch` so the message becomes `WidgetMsg Increase` as it is sent along to `program`
4. `program` calls `Main.update` with this message and `mainModel`
5. As the message was tagged with `WidgetMsg`, `Main.update` delegates the update to `Widget.update`, sending along the way the `widgetModel` part of `mainModel`
6. `Widget.update` modifies the model according to the given message, in this case `Increase`, and returns the modified `widgetModel` plus a command
7. `Main.update` returns the updated `mainModel` to `program`
8. `program` then renders the view again passing the updated `mainModel`


Tasks and side-effects
---------------
Tasks such as reading a database or making a Web API call are performed using `async` and `promise` blocks or just plain functions.
These operations may return immediately but complete (or fail) at some later time.
To feed the results back into the dispatch loop, instead of executing the operations directly, we instruct Elmish to do it for us by wrapping the instruction in a command.


Commands
---------------
Commands are carriers of instructions, which you issue from the `init` and `update` functions.
Once evaluated, a command may produce one or more new messages, mapping success or failure as instructed ahead of time. 
As with any message dispatch, in the case of Parent-Child composition, child commands need to be mapped to the parent's type: 

![cmd](https://www.elm-tutorial.org/en-v01/03-subs-cmds/02-commands.png)

Here we collect commands from three different levels. At the end we send all these commands to `program` to run.

See the [`Cmd`](cmd.html) module for ways to construct, map and batch commands.


Subscriptions
---------------
Most of the messages (changes in the state) will originate within your code, but some will come from the outside, for example from a timer or a websocket.
These sources can be tapped into with subscriptions, defined as F# functions that can dispatch new messages as they happen. 
Just like the `init` function, subscriptions will be called only once - with the initial model. If you need the current state, you can construct one-off subscribers via the [`Cmd`](cmd.html) module.

See the [`Program`](program.html) module for details.


View
---------------
The core is independent of any particular technolgy, instead relying on a renderer library to implement `setState` in any way seen fit.
In fact, an Elmish app can run entirely without a UI!

At the moment, there are two UI technologies for which rendering has been implemented: React and React Native.
For details please see [fable-elmish-react](https://fable-elmish.github.io/react).


Observing the state changes
---------------
Every message going through the dispatch loop can be traced, along with the current state of the app.
For more advanced debugging capabilities please see [fable-elmish-debugger](https://fable-elmish.github.io/debugger).


Interacting with a browser
---------------
Larger Elmish applications for the browser may benefit from advanced features like routing and explicit navigation control.
For information about these features please see [fable-elmish-browser](https://fable-elmish.github.io/browser).
*)




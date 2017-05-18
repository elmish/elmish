Elmish: Elm-like abstractions for F# applications targeting Fable.
=======

[![Gitter](https://badges.gitter.im/gitterHQ/gitter.svg)](https://gitter.im/fable-compiler/Fable)
[![Windows Build status](https://ci.appveyor.com/api/projects/status/c8k7a67evgci6ama?svg=true)](https://ci.appveyor.com/project/et1975/elmish)
[![Mono Build Status](https://travis-ci.org/fable-elmish/elmish.svg "Mono Build Status")](https://travis-ci.org/fable-elmish/elmish)
[![NuGet version](https://badge.fury.io/nu/Fable.Elmish.svg)](https://badge.fury.io/nu/Fable.Elmish)

Elmish implements core abstractions that can be used to build applications following [“model view update”](http://www.elm-tutorial.org/en/02-elm-arch/01-introduction.html) style of architecture, as made famous by Elm.
The library however does not model any "view" and is intended for use in conjuction with a DOM/renderer, like React/ReactNative or VirtualDOM.
For those familiar with Redux, the [Fable](https://github.com/fable-compiler) applications targeting React or ReactNative may find Elmish a more natural fit than Redux allowing one to stay completely in idiomatic F#.

Elmish abstractions have been carefully designed to resemble Elm's "look and feel" and anyone familiar with post-Signal Elm terminology will find themselves right at home.

See the [docs site](https://fable-elmish.github.io/elmish/) for more information.


Using Elmish
------
v0.9 Beta release of Elmish targets Fable 1.x and uses `dotnet` SDK and can be installed with `paket`:

`paket add nuget Fable.Elmish -i`

For v0.8 (stable) release information please see the [v0.8.x branch](https://github.com/fable-elmish/elmish/tree/v0.8.x)


Building Elmish
------
As part of Fable 1.x ecosystem, elmish depends on [dotnet SDK](https://www.microsoft.com/net/download/core).
Unless the SDK is installed and is in the path the build will download and install it locally:

Just run fake:
> `./build.sh` or `build`


Contributing
------
Please have a look at the [guidelines](https://github.com/fable-elmish/elmish/blob/master/.github/CONTRIBUTING.md).

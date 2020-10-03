Elmish: Elm-like abstractions for F# applications.
=======

[![Gitter](https://badges.gitter.im/gitterHQ/gitter.svg)](https://gitter.im/fable-compiler/Fable)
[![Windows Build status](https://ci.appveyor.com/api/projects/status/c8k7a67evgci6ama?svg=true)](https://ci.appveyor.com/project/et1975/elmish)
[![Mono Build Status](https://travis-ci.org/elmish/elmish.svg "Mono Build Status")](https://travis-ci.org/elmish/elmish)
[![NuGet version](https://badge.fury.io/nu/Fable.Elmish.svg)](https://badge.fury.io/nu/Fable.Elmish)

**Elmish** implements core abstractions that can be used to build applications following the “model view update” style of architecture, as made famous by Elm.
The library however does not model any "view" and is intended for use in conjunction with a DOM/renderer, like React/ReactNative or VirtualDOM.
Those familiar with Redux may find Elmish a more natural fit when targeting React or ReactNative as it allows one to stay completely in idiomatic F#.


Elmish abstractions have been carefully designed to resemble Elm's "look and feel" and anyone familiar with post-Signal Elm terminology will find themselves right at home.

See the [docs site](https://elmish.github.io/elmish/) for more information.


Using Elmish
------
v2.0 and above releases use `dotnet` SDK and can be installed with `dotnet nuget` or `paket`:

For use in a Fable project:
`paket add nuget Fable.Elmish -i`

If targeting CLR, please use Elmish package:
`paket add nuget Elmish -i`

For v1.x release information please see the [v1.x branch](https://github.com/elmish/elmish/tree/v1.x)
For v2.x release information please see the [v2.x branch](https://github.com/elmish/elmish/tree/v2.x)


Building Elmish
------
Elmish depends on [dotnet SDK 3.1.301](https://www.microsoft.com/net/download/core), `fake` tool and `yarn`:

When building for the first time: `dotnet tool restore`
Running fake:
> `dotnet fake build`


Contributing
------
Please have a look at the [guidelines](https://github.com/elmish/elmish/blob/master/.github/CONTRIBUTING.md).

## 4.0.0-beta-6
* WebSharper support by @granicz (Adam Granicz)

## 4.0.0-beta-5
* Breaking: subs receive current model, automatically started/stopped as needed (#248), thanks Kasey Speakman!

## 4.0.0-beta-4
* Move to .NET 6 SDK
* Breaking: dropping .NET 4.6.1 as the target

## 4.0.0-beta-3
* Breaking: `withSubscription` replaces existing subscription, use `mapSubscription` to add/accumulate the subscribers

## 4.0.0-beta-2
* Obsolete all `Cmd.xxx.result` functions

## 4.0.0-beta-1
* Move to .NET 5 SDK
* Deferring `Cmd` and `Sub` changes to v5
* For end-user compatibility with v3 keep `Program.runWith` signature and introduce `Program.runWithDispatch` to allow for multi-threaded sync function.

## 4.0.0-alpha-2
* Changing `Cmd` and `Sub` aliases to DUs
* Changing `ofSub` to take the error mapper
* Dropping netstandard1.6 from Elmish (for CLR) targets

## 4.0.0-alpha-1

* Adding termination
* Moving `syncDispatch` into `runWith` args

## 3.1.0
* Changing Cmd.OfAsync/OfAsyncImmediate `result` implementation to allow exceptions to escape into the dispatch loop.

## 3.0.6

* Changing Cmd.OfAsync implementations to start via 0-interval StartImmediate to mimic .NET behavior

## 3.0.5

* Changing Cmd.OfAsync implementations to start on thread pool to restore v2.x experience
* Adding Cmd.OfAsyncImmediate implementations
* Adding Cmd.OfAsyncWith for custom async start implementations

## 3.0.4

* Access to `Program`'s error handler

## 3.0.3

* Reordering: call `trace` with updated state

## 3.0.1

* Bugfix for ring resizing

## 3.0.0

* Releasing stable 3.0

## 3.0.0-beta-8

* Making `Program` type opaque and reorganizing `Cmd` functions

## 3.0.0-beta-5

* Fable 3 conversion courtesy of Alfonso

## 3.0.0-beta-4

* Ditching PowerPack in favour of Fable.Promise

## 3.0.0-beta-2

* Ditching MailboxProcessor

## 2.0.3

* Adding `Cmd.ofTask` for netstandard

## 2.0.1

* Adding `Cmd.exec`

## 2.0.0

* Stable release

## 2.0.0-beta-4

* re-releasing v1.x for Fable2

## 1.0.3

* re-releasing with azure-functions compatible FSharp.Core dependency

## 1.0.2

* backporting CLR (platform) support

## 1.0.1

* handle exceptions raising from initial subscription

## 1.0.0

* dotnet 2.0 SDK build

## 0.9.2

* `withErrorHandler` modifier
* cumulative `withSubscription`

## 0.9.1

* packaging fix: Console.WriteLine replaced with console, as commited
* Fable 1.1.3 dependency

## 0.9.0

* Releasing using fable 1.x "stable"
* Console tracing from @forki

## 0.9.0-beta-9

* Paket!

## 0.9.0-beta-7

* standalone package repo

## 0.9.0-beta-5

* BREAKING: Moved browser-specific stuff (navigation, urlparser) to elmish-browser

## 0.8.2

* Stricter signatures

## 0.8.1

* Browser navigation: working around IE11/Edge lack of `popstate` event

## 0.8.0

* Expanding `Program` to accommodate plugabble error reporting

## 0.7.2

* Fable dev tools bump

## 0.7.1

* Stable release

## 0.7.1-alpha.3

* Update dependencies

## 0.7.1-alpha.2

* Rearranging `Program` API to prepare for debugger

## 0.7.0-alpha.4

* Update README

## 0.7.0-alpha.3

* Update libraries

## 0.7.0-alpha.2

* Move Promise extensions to `Elmish.Cmd` module

## 0.7.0-alpha.1

* Migrate to Fable 0.7

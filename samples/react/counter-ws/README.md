Counter Modified to use WebSockets
========

This is a modified version of the [Elmish counter sample](https://github.com/fable-compiler/fable-elmish/tree/master/samples/react/counter) illustrating the use of WebSockets in a Fable app.

## Client
Uses ``Fable.Import.Browser.WebSocket`` type.

## Server
Uses ``fable-import-ws`` to fire WebSockets events from an ``Express`` node HTTP server, all from Fable-generated code.

## Build and running the sample
1. Build/Watch the client: `yarn run client`
2. Build/Watch the server: `yarn run watchserver`
3. Start Express server: `yarn run startserver`
4. open http://localhost:8080

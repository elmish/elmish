Counter Modified to use WebSockets
========

This is a modified version of the [Elmish counter sample](https://github.com/fable-compiler/fable-elmish/tree/master/samples/react/counter) illustrating the use of WebSockets in a Fable app.

## Client
Uses ``Fable.Import.Browser.WebSocket`` type.

## Server
Uses ``fable-import-ws`` to fire WebSockets events from an ``Express`` node HTTP server, all from Fable-generated code.

## Build and running the sample
1. Install shared sample dependencies: `pushd .. && yarn install && popd`
2. Install dependencies of this sample: `yarn install`
3. Build/Watch the client: `cd Client && yarn build` or `cd Client && yarn watch`
4. Build/Watch the server: `cd Server && yarn build` or `cd Server && yarn watch`
5. Start Express server: `yarn run start`
6. open http://localhost:8080

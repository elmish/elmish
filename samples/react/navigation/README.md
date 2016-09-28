This is a port of [Elm's counter example](https://github.com/evancz/elm-architecture-tutorial/blob/master/examples/1-button.elm) implemented in F# and targeting Fable and React.
========

This is a simple demo of [Elmish](https://github.com/et1975/fable-elmish).
The model had to be made a record because React doesn't support primitives as state. A limitation one can live with.

## Build
1. `npm install -g fable-compiler`
2. `npm install`
3. `npm run build`

## Running a hot-loading webpack server
`npm start`
open http://localhost:8080/webpack-dev-server/




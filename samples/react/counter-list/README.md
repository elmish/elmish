This is a port of [Elm's counter list example](https://github.com/debois/elm-parts/blob/master/examples/2-counter-list.elm) implemented in F# and targeting Fable and React.
========

This is a simple demo of [Elmish](https://github.com/fable-compiler/fable-elmish).
It show how to use the [counter sample](https://github.com/fable-compiler/fable-elmish/tree/master/samples/react/counter) as a component for reuse.


## Build and running the sample
1. `pushd .. && yarn install && popd`
2. `dotnet restore`
3. `dotnet fable npm-run start`
4. open http://localhost:8080/webpack-dev-server/

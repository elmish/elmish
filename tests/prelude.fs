[<AutoOpen>]
module Prelude

open Fable.Core.Testing

let testList (name : string) (tests: seq<'b>) = name, tests
let testCase (msg : string) (test : obj -> unit) = msg, test

let ( =! ) expected actual: unit =
    Assert.AreEqual(actual, expected)

type Test = string * seq<string * seq<string * (obj -> unit)>>
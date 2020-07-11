module ElmishTests.Cmd

open Elmish

let tests : Test =
    testList "Cmd" [
        testList "OfAsyncImmediate" [
            testCase "exn calls onError" <| fun _ ->
                let task _ = async { failwith "expected" }
                let mutable errored = false
                let onError (ex:exn) = 
                    if ex.Message = "expected" then errored <- true
                Cmd.OfAsyncImmediate.attempt task () onError
                |> Cmd.exec ignore
                Timer.delay 1000 ignore
                true =! errored
        ]
    ]

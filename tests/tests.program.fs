module ElmishTests.Program

open Elmish

let tests : Test =
    testList "Program" [
        testList "Run" [
            // FAILS: https://github.com/fable-compiler/Fable/issues/2115
            // testCase "exn is handled" <| fun _ ->
            //     let task _ = async { failwith "triggering ofError" }
            //     let onError (ex:exn) = 
            //         printfn "Got: %A, throwing" ex
            //         failwith "expected"
            //     let cmd = Cmd.OfAsyncImmediate.attempt task () onError
                
            //     let mutable errored = false
            //     Program.mkProgram (fun _ -> (), cmd) (fun _ _ -> (), Cmd.none) (fun _ _ -> ())
            //     |> Program.withErrorHandler (fun (text,ex) -> 
            //         printfn "%A, %A" text ex
            //         if ex.Message = "expected" then errored <- true)
            //     |> Program.run

            //     Timer.delay 1000 ignore
            //     true =! errored
        ]
    ]

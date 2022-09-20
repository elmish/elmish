namespace Elmish

open System

#nowarn "44"
[<AutoOpen>]
module Obsolete =
    /// Cmd module for creating and manipulating commands
    [<RequireQualifiedAccess>]
    module Cmd =
        module OfFunc =
            /// Command to issue a specific message
            [<Obsolete("Use `either`,`attempt` or `perform` instead")>]
            let result (msg:'msg) : Cmd<'msg> =
                [fun dispatch -> dispatch msg]

        module OfAsyncWith =
            /// Command that will evaluate an async block to the message
            [<Obsolete("Use `either`,`attempt` or `perform` instead")>]
            let result (start: Async<unit> -> unit)
                    (task: Async<'msg>) : Cmd<'msg> =
                let bind dispatch =
                    async {
                        let! r = task
                        dispatch r
                    }
                [bind >> start]

        module OfAsync =
            /// Command that will evaluate an async block to the message
            [<Obsolete("Use `either`,`attempt` or `perform` instead")>]
            let inline result (task: Async<'msg>) : Cmd<'msg> =
                OfAsyncWith.result Cmd.OfAsync.start task

        module OfAsyncImmediate =
            /// Command that will evaluate an async block to the message
            [<Obsolete("Use `either`,`attempt` or `perform` instead")>]
            let inline result (task: Async<'msg>) : Cmd<'msg> =
                OfAsyncWith.result Async.StartImmediate task

    #if FABLE_COMPILER
        module OfPromise =
            /// Command to dispatch the `promise` result
            [<Obsolete("Use `either`,`attempt` or `perform` instead")>]
            let result (task: Fable.Core.JS.Promise<'msg>) : Cmd<'msg> =
                let bind dispatch =
                    task.``then`` dispatch
                    |> ignore
                [bind]
    #else
        open System.Threading.Tasks
        module OfTask =
            /// Command and map the task success
            [<Obsolete("Use `either`,`attempt` or `perform` instead")>]
            let inline result (task: Task<'msg>) : Cmd<'msg> =
                OfAsync.result (task |> Async.AwaitTask)
    #endif

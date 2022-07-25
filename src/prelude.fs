namespace Elmish

(**
Log
---------
Basic cross-platform logging API.

*)
module internal Log =

#if FABLE_COMPILER_DART
    open Fable.Core

    [<Global>]
    let print (info: obj): unit = ()

    let onError (text: string, ex: exn) = print $"{text} {ex}"
    let toConsole(text: string, o: #obj) = print $"{text} {o}"

#else
#if NETSTANDARD2_0
    let onError (text: string, ex: exn) = System.Diagnostics.Trace.TraceError("{0}: {1}", text, ex)
    let toConsole(text: string, o: #obj) = printfn "%s: %A" text o
#else
    let onError (text: string, ex: exn) = System.Console.Error.WriteLine("{0}: {1}", text, ex)
    let toConsole(text: string, o: #obj) = printfn "%s: %A" text o
#endif
#endif

// #if FABLE_COMPILER
// module internal Timer =
//     open System.Timers
//     let delay interval callback =
//         let t = new Timer(float interval, AutoReset = false)
//         t.Elapsed.Add callback
//         t.Enabled <- true
//         t.Start()
// #endif

(*** hide ***)
#I "../../src/bin/Debug/netstandard2.0"
#r "Fable.Core.dll"
#r "Fable.PowerPack.dll"
#r "Fable.Elmish.dll"

(**
*)
namespace Elmish

module internal Log =

#if FABLE_COMPILER
    open Fable.Core.JsInterop
    open Fable.Import.JS

    let onError (text: string, ex: exn) = console.error (text,ex)
    let toConsole(text: string, o: obj) = console.log(text, toJson o |> JSON.parse)

#else
#if NETSTANDARD2_0
    let onError (text: string, ex: exn) = System.Diagnostics.Trace.TraceError("{0}: {1}", text, ex)
    let toConsole(text: string, o: obj) = printfn "%s: %A" text o
#else
    let onError (text: string, ex: exn) = System.Console.Error.WriteLine("{0}: {1}", text, ex)
    let toConsole(text: string, o: obj) = printfn "%s: %A" text o
#endif
#endif

module internal Memo =
#if FABLE_COMPILER
    let once() =
        let mutable v:'a = Unchecked.defaultof<'a>
        fun (generator: unit ->'a) ->
            if obj.ReferenceEquals(v,Unchecked.defaultof<'a>) then
                v <- generator ()
            v            
#else
    let once() =
        let lockObject = obj()
        let mutable v:'a = Unchecked.defaultof<'a>
        fun (generator: unit ->'a) ->
            lock lockObject (fun () ->
                if (obj.ReferenceEquals(v, Unchecked.defaultof<'a>)) then
                    v <- generator ()
            )
            v
#endif

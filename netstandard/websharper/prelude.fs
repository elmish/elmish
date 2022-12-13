namespace Elmish

open WebSharper
open WebSharper.JavaScript

module internal Unchecked =
    [<Inline>]
    let defaultof<'T> = As<'T> null

module internal Array =
    [<Inline>]
    let zeroCreate size = As<'T[]> (Array.zeroCreate<obj> size)

module internal Log =
    let onError (text: string, ex: exn) = Console.Error (text,ex)
    let toConsole(text: string, o: #obj) = Console.Log(text,o)

[<Fable.Core.Erase>]
module Fable.Snabbdom.Internal

open Fable.AST
open Fable.AST.Fable.Util

type Emitter() =
    let (|CoreMeth|_|) coreMod meth = function
        | Fable.Value(Fable.ImportRef(meth', coreMod', Fable.CoreLib))
            when meth' = meth && coreMod' = coreMod -> Some CoreMeth
        | _ -> None
    let (|ArrayConst|_|) = function
        | Fable.Value(Fable.ArrayConst(Fable.ArrayValues vals, _)) -> Some vals
        | _ -> None
    let spread args =
        match args with
        | Fable.Apply(CoreMeth "List" "default",[],_,_,_) -> Fable.ArrayConst(Fable.ArrayValues [], Fable.Any) |> Fable.Value
        | Fable.Apply(CoreMeth "List" "ofArray",  [array], Fable.ApplyMeth,_,_) -> array
        | expr -> Fable.Value(Fable.ImportRef("toArray", "List", Fable.CoreLib))
    let createEl = makeImport "h" "snabbdom"

    member x.Tagged(_com: Fable.ICompiler, i: Fable.ApplyInfo, tag: string) =
        let args =
            let tag = Fable.Value(Fable.StringConst tag)
            match i.args with
            | [props; children] -> [tag; props; spread children]
            | [props] -> [tag; props]
            | _ -> failwith "Unexpected arguments"
        _com.AddLog(Fable.Info "A message")
        Fable.Apply(createEl, args, Fable.ApplyMeth, i.returnType, i.range)
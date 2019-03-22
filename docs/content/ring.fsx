﻿(*** hide ***)
#I "../../src/bin/Release/netstandard2.0"
#r "Fable.Core.dll"
#r "Fable.PowerPack.dll"
#r "Fable.Elmish.dll"

(**
*)
namespace Elmish
open System

[<Struct>]
type internal RingState<'item> =
    | Writable of wx:'item array * ix:int
    | ReadWritable of rw:'item array * wix:int * rix:int

type internal RingBuffer<'item>(size) =
    let doubleSize ix (items: 'item array) =
        seq { yield! items |> Seq.skip ix
              yield! items |> Seq.take ix
              for _ in 0..items.Length do
                yield Unchecked.defaultof<'item> }
        |> Array.ofSeq

    let mutable state : 'item RingState =
        Writable (Array.zeroCreate (max size 10),0)

    member __.Pop() =
        match state with
        | ReadWritable (items,wix,rix) ->
            let rix' = (rix + 1) % items.Length
            match rix' = wix with
            | true -> 
                state <- Writable(items,wix)
            | _ ->
                state <- ReadWritable(items,wix,rix')
            Some items.[rix]
        | _ ->
            None

    member __.Push (item:'item) =
        match state with
        | Writable (items,ix) ->
            items.[ix] <- item
            let wix = (ix + 1) % items.Length
            state <- ReadWritable(items,wix,ix)
        | ReadWritable (items,wix,rix) ->
            items.[wix] <- item
            let wix' = (wix + 1) % items.Length
            match wix' = rix with
            | true -> 
                let items = items |> doubleSize rix                                
                state <- ReadWritable(items,wix',0)
            | _ -> 
                state <- ReadWritable(items,wix',rix)

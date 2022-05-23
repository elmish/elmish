namespace Elmish
open System

[<Struct>]
type internal RingState<'item> =
    | Writable of wx:'item option array * ix:int
    | ReadWritable of rw:'item option array * wix:int * rix:int

type internal RingBuffer<'item>(size) =
    let doubleSize ix (items: 'item option array) =
        seq { yield! items |> Seq.skip ix
              yield! items |> Seq.take ix
              for _ in 0..items.Length do
                yield None }
        |> Array.ofSeq

    let mutable state : 'item RingState =
        Writable (Array.zeroCreate (max size 10), 0)

    member __.Pop() =
        match state with
        | ReadWritable (items, wix, rix) ->
            let rix' = (rix + 1) % items.Length
            match rix' = wix with
            | true -> 
                state <- Writable(items, wix)
            | _ ->
                state <- ReadWritable(items, wix, rix')
            items.[rix]
        | _ ->
            None

    member __.Push (item:'item) =
        match state with
        | Writable (items, ix) ->
            items.[ix] <- Some item
            let wix = (ix + 1) % items.Length
            state <- ReadWritable(items, wix, ix)
        | ReadWritable (items, wix, rix) ->
            items.[wix] <- Some item
            let wix' = (wix + 1) % items.Length
            match wix' = rix with
            | true -> 
                state <- ReadWritable(items |> doubleSize rix, items.Length, 0)
            | _ -> 
                state <- ReadWritable(items, wix', rix)
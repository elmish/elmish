namespace Elmish

open System

/// SubId - ID for live subscriptions, alias for string
type SubId = string

/// Subscription - Generates new messages when running
type Sub<'msg> = { Subscribe: Dispatch<'msg> -> IDisposable }

module Sub =

    /// When emitting the message, map to another type
    let map (f: 'a -> 'msg) (sub: Sub<'a>) : Sub<'msg> =
        { Subscribe = fun dispatch -> sub.Subscribe (f >> dispatch) }

module Subs =

    /// When emitting the message, map to another type
    let map (f: 'a -> 'msg) (subs: (SubId * Sub<'a>) list) : (SubId * Sub<'msg>) list =
        List.map (fun (subId, sub) -> subId, Sub.map f sub) subs

    module Internal =

        let tryStop onError (subId, sub: IDisposable) =
            try
                sub.Dispose()
            with ex ->
                onError (sprintf "Error stopping subscription: %s" subId, ex)

        let tryStart onError dispatch (subId, newSub: Sub<'msg>) =
            try
                Some (subId, newSub.Subscribe dispatch)
            with ex ->
                onError (sprintf "Error starting subscription: %s" subId, ex)
                None

        let change onError dispatch (toStop, toKeep, toStart) =
            toStop |> List.iter (tryStop onError)
            let started = toStart |> List.choose (tryStart onError dispatch)
            List.append toKeep started

        let recalc (subs: (SubId * IDisposable) list) (newSubs: (SubId * Sub<'msg>) list) =
            let keys = subs |> List.map fst |> Set.ofList
            let newKeys = newSubs |> List.map fst |> Set.ofList
            if keys = newKeys then
                [], subs, []
            else
                let toKeep, toStop = subs |> List.partition (fun (k, _) -> Set.contains k newKeys)
                let toStart = newSubs |> List.filter (fun (k, _) -> not (Set.contains k keys))
                toStop, toKeep, toStart

        let stop onError subs =
            subs |> List.iter (tryStop onError)

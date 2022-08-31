namespace Elmish

open System

/// SubId - Subscription ID, alias for string
type SubId = string

/// Subscription - Generates new messages when running
type Sub<'msg> = { Start: SubId -> Dispatch<'msg> -> IDisposable }

module Sub =

    module One =

        /// When emitting the message, map to another type
        let map (f: 'a -> 'msg) (sub: Sub<'a>) : Sub<'msg> =
            { Start = fun subId dispatch ->
                sub.Start subId (f >> dispatch) }

    /// None - no subscriptions, also known as `[]`
    let none : Cmd<'msg> =
        []

    /// Aggregate multiple subscriptions
    let batch (subs: #seq<Sub<'msg> list>) : Sub<'msg> list =
        subs |> List.concat

    /// When emitting the message, map to another type.
    /// To avoid ID conflicts with other components, scope SubIds with a prefix.
    let map (idPrefix: string) (f: 'a -> 'msg) (subs: (SubId * Sub<'a>) list)
        : (SubId * Sub<'msg>) list =
        List.map (fun (subId, sub) -> idPrefix + subId, One.map f sub) subs

    module Internal =

        module Fx =

            let warnDupe onError subId =
                let ex = exn "Duplicate SubId"
                onError ("Duplicate SubId: " + subId, ex)

            let tryStop onError (subId, sub: IDisposable) =
                try
                    sub.Dispose()
                with ex ->
                    onError ("Error stopping subscription: " + subId, ex)

            let tryStart onError dispatch (subId, newSub: Sub<'msg>) =
                try
                    Some (subId, newSub.Start subId dispatch)
                with ex ->
                    onError ("Error starting subscription: " + subId, ex)
                    None

            let stop onError subs =
                subs |> List.iter (tryStop onError)

            let change onError dispatch (dupes, toStop, toKeep, toStart) =
                dupes |> List.iter (warnDupe onError)
                toStop |> List.iter (tryStop onError)
                let started = toStart |> List.choose (tryStart onError dispatch)
                List.append toKeep started

        module NewSubs =

            let (_dupes, _newKeys, _newSubs) as init =
                List.empty, Set.empty, List.empty

            let update ((subId, _) as newSub) (dupes, newKeys, newSubs) =
                if Set.contains subId newKeys then
                    subId :: dupes, newKeys, newSubs
                else
                    dupes, Set.add subId newKeys, newSub :: newSubs

            let calculate subs =
                List.foldBack update subs init

        let empty = List.empty<SubId * IDisposable>

        let diff (activeSubs: (SubId * IDisposable) list) (subs: (SubId * Sub<'msg>) list) =
            let keys = activeSubs |> List.map fst |> Set.ofList
            let dupes, newKeys, newSubs = NewSubs.calculate subs
            if keys = newKeys then
                dupes, [], activeSubs, []
            else
                let toKeep, toStop = activeSubs |> List.partition (fun (k, _) -> Set.contains k newKeys)
                let toStart = newSubs |> List.filter (fun (k, _) -> not (Set.contains k keys))
                dupes, toStop, toKeep, toStart

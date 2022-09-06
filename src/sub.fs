namespace Elmish

open System

/// SubId - Subscription ID, alias for string list
type SubId = string list

/// Subscription - Generates new messages when running
type Sub<'msg> =
    { SubId: SubId
      Start: Dispatch<'msg> -> IDisposable }

module Sub =

    /// None - no subscriptions, also known as `[]`
    let none : Sub<'msg> list =
        []

    /// Aggregate multiple subscriptions
    let batch (subs: #seq<Sub<'msg> list>) : Sub<'msg> list =
        subs |> List.concat

    /// When emitting the message, map to another type.
    /// To avoid ID conflicts with other components, scope SubIds with a prefix.
    let map (idPrefix: string) (f: 'a -> 'msg) (subs: Sub<'a> list)
        : Sub<'msg> list =
        subs |> List.map (fun sub ->
            { SubId = idPrefix :: sub.SubId
              Start = fun dispatch -> sub.Start (f >> dispatch) })

    module Internal =

        module SubId =

            let toString (subId: SubId) =
                String.Join("/", subId)

        module Fx =

            let warnDupe onError subId =
                let ex = exn "Duplicate SubId"
                onError ("Duplicate SubId: " + SubId.toString subId, ex)

            let tryStop onError (subId, sub: IDisposable) =
                try
                    sub.Dispose()
                with ex ->
                    onError ("Error stopping subscription: " + SubId.toString subId, ex)

            let tryStart onError dispatch (subId, start) : (SubId * IDisposable) option =
                try
                    Some (subId, start dispatch)
                with ex ->
                    onError ("Error starting subscription: " + SubId.toString subId, ex)
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

            let update ({SubId = subId; Start = start}) (dupes, newKeys, newSubs) =
                if Set.contains subId newKeys then
                    subId :: dupes, newKeys, newSubs
                else
                    dupes, Set.add subId newKeys, (subId, start) :: newSubs

            let calculate subs =
                List.foldBack update subs init

        let empty = List.empty<SubId * IDisposable>

        let diff (activeSubs: (SubId * IDisposable) list) (subs: Sub<'msg> list) =
            let keys = activeSubs |> List.map fst |> Set.ofList
            let dupes, newKeys, newSubs = NewSubs.calculate subs
            if keys = newKeys then
                dupes, [], activeSubs, []
            else
                let toKeep, toStop = activeSubs |> List.partition (fun (k, _) -> Set.contains k newKeys)
                let toStart = newSubs |> List.filter (fun (k, _) -> not (Set.contains k keys))
                dupes, toStop, toKeep, toStart

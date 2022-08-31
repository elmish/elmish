module Elmish.SubTests

open Elmish
open NUnit.Framework
open System

[<TestFixture>]
type GetChangesBehavior() =
    // data
    let stop = {new IDisposable with member _.Dispose() = () }
    let sub = { Start = fun _ (_: Dispatch<obj>) -> stop }
    let dupeSub = { Start = fun _ (_: Dispatch<obj>) -> stop }
    let newId i = sprintf "sub/%i" i
    let gen idRangeStart idRangeEnd second =
        let count = idRangeEnd + 1 - idRangeStart
        List.init count (fun i -> newId (idRangeStart + i), second)
    // helpers
    let toKeys keyValueList =
        List.map fst keyValueList
    let toIds (dupes, toStop, toKeep, toStart) =
        dupes, toKeys toStop, toKeys toKeep, toKeys toStart
    let toIds2 (dupes, toStop, toKeep, toStart) =
        toKeys dupes, toKeys toStop, toKeys toKeep, toKeys toStart
    let run = Sub.Internal.diff
    let eq expected actual (message: string) =
        Assert.IsTrue((toIds2 expected = toIds actual), message)

    [<Test>]
    member _.``no changes when subs and active subs are the same`` () =
        let activeSubs = gen 0 6 stop
        let subs = gen 0 6 sub
        let expected = [], [], activeSubs, []
        let actual = run activeSubs subs
        eq expected actual "incorrect changes, expecting none"

    [<Test>]
    member _.``active subs are stopped when not found in subs`` () =
        let activeSubs = gen 0 6 stop
        let subs = gen 3 6 sub
        let expected = [], activeSubs[0..2], activeSubs[3..6], []
        let actual = run activeSubs subs
        eq expected actual "incorrect stopped subs"

    [<Test>]
    member _.``subs are started when not found in active subs`` () =
        let activeSubs = gen 0 2 stop
        let subs = gen 0 6 sub
        let expected = [], [], activeSubs, subs[3..6]
        let actual = run activeSubs subs
        eq expected actual "incorrect started subs"

    [<Test>]
    member _.``subs are started and stopped when subs has new ids and omits old ids`` () =
        let activeSubs = gen 0 6 stop
        let tmp = gen 0 9 sub
        let subs = tmp[3..9]
        let expected = [], activeSubs[0..2], activeSubs[3..6], tmp[7..9]
        let actual = run activeSubs subs
        eq expected actual "incorrect started and/or stopped subs"

    [<Test>]
    member _.``dupe subs are detected even when there are no changes`` () =
        let activeSubs = gen 0 6 stop
        let subs = List.concat [gen 2 2 dupeSub; gen 2 2 dupeSub; gen 0 6 sub]
        let expected = subs[0..1], [], activeSubs, []
        let actual = run activeSubs subs
        eq expected actual "incorrect dupes"

    [<Test>]
    member _.``last dupe wins when starting new subs`` () =
        let activeSubs = []
        let dupeSubId = newId 2
        let subs = List.concat [gen 2 2 dupeSub; gen 2 2 dupeSub; gen 0 6 sub]
        let expected = subs[0..1], [], activeSubs, subs[2..8]
        let ((dupes, _, _, toStart) as actual) = run activeSubs subs
        let startId, startDupe = toStart[2]
        Assert.IsTrue(List.forall (fun subId -> dupeSubId = subId) dupes, "Dupes have wrong ID")
        Assert.IsTrue((dupeSubId = startId), "Started dupe has wrong ID")
        Assert.IsTrue(Object.ReferenceEquals(sub, startDupe), "Started dupe is the wrong one")
        Assert.IsFalse(Object.ReferenceEquals(dupeSub, startDupe), "Started dupe is the wrong one")
        eq expected actual "incorrect dupes and/or started subs"




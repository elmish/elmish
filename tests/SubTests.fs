module Elmish.SubTests

open Elmish
open NUnit.Framework
open Swensen.Unquote
open System

// TIL: Each use of a let fn binding creates a new FSharpFunc object.
// Meaning reference equality with let fn binding is always false.
// As record properties, these become concrete objects, ref equality works.
type SubContainer =
    { Sub: SubId -> Dispatch<obj> -> IDisposable
      Dupe: SubId -> Dispatch<obj> -> IDisposable }

[<TestFixture>]
type DiffBehavior() =
    // data
    let stop = {new IDisposable with member _.Dispose() = () }
    let sub = {Sub = (fun _ _  -> stop); Dupe = (fun _ _ -> stop) }
    let newId i = ["sub"; string i]
    let gen idRangeStart idRangeEnd second =
        let count = idRangeEnd + 1 - idRangeStart
        List.init count (fun i -> newId (idRangeStart + i), second)
    let genSub idRangeStart idRangeEnd whichSub =
        let count = idRangeEnd + 1 - idRangeStart
        List.init count (fun i ->
            { SubId = newId (idRangeStart + i)
              Start = whichSub })
    // helpers
    let toKeys keyValueList =
        List.map fst keyValueList
    let toKeys2 subs =
        subs |> List.map (fun sub -> sub.SubId)
    let toIds (dupes, toStop, toKeep, toStart) =
        {| Dupes = dupes; ToStop = toKeys toStop; ToKeep = toKeys toKeep; ToStart = toKeys toStart |}
    let toIds2 (dupes, toStop, toKeep, toStart) =
        {| Dupes = toKeys2 dupes; ToStop = toKeys toStop; ToKeep = toKeys toKeep; ToStart = toKeys2 toStart |}
    let run = Sub.Internal.diff
    let eq expected actual =
        toIds2 expected =! toIds actual

    [<Test>]
    member _.``no changes when subs and active subs are the same`` () =
        let activeSubs = gen 0 6 stop
        let subs = genSub 0 6 sub.Sub
        let expected = [], [], activeSubs, []
        let actual = run activeSubs subs
        eq expected actual

    [<Test>]
    member _.``active subs are stopped when not found in subs`` () =
        let activeSubs = gen 0 6 stop
        let subs = genSub 3 6 sub.Sub
        let expected = [], activeSubs[0..2], activeSubs[3..6], []
        let actual = run activeSubs subs
        eq expected actual

    [<Test>]
    member _.``subs are started when not found in active subs`` () =
        let activeSubs = gen 0 2 stop
        let subs = genSub 0 6 sub.Sub
        let expected = [], [], activeSubs, subs[3..6]
        let actual = run activeSubs subs
        eq expected actual

    [<Test>]
    member _.``subs are started and stopped when subs has new ids and omits old ids`` () =
        let activeSubs = gen 0 6 stop
        let tmp = genSub 0 9 sub.Sub
        let subs = tmp[3..9]
        let expected = [], activeSubs[0..2], activeSubs[3..6], tmp[7..9]
        let actual = run activeSubs subs
        eq expected actual

    [<Test>]
    member _.``dupe subs are detected even when there are no changes`` () =
        let activeSubs = gen 0 6 stop
        let subs = Sub.batch [genSub 2 2 sub.Dupe; genSub 2 2 sub.Dupe; genSub 0 6 sub.Sub]
        let expected = subs[0..1], [], activeSubs, []
        let actual = run activeSubs subs
        eq expected actual

    [<Test>]
    member _.``last dupe wins when starting new subs`` () =
        let activeSubs = []
        let dupeSubId = newId 2
        let subs = List.concat [genSub 2 2 sub.Dupe; genSub 2 2 sub.Dupe; genSub 0 6 sub.Sub]
        let expected = subs[0..1], [], activeSubs, subs[2..8]
        let ((dupes, _, _, toStart) as actual) = run activeSubs subs
        let startId, startDupe = toStart[2]
        Assert.IsTrue(List.forall (fun subId -> dupeSubId = subId) dupes, "Dupes have wrong ID")
        Assert.IsTrue((dupeSubId = startId), "Started dupe has wrong ID")
        Assert.IsTrue(Object.ReferenceEquals(sub.Sub, startDupe), "Started dupe is the wrong one")
        Assert.IsFalse(Object.ReferenceEquals(sub.Dupe, startDupe), "Started dupe is the wrong one")
        eq expected actual


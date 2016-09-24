namespace Elmish

//---------------------------------------------------
//PromisesExt (by Dave)
// pilfered from ionide-helpers
//---------------------------------------------------
module Promise =
    open System
    open Fable.Core
    open Fable.Import
    open Fable.Import.JS
    
    /// <summary>
    /// Creates new promise. 
    /// Constuctor function "body" is called with two arguments: 
    /// "resolve" - moves promise to fulfilled state, using provided argument as result
    /// "reject" - moves promise to rejected state
    /// </summary>
    let create (body: ('T->unit) -> (obj->unit) -> unit) =
        Promise.Create(fun (resolverFunc : Func<U2<_, _>, _>) (rejectorFunc : Func<_,_>) ->
            body (fun v -> resolverFunc.Invoke(U2.Case1 v)) (fun e -> rejectorFunc.Invoke e)) 

    /// <summary>
    /// Standard map implementation.
    /// </summary>
    let map (a : 'T -> 'R) (pr : Promise<'T>) : Promise<'R> =
        pr.``then``(   
            unbox<Func<'T, U2<'R, PromiseLike<'R>>>> a,
            unbox<Func<obj,unit>> None
        )

    /// <summary>
    /// Standard bind implementation.
    /// </summary>
    let bind (a : 'T -> Promise<'R>) (pr : Promise<'T>) : Promise<'R> =
        pr.``then``(   
            unbox<Func<'T, U2<'R, PromiseLike<'R>>>> a,
            unbox<Func<obj,unit>> None
        )

    /// <summary>
    /// Bind for rejected promise.
    /// </summary>
    let catch (a : obj -> Promise<'R>) (pr : Promise<'T>) : Promise<'R> =
        pr.``then``(   
            unbox<Func<'T, U2<'R, PromiseLike<'R>>>> None,
            unbox<Func<obj,U2<'R, PromiseLike<'R>>>> a
        )

    /// <summary>
    /// Combination of bind and catch methods. If promise in fulfilled state - a is bound, if in rejected state - then b.
    /// </summary>
    let either (a : 'T -> Promise<'R>) (b: obj -> Promise<'R>) (pr : Promise<'T>) : Promise<'R> =
        pr.``then``(
            unbox<Func<'T, U2<'R, PromiseLike<'R>>>> a,
            unbox<Func<obj,U2<'R, PromiseLike<'R>>>> b
        )

    /// <summary>
    /// Creates promise (in pending state) from the supplied value.
    /// </summary>
    let lift<'T> (a : 'T) : Promise<'T> =
        Promise.resolve(U2.Case1 a)

    /// <summary>
    /// Creates promise (in rejected state) with supplied reason.
    /// </summary>
    let reject<'T> reason : Promise<'T> =
        Promise.reject<'T> reason

    /// <summary>
    /// Allows handing promise which is in fulfilled state. 
    /// Can be used for side-effects. 
    /// </summary>
    let onSuccess (a : 'T -> unit) (pr : Promise<'T>) : Promise<'T> =
        pr.``then``(   
            unbox<Func<'T, U2<'T, PromiseLike<'T>>>> (fun value -> a value; value),
            unbox<Func<obj,unit>> None
        )

    /// <summary>
    /// Allows handing promise which is in rejected state. Propagates rejected promise, to allow chaining.
    /// Can be used for side-effects. 
    /// </summary>
    let onFail (a : obj -> unit) (pr : Promise<'T>) : Promise<'T> =
        pr.catch (unbox<Func<obj, U2<'T, PromiseLike<'T>>>> (fun reason -> a reason |> ignore; reject reason))

    type PromiseBuilder() =
        member inline x.Bind(m,f) = bind f m
        member inline x.Return(a) = lift a
        member inline x.ReturnFrom(a) = a
        member inline x.Zero() = Fable.Import.JS.Promise.resolve()

[<AutoOpen>]
module PromiseBuilderImp =
    let promise = Promise.PromiseBuilder()


/// Elmish Cmd extension for promises
module Cmd =
    /// Command to call `promise` block and map the results
    let ofPromise (task:unit->Fable.Import.JS.Promise<_>) (ofSuccess:_->'msg) (ofError:_->'msg) : Cmd<'msg> =
        let bind (dispatch:'msg -> unit) =
            task()
            |> Promise.onSuccess (ofSuccess >> dispatch)
            |> Promise.onFail (ofError >> dispatch)
            |> ignore
        [bind]

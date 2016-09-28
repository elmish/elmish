namespace Elmish

type Result<'s, 'f> =
    | Ok of 's
    | Error of 'f

    static member (>>=) (r: Result<'t, 'e>, f: 't -> Result<'u, 'e>) : Result<'u, 'e> =
        match r with
        | Error e -> Error e
        | Ok v -> f v

    static member (<^>) (f: 't -> 'u, r: Result<'t, 'e>) : Result<'u, 'e> =
        r >>= (f >> Ok)

    static member (<*>) (f: Result<('t -> 'u), 'e>, r: Result<'t, 'e>) : Result<'u, 'e> =
        f >>= fun f -> f <^> r

[<RequireQualifiedAccess>]
module Result =

    let unit (v: 't) : Result<'t, 'e> =
        Ok v

    let map (f: 't -> 'u) (r: Result<'t, 'e>) : Result<'u, 'e> =
        f <^> r

    let apply (f: Result<('t -> 'u), 'e>) (r: Result<'t, 'e>) : Result<'u, 'e> =
        f <*> r

    let bind (f: 't -> Result<'u, 'e>) (r: Result<'t, 'e>) : Result<'u, 'e> =
        r >>= f

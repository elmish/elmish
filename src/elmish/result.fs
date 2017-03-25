namespace Elmish

[<RequireQualifiedAccess>]
module Result =

    let unit (v: 't) : Result<'t, 'e> =
        Ok v

    let bind (f: 't -> Result<'u, 'e>) (r: Result<'t, 'e>) : Result<'u, 'e> =
        match r with
        | Error e -> Error e
        | Ok v -> f v

    let map (f: 't -> 'u) (r: Result<'t, 'e>) : Result<'u, 'e> =
        bind (f >> Ok) r

    let apply (a: Result<('t -> 'u), 'e>) (r: Result<'t, 'e>) : Result<'u, 'e> =
        bind (fun f -> map f r) a 

    let toOption =
        function
        | Ok v -> Some v
        | _ -> None


[<RequireQualifiedAccess>]
module Option =

    let tuple a b =
        match (a,b) with
        | Some a, Some b -> Some (a,b)
        | _ -> None

    let ofFunc f arg =
        try
            Some (f arg)
        with _ ->
            None
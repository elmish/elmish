namespace Elmish.Browser


[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
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
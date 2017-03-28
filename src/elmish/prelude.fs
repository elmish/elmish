namespace Elmish

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Tuple =
    let inline map (mapFst:'a->'b) (mapSnd:'c->'d) (x:'a, y:'c): 'b * 'd =
        (mapFst x, mapSnd y)

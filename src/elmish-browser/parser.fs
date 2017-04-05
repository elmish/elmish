(* This port of the Elm library helps you turn URLs into nicely structured data.
It is designed to be used with Browser.Navigation module to help folks create
single-page applications (SPAs) where you manage browser navigation yourself.
*)

module Elmish.Browser.UrlParser

type State<'v> =
  { visited : string list
    unvisited : string list
    args : Map<string,string>
    value : 'v }

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module State =
  let mkState visited unvisited args value =
        { visited = visited
          unvisited = unvisited
          args = args
          value = value }
  let map f { visited = visited; unvisited = unvisited; args = args; value = value } =
        { visited = visited
          unvisited = unvisited
          args = args
          value = f value }


/// Turn URLs like `/blog/42/cat-herding-techniques` into nice data.
type Parser<'a,'b> = State<'a> -> State<'b> list


// PARSE SEGMENTS

(* Create a custom path segment parser. Here is how it is used to define the
`i32` and `str` parsers:
    i32 =
      custom "NUMBER" string.toInt
    str =
      custom "string" Ok
You can use it to define something like “only CSS files” like this:
    css =
      custom "CSS_FILE" <| fun segment ->
        if string.endsWith ".css" then
          Ok segment
        else
          Error "Does not end with .css"
*)
let custom tipe stringToSomething : Parser<_,_> =
    let inner { visited = visited; unvisited = unvisited; args = args; value = value } =
        match unvisited with
        | [] -> []
        | next :: rest ->
            match stringToSomething next with
            | Ok nextValue ->
                [ State.mkState (next :: visited) rest args (value nextValue) ]

            | Error msg ->
                []
    inner


(* Parse a segment of the path as a `string`.
    parsePath string location
    /alice/  ==>  Some "alice"
    /bob     ==>  Some "bob"
    /42/     ==>  Some "42"
*)
let str state =
    custom "string" Ok state


(* Parse a segment of the path as an `Int`.
    parsePath int location
    /alice/  ==>  None
    /bob     ==>  None
    /42/     ==>  Some 42
*)
let i32 state =
    custom "i32" (System.Int32.Parse >> Ok) state


(* Parse a segment of the path if it matches a given string.
    s "blog"  // can parse /blog/
              // but not /glob/ or /42/ or anything else
*)
let s str : Parser<_,_> =
    let inner { visited = visited; unvisited = unvisited; args = args; value = value } =
        match unvisited with
        | [] -> []
        | next :: rest ->
            if next = str then
                [ State.mkState (next :: visited) rest args value ]
            else
                []
    inner


// COMBINING PARSERS

(* Parse a path with multiple segments.
    parsePath (s "blog" </> int32) location
    /blog/35/  ==>  Some 35
    /blog/42   ==>  Some 42
    /blog/     ==>  None
    /42/       ==>  None
    parsePath (s "search" </> string) location
    /search/cats/  ==>  Some "cats"
    /search/frog   ==>  Some "frog"
    /search/       ==>  None
    /cats/         ==>  None
*)
//(</>) : Parser a b -> Parser b c -> Parser a c
let inline (</>) (parseBefore:Parser<_,_>) (parseAfter:Parser<_,_>) =
  fun state ->
    List.collect parseAfter (parseBefore state)


(* Transform a path parser.
    type Comment = { author : string, id : Int }
    rawComment =
      s "user" </> str </> s "comments" </> i32
    comment =
      map Comment rawComment
    parsePath comment location
    /user/bob/comments/42  ==>  Some { author = "bob", id = 42 }
    /user/tom/comments/35  ==>  Some { author = "tom", id = 35 }
    /user/sam/             ==>  None
*)
let map (subValue:'a) (parse:Parser<'a,'b>) : Parser<'b->'c,'c> =
    let inner { visited = visited; unvisited = unvisited; args = args; value = value } =
        List.map (State.map value) 
        <| parse { visited = visited
                   unvisited = unvisited
                   args = args
                   value = subValue }
    inner
  


(* Try a bunch of different path parsers.
    type Route
      = Search string
      | Blog Int
      | User string
      | Comment string Int
    route =
      oneOf
        [ map Search  (s "search" </> string)
          map Blog    (s "blog" </> int32)
          map User    (s "user" </> string)
          map Comment (s "user" </> string </> "comments" </> int32)
        ]
    parsePath route location
    /search/cats           ==>  Some (Search "cats")
    /search/               ==>  None
    /blog/42               ==>  Some (Blog 42)
    /blog/cats             ==>  None
    /user/sam/             ==>  Some (User "sam")
    /user/bob/comments/42  ==>  Some (Comment "bob" 42)
    /user/tom/comments/35  ==>  Some (Comment "tom" 35)
    /user/                 ==>  None
*)
let oneOf parsers state =
    List.collect (fun parser -> parser state) parsers


(* A parser that does not consume any path segments.
    type BlogRoute = Overview | Post int
    blogRoute =
      oneOf
        [ map Overview top
          map Post  (s "post" </> int32) ]
    parsePath (s "blog" </> blogRoute) location
    /blog/         ==>  Some Overview
    /blog/post/42  ==>  Some (Post 42)
*)
let top state=
    [state]



// QUERY PARAMETERS

/// Turn query parameters like `?name=tom&age=42` into nice data.
type QueryParser<'a,'b> = State<'a> -> State<'b> list


(* Parse some query parameters.
    type Route = BlogList (Option string) | BlogPost Int
    route =
      oneOf
        [ map BlogList (s "blog" <?> stringParam "search")
          map BlogPost (s "blog" </> i32)
        ]
    parsePath route location
    /blog/              ==>  Some (BlogList None)
    /blog/?search=cats  ==>  Some (BlogList (Some "cats"))
    /blog/42            ==>  Some (BlogPost 42)
*)
let inline (<?>) (parser:Parser<_,_>) (queryParser:QueryParser<_,_>) : Parser<_,_> =
    fun state ->
        List.collect queryParser (parser state)

(* Create a custom query parser. You could create parsers like these:
    jsonParam : string -> Decoder a -> QueryParser (Option a -> b) b
    enumParam : string -> Map<string,a> -> QueryParser (Option a -> b) b
*)
let customParam (key: string) (func:string option -> _) : QueryParser<_,_> =
    let inner { visited = visited; unvisited = unvisited; args = args; value = value } =
        [ State.mkState visited unvisited args (value (func (Map.tryFind key args))) ]
    inner


(* Parse a query parameter as a `string`.
    parsePath (s "blog" <?> stringParam "search") location
    /blog/              ==>  Some (Overview None)
    /blog/?search=cats  ==>  Some (Overview (Some "cats"))
*)
let stringParam name =
    customParam name id

let internal intParamHelp =
    Option.bind 
        (fun value ->
            match System.Int32.TryParse value with
            | (true,x) -> Some x
            | _ -> None)

(* Parse a query parameter as an `Int`. Option you want to show paginated
search results. You could have a `start` query parameter to say which result
should appear first.
    parsePath (s "results" <?> intParam "start") location
    /results           ==>  Some None
    /results?start=10  ==>  Some (Some 10)
*)
let intParam name =
    customParam name intParamHelp


// PARSER HELPERS

let rec internal parseHelp states =
    match states with
    | [] ->
        None
    | state :: rest ->
        match state.unvisited with
        | [] ->
            Some state.value
        | [""] ->
            Some state.value
        | _ ->
            parseHelp rest

let internal splitUrl (url:string) =
    match List.ofArray <| url.Split([|'/'|]) with
    | "" :: segments ->
        segments
    | segments ->
        segments

let internal parse (parser:Parser<'a->'a,'a>) url args =
    { visited = []
      unvisited = splitUrl url
      args = args
      value = id }
    |> parser
    |> parseHelp

open Fable.Import

let internal toKeyValuePair (segment:string) =
    match segment.Split('=') with
    | [| key; value |] ->
        Option.tuple (Option.ofFunc JS.decodeURI key) (Option.ofFunc JS.decodeURI value)
    | _ -> None


let internal parseParams (querystring:string) =
    querystring.Substring(1).Split('&')
    |> Seq.map toKeyValuePair
    |> Seq.choose id
    |> Map.ofSeq

open Fable.Import.Browser

(* Parse based on `location.pathname` and `location.search`. This parser
ignores the hash entirely.
*)
let parsePath (parser:Parser<_,_>) (location:Location) =
    parse parser location.pathname (parseParams location.search)

(* Parse based on `location.hash` and `location.search`. This parser
ignores the normal path entirely.
*)
let parseHash (parser:Parser<_,_>) (location:Location) =
    parse parser (location.hash.Substring 1) (parseParams location.search)


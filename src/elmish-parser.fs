namespace Elmish

(* This library helps you turn URLs into nicely structured data.
It is designed to be used with `elm-lang/navigation` to help folks create
single-page applications (SPAs) where you manage browser navigation yourself.
*)

module UrlParser = 

    type Chunks = { seen : string list; rest : string list }

    (* A `Parser` is a way of turning a URL like `/blog/42/cat-herding-techniques`
    into structured data.
    The two type variables can be a bit tricky to understand. I think the best way
    to proceed is to just start using it. You can go far if you just assume it will
    do the intuitive thing.
    **Note:** If you *insist* on digging deeper, I recommend figuring out the type
    of `int </> int` based on the type signatures for `int` and `</>`. You may be
    able to just know based on intuition, but instead, you should figure out
    exactly how every type variable gets unified. It is pretty cool! From there,
    maybe check out the implementation a bit.
    *)
    type Parser<'formatter,'result> = Chunks -> 'formatter -> Result<Chunks*'result, string>


    (* Actually run a parser. For example, if we want to handle blog posts with
    an ID number and a name, we might write something like this:
        blog = s "blog" </> int </> string
        result = parse (,) blog "blog/42/cat-herding-techniques"
        -- result == OK (42, "cat-herding-techniques")
    Notice that we use the `(,)` function for building tuples as the first argument
    to `parse`. The `blog` parser requires a formatter of type `(Int -> String -> a)`
    so we need to provide that to actually run things.
    **Note:** The error messages are intended to be fairly helpful. They are
    nice for debugging during development, but probably too detailed to show
    directly to users.
    *)
    let parse (input:'f) (actuallyParse:Parser<'f,'a>) (url:string) : Result<'a,string> =
        actuallyParse {seen = []; rest = url.Split '/' |> List.ofArray } input
        |> function
           | Error msg ->
                Error msg

           | Ok ({rest = rest}, result) ->
                match rest with
                | [] -> Ok result
                | [""] -> Ok result
                | _ ->
                    Error <| "The parser worked, but /" + System.String.Join("/", rest) + " was left over."



    // PARSE SEGMENTS
    (* A parser that matches *exactly* the given string. So the following parser
    will match the URL `/hello/world` and nothing else:
        helloWorld = s "hello" </> s "world"
    *)
    let s (str : string) : Parser<_,_> =
        fun {seen = seen;rest = rest} result ->
            match rest with
            | [] ->
                Error ("Got to the end of the URL but wanted /" + str)

            | chunk :: remaining ->
                if chunk = str then
                    Ok ( {seen=chunk :: seen; rest=remaining}, result )
                else
                    Error ("Wanted /" + str + " but got /" + System.String.Join("/",rest))


    (* Create a custom segment parser. The `i32` and `str` parsers are actually
    defined with it like this:
        str = custom "string" Ok
        i32 = custom "NUMBER" String.toInt
    The first argument is to help with error messages. It lets us say something
    like, &ldquo;Got to the end of the URL but wanted /STRING&rdquo; instead of
    something totally nonspecific. The second argument lets you process the URL
    segment however you want.
    An example usage would be a parser that only accepts segments with a particular
    file extension. So stuff like this:
        css = custom "FILE.css" <| fun str ->
                if String.endsWith ".css" str then
                    Ok str
                else
                    Error "Need something that ends with .css"
    *)
    let custom tipe stringToSomething {seen=seen; rest= rest} fmt =
        match rest with
        | [] ->
            Error ("Got to the end of the URL but wanted /" + tipe)

        | chunk :: remaining ->
            match stringToSomething chunk with
            | Ok something ->
                Ok ( {seen = chunk::seen; rest=remaining}, fmt something )

            | Error msg ->
                Error ("Parsing `" + chunk + "` went wrong: " + msg)

    (* A parser that matches any string. So the following parser will match
    URLs like `/search/whatever` where `whatever` can be replaced by any string
    you can imagine.
        search = s "search" </> str
    **Note:** this parser will only match URLs with exactly two segments. So things
    like `/search/this/that` would fail. You could use `search </> string` to handle
    that case if you wanted though!
    *)
    let str chunks = custom "string" Ok chunks


    (* A parser that matches any integer. So the following parser will match
    URLs like `/blog/42` where `42` can be replaced by any positive number.
        blog = s "blog" </> i32
    **Note:** this parser will only match URLs with exactly two segments. So things
    like `/blog/42/cat-herding-techniques` would fail. You could use `blog </> string`
    to handle that scenario if you wanted though!
    *)
    let i32 chunks = custom "i32" (int >> Ok) chunks



    // COMBINING PARSERS

    (* Combine parsers. It can be used to combine very simple building blocks
    like this:
        hello = s "hello" </> str
    So we can say hello to whoever we want. It can also be used to put together
    arbitrarily complex parsers, so you *could* say something like this too:
        doubleHello = hello </> hello
    This would match URLs like `/hello/alice/hello/bob`. The point is more that you
    can build complex URL parsers in submodules and then put them on the end of
    parsers in parent modules.
    *)
    let (</>) (parseFirst:Parser<'a,'b>) (parseRest:Parser<'b,'c>) : Parser<'a,'c> =
        fun chunks fmt ->
            parseFirst chunks fmt
            |> Result.bind (fun (nextChunks,nextFmt) -> parseRest nextChunks nextFmt)


    (* Try a bunch of parsers one at a time. This is useful when there is a known
    set of branches that are possible. For example, maybe we have a website that
    just has a blog and a search:
        type DesiredPage = Blog Int | Search String

        desiredPage = oneOf
                        [ format Blog (s "blog" </> i32)
                          format Search (s "search" </> str) ]
    The `desiredPage` parser will first try to match things like `/blog/42` and if
    that fails it will try to match things like `/search/badgers`. It fails if none
    of the parsers succeed.
    *)
    let oneOf (choices:Parser<_,_> list) : Parser<_,_> =
        let rec oneOfHelp choices chunks formatter =
            match choices with
            | [] ->
                Error "Tried many parsers, but none of them worked!"

            | parser :: otherParsers ->
                match parser chunks formatter with
                | Error _ ->
                    oneOfHelp otherParsers chunks formatter

                | Ok answerPair ->
                    Ok answerPair

        oneOfHelp choices



    (* Customize an existing parser. Perhaps you want a parser that matches any
    string, but gives you the result with all lower-case letters:
        caseInsensitiveString = format String.toLower string
        -- String.toLower : String -> String
    I recommend working through how the type variables in `format` would get
    unified to get a better idea of things, but an intuition of how to use things
    is probably enough.
    *)
    let format input parse chunks fmt =
        match parse chunks input with
        | Error msg ->
            Error msg

        | Ok (newChunks, value) ->
            Ok (newChunks, fmt value)
(**
 - title: Navigation demo
 - tagline: The router sample ported from Elm
*)

#r "../node_modules/fable-core/Fable.Core.dll"
#r "../node_modules/fable-elmish/Fable.Elmish.dll"
#r "../node_modules/fable-powerpack/Fable.PowerPack.dll"
#r "../node_modules/fable-elmish-snabbdom/Fable.Elmish.Snabbdom.dll"
#r "../node_modules/fable-elmish-debugger/Fable.Elmish.Debugger.dll"

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Fable.PowerPack
open Elmish.Browser.Navigation
open Elmish.UrlParser

JsInterop.importAll "whatwg-fetch"

// Types
type Page =
  | Home
  | Blog of int
  | Search of string

type Model =
  {
    Page: Page
    Query: string
    Cache: Map<string, string list>
  }

let toHash =
  function
  | Home -> "#home"
  | Blog id -> "#blog/" + (string id)
  | Search query -> "#search/" + query

// The URL is turned into a Result.
let pageParser: Parser<Page->_,_> =
  oneOf [
    format Home (s "home")
    format Blog (s "blog" </> i32)
    format Search (s "search" </> str)
  ]

let hashParser (location: Location) =
  UrlParser.parse id pageParser (location.hash.Substring 1)

type Msg =
  | Query of string
  | Enter
  | FetchFailure of string*exn
  | FetchSuccess of string*(string list)

type Place =
  {
    ``place name``: string
    state: string
  }

type ZipResponse =
  {
    places: Place list
  }

let get query =
  promise {
      let! r = Fable.PowerPack.Fetch.fetchAs<ZipResponse> ("http://api.zippopotam.us/us/" + query) []
      return r |> fun r -> r.places |> List.map (fun p -> p.``place name`` + ", " + p.state)
  }

(* If the URL is valid, we just update our model or issue a command.
If it is not a valid URL, we modify the URL to whatever makes sense.
*)
let urlUpdate (result:Result<Page,string>) model =
  match result with
  | Error e ->
      Browser.console.error("Error parsing url:", e)
      ( model, Navigation.modifyUrl (toHash model.Page) )

  | Ok (Search query as page) ->
      { model with Page = page; Query = query },
         if Map.containsKey query model.Cache then []
         else Cmd.ofPromise get query (fun r -> FetchSuccess (query,r)) (fun ex -> FetchFailure (query,ex))

  | Ok page ->
      { model with Page = page; Query = "" }, []

let init result =
  urlUpdate result { Page = Home; Query = ""; Cache = Map.empty }

(* A relatively normal update function. The only notable thing here is that we
are commanding a new URL to be added to the browser history. This changes the
address bar and lets us use the browser's back button to go back to
previous pages.
*)
let update msg model =
  match msg with
  | Query query ->
      { model with Query = query }, []

  | Enter ->
      let newPage = Search model.Query
      { model with Page = newPage }, Navigation.newUrl (toHash newPage)

  | FetchFailure (query,_) ->
      { model with Cache = Map.add query [] model.Cache }, []

  | FetchSuccess (query,locations) ->
      { model with Cache = Map.add query locations model.Cache }, []


open Fable.Helpers.Snabbdom
open Fable.Helpers.Snabbdom.Props

let viewLink page description =
  a
    [ Style [
        Padding "0 20px"
      ]
      Props [
        Href (toHash page)
      ]
    ]
    [ unbox description ]

let internal centerStyle direction =
  Style [
    Display "flex"
    FlexDirection direction
    AlignItems "center"
    JustifyContent "center"
    Padding "20px 0"
  ]

let words size message =
  span
    [ Style [
        CSSProp.FontSize (Case2 (size |> sprintf "%dpx"))
      ]
    ]
    [ unbox message ]

let internal onEnter msg dispatch =
    function
    | (ev:KeyboardEvent) when ev.keyCode = 13. ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> KeyDown

let viewPage model dispath =
  match model.Page with
  | Home ->
      [ words 60 "Welcome!"
        unbox "Play with the links and search bar above. (Press ENTER to trigger the zip code search.)"
      ]

  | Blog id ->
      [ words 20 "This is blog post number"
        words 100 (string id)
      ]

  | Search query ->
      match Map.tryFind query model.Cache with
      | Some [] ->
          [ unbox ("No results found for " + query + ". Need a valid zip code like 90210.") ]
      | Some (location :: _) ->
          [ words 20 ("Zip code " + query + " is in " + location + "!") ]
      | _ ->
          [ unbox "..." ]

let view model dispath =
  div
    []
    [ div
        [ centerStyle "row" ]
        [ viewLink (Blog 42) "Cat Facts"
          viewLink (Blog 13) "Alligator Jokes"
          viewLink (Blog 26) "Workout Plan"
        ]
      input
        [ Props [
            Placeholder "Enter a zip code (e.g. 90210)"
            Value (Case1 model.Query)
          ]
          Style [
            CSSProp.Width "200px"
            Margin "0 20px"
          ]
        ]
    ]


open Elmish.Snabbdom
open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.toNavigable hashParser urlUpdate
//|> Program.withDebugger
|> Program.withSnabbdom "elmish-app"
|> Program.run

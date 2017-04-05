(**
 - title: Navigation demo
 - tagline: The router sample ported from Elm
*)
module App

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Fable.PowerPack
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser


JsInterop.importAll "whatwg-fetch"

// Types
type Page = Home | Blog of int | Search of string option

type Model =
  { page : Page
    query : string
    cache : Map<string,string list> }

let toHash = 
    function
    | Blog id -> "#blog/" + (string id)
    | Search (Some query) -> "?search=" + query
    | _ -> "#home"

/// The URL is turned into a Page option.
let pageParser : Parser<Page->_,_> =
  let top s = 
    top s
    
  oneOf
    [ map Home (s "home")
      map Blog (s "blog" </> i32)
      map Search (top <?> stringParam "search") ]


type Msg = 
  | Query of string
  | Enter
  | FetchFailure of string*exn
  | FetchSuccess of string*(string list)


type Place = { ``place name``: string; state: string; }
type ZipResponse = { places : Place list }

let get query =
    promise {
        let! r = Fable.PowerPack.Fetch.fetchAs<ZipResponse> ("http://api.zippopotam.us/us/" + query) []
        return r |> fun r -> r.places |> List.map (fun p -> p.``place name`` + ", " + p.state)
    }

(* If the URL is valid, we just update our model or issue a command. 
If it is not a valid URL, we modify the URL to whatever makes sense.
*)
let urlUpdate (result:Option<Page>) model =
  match result with
  | Some (Search (Some query) as page) ->
      { model with page = page; query = query },
         if Map.containsKey query model.cache then [] 
         else Cmd.ofPromise get query (fun r -> FetchSuccess (query,r)) (fun ex -> FetchFailure (query,ex)) 

  | Some page ->
      { model with page = page; query = "" }, []

  | None ->
      Browser.console.error("Error parsing url")  
      ( model, Navigation.modifyUrl (toHash model.page) )

let init result =
  urlUpdate result { page = Home; query = ""; cache = Map.empty }


(* A relatively normal update function. The only notable thing here is that we
are commanding a new URL to be added to the browser history. This changes the
address bar and lets us use the browser&rsquo;s back button to go back to
previous pages.
*)
let update msg model =
  match msg with
  | Query query ->
      { model with query = query }, []

  | Enter ->
      let newPage = Search (Some model.query)
      { model with page = newPage }, Navigation.newUrl (toHash newPage)

  | FetchFailure (query,_) ->
      { model with cache = Map.add query [] model.cache }, []

  | FetchSuccess (query,locations) -> 
      { model with cache = Map.add query locations model.cache }, []




// VIEW

open Fable.Helpers.React
open Fable.Helpers.React.Props


let viewLink page description =
  a [ Style [ Padding "0 20px" ]
      Href (toHash page) ] 
    [ str description]

let internal centerStyle direction =
    Style [ Display "flex"
            FlexDirection direction
            AlignItems "center"
            unbox("justifyContent", "center")
            Padding "20px 0" ]

let words size message =
  span [ Style [ unbox("fontSize", size |> sprintf "%dpx") ] ] [ str message ]

let internal onEnter msg dispatch =
    function 
    | (ev:React.KeyboardEvent) when ev.keyCode = 13. ->
        ev.preventDefault() 
        dispatch msg
    | _ -> ()
    |> OnKeyDown

let viewPage model dispatch =
  match model.page with
  | Home ->
      [ words 60 "Welcome!"
        str "Play with the links and search bar above. (Press ENTER to trigger the zip code search.)" ]

  | Blog id ->
      [ words 20 "This is blog post number"
        words 100 (string id) ]

  | Search (Some query) ->
      match Map.tryFind query model.cache with
      | Some [] ->
          [ str ("No results found for " + query + ". Need a valid zip code like 90210.") ]
      | Some (location :: _) ->
          [ words 20 ("Zip code " + query + " is in " + location + "!") ]
      | _ ->
          [ str "..." ]
  
  | Search (None) ->
          [ str "Invalid query" ]

open Fable.Core.JsInterop

let view model dispatch =
  div []
    [ div [ centerStyle "row" ]
        [ viewLink Home "Home"
          viewLink (Blog 42) "Cat Facts"
          viewLink (Blog 13) "Alligator Jokes"
          viewLink (Blog 26) "Workout Plan"
          input
            [ Placeholder "Enter a zip code (e.g. 90210)"
              Value (U2.Case1 model.query)
              onEnter Enter dispatch
              OnInput (fun ev -> Query (unbox ev.target?value) |> dispatch)
              Style [ CSSProp.Width "200px"; Margin "0 20px" ] ]
            []
        ]
      hr [] []
      div [ centerStyle "column" ] (viewPage model dispatch)
    ]

open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withReact "elmish-app"
|> Program.withDebugger
|> Program.run 
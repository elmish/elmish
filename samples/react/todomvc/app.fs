(**
 - title: Todo MVC
 - tagline: The famous todo mvc ported from elm-todomvc
*)

module App

open Fable.Core
open Fable.Import
open Elmish

let [<Literal>] ESC_KEY = 27.
let [<Literal>] ENTER_KEY = 13.
let [<Literal>] ALL_TODOS = "all"
let [<Literal>] ACTIVE_TODOS = "active"
let [<Literal>] COMPLETED_TODOS = "completed"

// Local storage interface
module S =
    let private STORAGE_KEY = "elmish-react-todomvc"
    let [<PassGenericsAttribute>] load<'T> (): 'T option =
        Browser.localStorage.getItem(STORAGE_KEY)
        |> unbox
        |> Core.Option.map (JsInterop.ofJson)

    let save<'T> (model: 'T) =
        Browser.localStorage.setItem(STORAGE_KEY, JsInterop.toJson model)


// MODEL
type Entry = {
    description : string
    completed : bool
    editing : bool
    id : int
}

// The full application state of our todo app.
type Model = {
    entries : Entry list
    field : string
    uid : int
    visibility : string
}

let emptyModel =
    { entries = []
      visibility = ALL_TODOS
      field = ""
      uid = 0 }

let newEntry desc id =
  { description = desc
    completed = false
    editing = false
    id = id }


let init = function
  | Some savedModel -> savedModel, []
  | _ -> emptyModel, []


// UPDATE


(** Users of our app can trigger messages by clicking and typing. These
messages are fed into the `update` function as they occur, letting us react
to them.
*)
type Msg =
    | NoOp
    | UpdateField of string
    | EditingEntry of int*bool
    | UpdateEntry of int*string
    | Add
    | Delete of int
    | DeleteComplete
    | Check of int*bool
    | CheckAll of bool
    | ChangeVisibility of string



// How we update our Model on a given Msg?
let update (msg:Msg) (model:Model) : Model*Cmd<Msg>=
    match msg with
    | NoOp ->
        model, []

    | Add ->
        let xs = if System.String.IsNullOrEmpty model.field then
                    model.entries
                 else
                    model.entries @ [newEntry model.field model.uid]
        { model with
            uid = model.uid + 1
            field = ""
            entries = xs }, []

    | UpdateField str ->
      { model with field = str }, []

    | EditingEntry (id,isEditing) ->
        let updateEntry t =
          if t.id = id then { t with editing = isEditing } else t
        { model with entries = List.map updateEntry model.entries }, []

    | UpdateEntry (id,task) ->
        let updateEntry t =
          if t.id = id then { t with description = task } else t
        { model with entries = List.map updateEntry model.entries }, []

    | Delete id ->
        { model with entries = List.filter (fun t -> t.id <> id) model.entries }, []

    | DeleteComplete ->
        { model with entries = List.filter (fun t -> not t.completed) model.entries }, []

    | Check (id,isCompleted) ->
        let updateEntry t =
          if t.id = id then { t with completed = isCompleted } else t
        { model with entries = List.map updateEntry model.entries }, []

    | CheckAll isCompleted ->
        let updateEntry t = { t with completed = isCompleted }
        { model with entries = List.map updateEntry model.entries }, []

    | ChangeVisibility visibility ->
        { model with visibility = visibility }, []

let setStorage (model:Model) : Cmd<Msg> =
    let noop _ = NoOp
    Cmd.ofFunc S.save model noop noop // TODO

let updateWithStorage (msg:Msg) (model:Model) =
  match msg with
  // If the Msg is NoOp we don't need to store the model in the storage
  | NoOp -> model, []
  | _ ->
    let (newModel, cmds) = update msg model
    newModel, Cmd.batch [ setStorage newModel; cmds ]

// rendering views with React
module R = Fable.Helpers.React
open Fable.Core.JsInterop
open Fable.Helpers.React.Props
open Elmish.React

let internal onEnter msg dispatch =
    function
    | (ev:React.KeyboardEvent) when ev.keyCode = ENTER_KEY ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> OnKeyDown

let viewInput (model:string) dispatch =
    R.header [ ClassName "header" ] [
        R.h1 [] [ unbox "todos" ]
        R.input [
            ClassName "new-todo"
            Placeholder "What needs to be done?"
            DefaultValue (U2.Case1 model)
            onEnter Add dispatch
            OnChange ((fun (ev:React.FormEvent) -> ev.target?value) >> unbox >> UpdateField >> dispatch)
            AutoFocus true
        ] []
    ]

let internal classList classes =
    classes
    |> List.fold (fun complete -> function | (name,true) -> complete + " " + name | _ -> complete) ""
    |> ClassName

let viewEntry todo dispatch =
  R.li
    [ classList [ ("completed", todo.completed); ("editing", todo.editing) ]]
    [ R.div
        [ ClassName "view" ]
        [ R.input
            [ ClassName "toggle"
              Type "checkbox"
              Checked todo.completed
              OnChange (fun _ -> Check (todo.id,(not todo.completed)) |> dispatch) ]
            []
          R.label
            [ OnDoubleClick (fun _ -> EditingEntry (todo.id,true) |> dispatch) ]
            [ unbox todo.description ]
          R.button
            [ ClassName "destroy"
              OnClick (fun _-> Delete todo.id |> dispatch) ]
            []
        ]
      R.input
        [ ClassName "edit"
          DefaultValue (U2.Case1 todo.description)
          Name "title"
          Id ("todo-" + (string todo.id))
          OnInput (fun ev -> UpdateEntry (todo.id, unbox ev.target?value) |> dispatch)
          OnBlur (fun _ -> EditingEntry (todo.id,false) |> dispatch)
          onEnter (EditingEntry (todo.id,false)) dispatch ]
        []
    ]

let viewEntries visibility entries dispatch =
    let isVisible todo =
        match visibility with
        | COMPLETED_TODOS -> todo.completed
        | ACTIVE_TODOS -> not todo.completed
        | _ -> true

    let allCompleted =
        List.forall (fun t -> t.completed) entries

    let cssVisibility =
        if List.isEmpty entries then "hidden" else "visible"

    R.section
      [ ClassName "main"
        Style [ Visibility cssVisibility ]]
      [ R.input
          [ ClassName "toggle-all"
            Type "checkbox"
            Name "toggle"
            Checked allCompleted
            OnChange (fun _ -> CheckAll (not allCompleted) |> dispatch)]
          []
        R.label
          [ HtmlFor "toggle-all" ]
          [ unbox "Mark all as complete" ]
        R.ul
          [ ClassName "todo-list" ]
          (entries
           |> List.filter isVisible
           |> List.map (fun i -> lazyView2 viewEntry i dispatch)) ]

// VIEW CONTROLS AND FOOTER
let visibilitySwap uri visibility actualVisibility dispatch =
  R.li
    [ OnClick (fun _ -> ChangeVisibility visibility |> dispatch) ]
    [ R.a [ Href uri
            classList ["selected", visibility = actualVisibility] ]
          [ unbox visibility ] ]

let viewControlsFilters visibility dispatch =
  R.ul
    [ ClassName "filters" ]
    [ visibilitySwap "#/" ALL_TODOS visibility dispatch
      unbox " "
      visibilitySwap "#/active" ACTIVE_TODOS visibility dispatch
      unbox " "
      visibilitySwap "#/completed" COMPLETED_TODOS visibility dispatch ]

let viewControlsCount entriesLeft =
  let item =
      if entriesLeft = 1 then " item" else " items"

  R.span
      [ ClassName "todo-count" ]
      [ R.strong [] [ unbox (string entriesLeft) ]
        unbox (item + " left") ]

let viewControlsClear entriesCompleted dispatch =
  R.button
    [ ClassName "clear-completed"
      Hidden (entriesCompleted = 0)
      OnClick (fun _ -> DeleteComplete |> dispatch)]
    [ unbox ("Clear completed (" + (string entriesCompleted) + ")") ]

let viewControls visibility entries dispatch =
  let entriesCompleted =
      entries
      |> List.filter (fun t -> t.completed)
      |> List.length

  let entriesLeft =
      List.length entries - entriesCompleted

  R.footer
      [ ClassName "footer"
        Hidden (List.isEmpty entries) ]
      [ lazyView viewControlsCount entriesLeft
        lazyView2 viewControlsFilters visibility dispatch
        lazyView2 viewControlsClear entriesCompleted dispatch ]


let infoFooter =
  R.footer [ ClassName "info" ]
    [ R.p []
        [ unbox "Double-click to edit a todo" ]
      R.p []
        [ unbox "Ported from Elm by "
          R.a [ Href "https://github.com/et1975" ] [ unbox "Eugene Tolmachev" ]]
      R.p []
        [ unbox "Part of "
          R.a [ Href "http://todomvc.com" ] [ unbox "TodoMVC" ]]
    ]

let view model dispatch =
  R.div
    [ ClassName "todomvc-wrapper"]
    [ R.section
        [ ClassName "todoapp" ]
        [ lazyView2 viewInput model.field dispatch
          lazyView3 viewEntries model.visibility model.entries dispatch
          lazyView3 viewControls model.visibility model.entries dispatch ]
      infoFooter ]

open Elmish.Debug
// App
Program.mkProgram (S.load >> init) updateWithStorage view
|> Program.withReact "todoapp"
|> Program.withConsoleTrace
|> Program.run

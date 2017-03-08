(**
 - title: Todo MVC
 - tagline: The famous todo mvc ported from elm-todomvc
*)


#r "../node_modules/fable-core/Fable.Core.dll"
#r "../node_modules/fable-elmish/Fable.Elmish.dll"
#r "../node_modules/fable-elmish-snabbdom/Fable.Elmish.Snabbdom.dll"
#r "../node_modules/fable-elmish-debugger/Fable.Elmish.Debugger.dll"

open Fable.Core
open Fable.Import
open Elmish

let [<Literal>] ESC_KEY = 27.
let [<Literal>] ENTER_KEY = 13.
let [<Literal>] ALL_TODOS = "all"
let [<Literal>] ACTIVE_TODOS = "active"
let [<Literal>] COMPLETED_TODOS = "completed"

// MODEL
type Entry = 
  { Description : string
    Completed : bool
    Editing : bool
    Id : int }

// The full application state of our todo app.
type Model = 
  { Entries : Entry list
    Field : string
    Uid : int
    Visibility : string }

let emptyModel =
  { Entries = []
    Visibility = ALL_TODOS
    Field = ""
    Uid = 0 }

let newEntry desc id =
  { Description = desc
    Completed = false
    Editing = false
    Id = id }


let init _ = emptyModel, Cmd.Empty
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
    Browser.console.log model.Field
    let xs =
      if System.String.IsNullOrEmpty model.Field then
        model.Entries
      else
        model.Entries @ [newEntry model.Field model.Uid]
    { model with
        Uid = model.Uid + 1
        Field = ""
        Entries = xs }, []

  | UpdateField str ->
    Browser.console.log str
    { model with Field = str }, []

  | EditingEntry (id,isEditing) ->
    let updateEntry t =
      if t.Id = id then { t with Editing = isEditing } else t
    { model with Entries = List.map updateEntry model.Entries }, []

  | UpdateEntry (id,task) ->
    let updateEntry t =
      if t.Id = id then { t with Description = task } else t
    { model with Entries = List.map updateEntry model.Entries }, []

  | Delete id ->
    { model with Entries = List.filter (fun t -> t.Id <> id) model.Entries }, []

  | DeleteComplete ->
    { model with Entries = List.filter (fun t -> not t.Completed) model.Entries }, []

  | Check (id,isCompleted) ->
    let updateEntry t =
      if t.Id = id then { t with Completed = isCompleted } else t
    { model with Entries = List.map updateEntry model.Entries }, []

  | CheckAll isCompleted ->
    let updateEntry t = { t with Completed = isCompleted }
    { model with Entries = List.map updateEntry model.Entries }, []

  | ChangeVisibility visibility ->
    { model with Visibility = visibility }, []


// rendering views with React
open Fable.Core.JsInterop
open Fable.Helpers.Snabbdom
open Fable.Helpers.Snabbdom.Props
open Fable.Import.Browser



let internal onEnter msg dispatch =
  function
  | (ev: KeyboardEvent) when ev.keyCode = ENTER_KEY ->
    dispatch msg
  | _ -> ()
  |> OnKeyDown

let viewInput (model:string) dispatch =
  header
    [
      Props [
        ClassName "header"
      ]
    ]
    [
      h1
        []
        [ unbox "todos" ]
      input
        [
          Props [
            ClassName "new-todo"
            Placeholder "What needs to be done?"
            Value (U2.Case1 model)
            AutoFocus true
          ]
          Events [
            onEnter Add dispatch
            OnInput ((fun ev -> ev.target?value) >> unbox >> UpdateField >> dispatch)
          ]
        ]
  ]

let viewEntry todo dispatch =
  li
    [
      Class [
        Classy ("completed", todo.Completed)
        Classy ("editing", todo.Editing)
      ]
    ]
    [
      div
        [
          Props [
            ClassName "view"
          ]
        ]
        [
          input
            [
              Props [
                ClassName "toggle"
                Type "checkbox"
                Checked todo.Completed
              ]
              Events [
                OnInput (fun _ -> Check (todo.Id,(not todo.Completed)) |> dispatch)
              ]
            ]
          label
            [
              Events [
                OnDoubleClick (fun _ -> EditingEntry (todo.Id,true) |> dispatch)
              ]
            ]
            [ unbox todo.Description ]
          button
            [
              Props [
                ClassName "destroy"
              ]
              Events [
                OnClick (fun _-> Delete todo.Id |> dispatch)
              ]
            ] []
        ]
      input
        [
          Props [
            ClassName "edit"
            DefaultValue (Case1 todo.Description)
            Name "title"
            Id ("todo-" + (string todo.Id))
          ]
          Events [
            OnInput (fun ev -> UpdateEntry (todo.Id, unbox ev.target?value) |> dispatch)
            OnBlur (fun _ -> EditingEntry (todo.Id,false) |> dispatch)
            onEnter (EditingEntry (todo.Id,false)) dispatch
          ]
        ]
    ]

let viewEntries visibility entries dispatch =
  let isVisible todo =
    match visibility with
    | COMPLETED_TODOS -> todo.Completed
    | ACTIVE_TODOS -> not todo.Completed
    | _ -> true

  let allCompleted =
    List.forall (fun t -> t.Completed) entries

  let cssVisibility =
    if List.isEmpty entries then "hidden" else "visible"

  section
    [
      Props [
        ClassName "main"
      ]
      Style [
        Visibility cssVisibility
      ]
    ]
    [
      input
        [
          Props [
            ClassName "toggle-all"
            Type "checkbox"
            Name "toggle"
            Checked allCompleted
          ]
          Events [
            OnChange (fun _ -> CheckAll (not allCompleted) |> dispatch)
          ]
        ]
      label
        [
          Props [
            HtmlFor "toggle-all"
          ]
        ]
        [ unbox "Mark all as complete" ]
      ul
        [
          Props [
            ClassName "todo-list"
          ]
        ]
        ( entries
          |> List.filter isVisible
          |> List.map (fun i -> viewEntry i dispatch))
    ]

// VIEW CONTROLS AND FOOTER
let visibilitySwap uri visibility actualVisibility dispatch =
  li
    [
      Events [
        OnClick (fun _ -> ChangeVisibility visibility |> dispatch)
      ]
    ]
    [
      a
        [
          Props [
            Href uri
          ]
          Class [
            Classy ("selected", visibility = actualVisibility)
          ]
        ]
        [ unbox visibility ]
    ]


let viewControlsFilters visibility dispatch =
  ul
    [ Props [
        ClassName "filters"
      ]
    ]
    [
      visibilitySwap "#/" ALL_TODOS visibility dispatch
      unbox " "
      visibilitySwap "#/active" ACTIVE_TODOS visibility dispatch
      unbox " "
      visibilitySwap "#/completed" COMPLETED_TODOS visibility dispatch
    ]


let viewControlsCount entriesLeft =
  let item =
    if entriesLeft = 1 then " item" else " items"

  span
    [
      Props [
        ClassName "todo-count"
      ]
    ]
    [
      strong
        []
        [ unbox (string entriesLeft) ]
      unbox (item + " left")
    ]


let viewControlsClear entriesCompleted dispatch =
  button
    [ Props [
        ClassName "clear-completed"
        Hidden (entriesCompleted = 0)
      ]
      Events [
        OnClick (fun _ -> DeleteComplete |> dispatch)
      ]
    ]
    [ unbox ("Clear completed (" + (string entriesCompleted) + ")") ]

let viewControls visibility entries dispatch =
  let entriesCompleted =
    entries
    |> List.filter (fun t -> t.Completed)
    |> List.length

  let entriesLeft =
    List.length entries - entriesCompleted

  footer
    [ Props [
        ClassName "footer"
        Hidden (List.isEmpty entries)
      ]
    ]
    [
      viewControlsCount entriesLeft
      viewControlsFilters visibility dispatch
      viewControlsClear entriesCompleted dispatch
    ]


let infoFooter =
  footer
    [
      Props [
        ClassName "info"
      ]
    ]
    [
      p
        []
        [ unbox "Double-click to edit a todo" ]
      p
        []
        [ unbox "Ported by "
          a
            [
              Props [
                Href "https://github.com/MangelMaxime"
              ]
            ]
            [ unbox "Maxime Mangel" ]
        ]
      p []
        [ unbox "Part of "
          a
            [
              Props [
                Href "http://todomvc.com"
              ]
            ]
            [ unbox "TodoMVC" ]
        ]
    ]


let view model dispatch =
  div
    [
      Props [
        ClassName "todomvc-wrapper"
      ]
    ]
    [
      section
        [ Props [
            ClassName "todoapp"
          ]
        ]
        [
          viewInput model.Field dispatch
          viewEntries model.Visibility model.Entries dispatch
          viewControls model.Visibility model.Entries dispatch
          infoFooter
        ]
    ]


open Elmish.Debug
open Elmish.Snabbdom
// App
Program.mkProgram init update view
|> Program.withSnabbdom "todoapp"
|> Program.run

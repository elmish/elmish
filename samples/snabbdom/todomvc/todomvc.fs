module TodoMVC

(**
 - title: Todo MVC
 - tagline: The famous todo mvc ported from elm-todomvc
*)

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
      props [
        ClassName "header"
      ]
    ]
    [
      h1
        []
        [ str "todos" ]
      input
        [
          props [
            ClassName "new-todo"
            Placeholder "What needs to be done?"
            Value !^model
            AutoFocus true
          ]
          events [
            onEnter Add dispatch
            OnInput (fun ev -> !!ev.target?value |> UpdateField |> dispatch)
          ]
        ]
  ]

let viewEntry todo dispatch =
  li
    [
      classes [
        "completed", todo.Completed
        "editing", todo.Editing
      ]
    ]
    [
      div
        [
          props [
            ClassName "view"
          ]
        ]
        [
          input
            [
              props [
                ClassName "toggle"
                Type "checkbox"
                Checked todo.Completed
              ]
              events [
                OnInput (fun _ -> Check (todo.Id,(not todo.Completed)) |> dispatch)
              ]
            ]
          label
            [
              events [
                OnDoubleClick (fun _ -> EditingEntry (todo.Id,true) |> dispatch)
              ]
            ]
            [ str todo.Description ]
          button
            [
              props [
                ClassName "destroy"
              ]
              events [
                OnClick (fun _-> Delete todo.Id |> dispatch)
              ]
            ] []
        ]
      input
        [
          props [
            ClassName "edit"
            DefaultValue !^todo.Description
            Name "title"
            Id ("todo-" + (string todo.Id))
          ]
          events [
            OnInput (fun ev -> UpdateEntry (todo.Id, !!ev.target?value) |> dispatch)
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
      props [
        ClassName "main"
      ]
      style [
        Visibility cssVisibility
      ]
    ]
    [
      input
        [
          props [
            ClassName "toggle-all"
            Type "checkbox"
            Name "toggle"
            Checked allCompleted
          ]
          events [
            OnChange (fun _ -> CheckAll (not allCompleted) |> dispatch)
          ]
        ]
      label
        [
          props [
            HtmlFor "toggle-all"
          ]
        ]
        [ str "Mark all as complete" ]
      ul
        [
          props [
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
      events [
        OnClick (fun _ -> ChangeVisibility visibility |> dispatch)
      ]
    ]
    [
      a
        [
          props [
            Href uri
          ]
          classes [
            "selected", visibility = actualVisibility
          ]
        ]
        [ str visibility ]
    ]


let viewControlsFilters visibility dispatch =
  ul
    [ props [
        ClassName "filters"
      ]
    ]
    [
      visibilitySwap "#/" ALL_TODOS visibility dispatch
      str " "
      visibilitySwap "#/active" ACTIVE_TODOS visibility dispatch
      str " "
      visibilitySwap "#/completed" COMPLETED_TODOS visibility dispatch
    ]


let viewControlsCount entriesLeft =
  let item =
    if entriesLeft = 1 then " item" else " items"

  span
    [
      props [
        ClassName "todo-count"
      ]
    ]
    [
      strong
        []
        [ str (string entriesLeft) ]
      str (item + " left")
    ]


let viewControlsClear entriesCompleted dispatch =
  button
    [ props [
        ClassName "clear-completed"
        Hidden (entriesCompleted = 0)
      ]
      events [
        OnClick (fun _ -> DeleteComplete |> dispatch)
      ]
    ]
    [ str ("Clear completed (" + (string entriesCompleted) + ")") ]

let viewControls visibility entries dispatch =
  let entriesCompleted =
    entries
    |> List.filter (fun t -> t.Completed)
    |> List.length

  let entriesLeft =
    List.length entries - entriesCompleted

  footer
    [ props [
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
      props [
        ClassName "info"
      ]
    ]
    [
      p
        []
        [ str "Double-click to edit a todo" ]
      p
        []
        [ str "Ported by "
          a
            [
              props [
                Href "https://github.com/MangelMaxime"
              ]
            ]
            [ str "Maxime Mangel" ]
        ]
      p []
        [ str "Part of "
          a
            [
              props [
                Href "http://todomvc.com"
              ]
            ]
            [ str "TodoMVC" ]
        ]
    ]


let view model dispatch =
  div
    [
      props [
        ClassName "todomvc-wrapper"
      ]
    ]
    [
      section
        [ props [
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


open Elmish.Snabbdom
// App
Program.mkProgram init update view
|> Program.withSnabbdom "todoapp"
|> Program.run

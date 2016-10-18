/// This library is primarily about treating the address bar as an input to your program. 
namespace Elmish.Browser.Navigation

open Fable.Import.Browser
open Elmish

/// Parser is a function to turn the string in the address bar into
/// data that is easier for your app to handle.
type Parser<'a> = Location -> 'a

type Navigable<'msg> = 
    | Change of Location
    | UserMsg of 'msg

module Navigation =
    let modifyUrl (newUrl:string) =
        [fun _ -> history.replaceState((), "", newUrl)]

    let newUrl (newUrl:string) =
        [fun _ -> history.pushState((), "", newUrl)
                  document.createEvent_PopStateEvent()
                  |> window.dispatchEvent 
                  |> ignore ]

    let jump (n:int) =
        [fun _ -> history.go n]

module Program =
  /// Add the navigation to a program made with `mkProgram` or `mkSimple`.
  /// urlUpdate: similar to `update` function, but receives parsed url instead of message as an input.
  let runWithNavigation (parser:Parser<'a>) 
                        (urlUpdate:'a->'model->('model * Cmd<'msg>)) 
                        (setState:'model->unit) 
                        (program:Program<'a,'model,'msg,'view>) =
    let map (model, cmd) = 
        model, cmd |> Cmd.map UserMsg
    
    let update msg model =
        match msg with
        | Change location ->
            urlUpdate (parser location) model
        | UserMsg userMsg ->
            program.update userMsg model
        |> map

    let locationChanges (dispatch:Dispatch<_ Navigable>) = 
        window.addEventListener_popstate(fun ev -> Change window.location |> dispatch |> box)
    
    let subs model =
        Cmd.batch
          [ [locationChanges]
            program.subscribe model |> Cmd.map UserMsg ]
    
    let init () = 
        program.init (parser window.location) |> map

    let dispatch = 
        { init = init 
          update = update
          subscribe = subs
          view = fun model dispatch -> program.view model (UserMsg >> dispatch) }
        |> Program.run setState
    
    UserMsg >> dispatch



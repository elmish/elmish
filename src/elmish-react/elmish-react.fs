namespace Elmish.React

open System
open Fable.Import.React
open Fable.Core

[<RequireQualifiedAccess>]
module Program =
    module R = Fable.Helpers.React

    /// Render root React component inside html element identified by placeholderId
    let toHtml run placeholderId program =
        let props = toAppProps run program

        Fable.Import.ReactDom.render(
            R.com<Components.App<_,_>,_,_> props [],
            Fable.Import.Browser.document.getElementsByClassName(placeholderId).[0]
        )

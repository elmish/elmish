module FifteenPuzzle.Styles

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React.Props

let baseStyle (color: string) = 
    Style [
        Height 40
        Padding 15
        TextAlign "center"
        Margin 5
        VerticalAlign "middle"
        BackgroundColor color
        CSSProp.Width 55
        CSSProp.FontSize (!^ 24.0)
        LineHeight (!^ 40.0)
        unbox ("font-size", "24px")
        unbox ("line-height", "40px")
        unbox ("cursor","pointer")
        unbox ("box-shadow", "0 0 3px black")
    ]

let greenSlotStyle = baseStyle "lightgreen"
let redSlotStyle = baseStyle "orange"
let freeSlotStyle = baseStyle "white"     

let puzzleStyle = 
    Style [
      unbox ("width", "407px")
      Padding 20
    ]

let headerStyle = 
    Style [
        Padding 20
        BackgroundColor "lightblue"
        unbox ("border-radius", "15px")
        unbox ("width", "400px")
    ]
module Calc.Styles

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React.Props

let digitStyle = 
    Style [
        Height 40
        Padding 15
        TextAlign "center"
        Margin 5
        VerticalAlign "middle"
        BackgroundColor "lightgreen"
        CSSProp.Width 55
        CSSProp.FontSize (!^ 24.0)
        LineHeight (!^ 40.0)
        unbox ("font-size", "24px")
        unbox ("line-height", "40px")
        unbox ("cursor","pointer")
        unbox ("box-shadow", "0 0 3px black")
    ]

let opButtonStyle = 
    Style [
        Height 40
        Padding 15
        TextAlign "center"
        Margin 5
        VerticalAlign "middle"
        BackgroundColor "lightblue"
        unbox ("width", "55px")
        unbox ("font-size","24px")
        unbox ("line-height","40px")
        unbox ("cursor","pointer")
        unbox ("box-shadow", "0 0 3px black")
    ]


let calcStyle = 
    Style [
      unbox ("width", "407px")
      unbox ("border", "2px black solid")
      unbox ("border-radius", "15px")
      unbox ("padding", "10px")
    ]
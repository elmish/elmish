module internal Styles

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props

let [<Literal>] brandPrimary = "#428bca"
let [<Literal>] brandInfo = "#5bc0de"
let [<Literal>] brandSuccess = "#5cb85c"
let [<Literal>] brandDanger = "#d9534f"
let [<Literal>] brandWarning = "#f0ad4e"
let [<Literal>] brandSidebar = "#252932"

let [<Literal>] inverseTextColor = "#000"

let [<Literal>] textColor = "#FFFFFF"

let [<Literal>] shadowColor = "#000000"

let [<Literal>] backgroundColor = "#615A5B"
let [<Literal>] inputBackgroundColor = "#251D1C"

let [<Literal>] touched = "#5499C4"

let [<Literal>] fontSizeBase = 15.
let [<Literal>] titleFontSize = 17.

let [<Literal>] borderRadius = 4.

let inline buttonStyle<'a> =
    TouchableHighlightProperties.Style [
        ViewStyle.BackgroundColor brandPrimary
        ViewStyle.BorderRadius borderRadius
        Margin 5.
      ]

let inline defaultText<'a> =
    TextProperties.Style [ 
        TextStyle.Color textColor
        TextStyle.TextAlign TextAlignment.Center
        Margin 5.
        TextStyle.FontSize fontSizeBase
      ]

let inline titleText<'a> =
    TextProperties.Style [ 
        TextStyle.Color textColor
        TextStyle.TextAlign TextAlignment.Center
        Margin 15.
        TextStyle.FontSize titleFontSize
      ] 

let inline sceneBackground<'a> =
    ViewProperties.Style [ 
        AlignSelf Alignment.Stretch
        Padding 20.
        ViewStyle.ShadowColor shadowColor
        ViewStyle.ShadowOpacity 0.8
        ViewStyle.ShadowRadius 3.
        JustifyContent JustifyContent.Center
        Flex 1.
        ViewStyle.BackgroundColor backgroundColor
      ]

let inline button label onPress =
    text [ defaultText ] label
    |> touchableHighlightWithChild [
        buttonStyle
        TouchableHighlightProperties.UnderlayColor touched
        OnPress onPress]

let inline verticalButton label onPress =
    text [ defaultText ] label
    |> touchableHighlightWithChild [
        TouchableHighlightProperties.Style [
            ViewStyle.BackgroundColor brandPrimary
            ViewStyle.BorderRadius borderRadius
            Margin 5.
            Padding 5.
        ]
        TouchableHighlightProperties.UnderlayColor touched
        OnPress onPress]
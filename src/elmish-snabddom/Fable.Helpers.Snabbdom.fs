[<Fable.Core.Erase>]
module Fable.Helpers.Snabbdom

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser

module Props =

    [<KeyValueList>]
    type ICSSProp =
        interface end

    [<KeyValueList>]
    type CSSProp =
        | [<Erase>] Extra of string * string
        | BoxFlex of float
        | BoxFlexGroup of float
        | ColumnCount of float
        | Flex of U2<float, string>
        | FlexGrow of float
        | FlexShrink of float
        | FontWeight of U2<float, string>
        | LineClamp of float
        | LineHeight of U2<float, string>
        | Opacity of float
        | Order of float
        | Orphans of float
        | Widows of float
        | ZIndex of float
        | Zoom of float
        | FontSize of U2<float, string>
        | FillOpacity of float
        | StrokeOpacity of float
        | StrokeWidth of float
        | AlignContent of obj
        | AlignItems of obj
        | AlignSelf of obj
        | AlignmentAdjust of obj
        | AlignmentBaseline of obj
        | AnimationDelay of obj
        | AnimationDirection of obj
        | AnimationIterationCount of obj
        | AnimationName of obj
        | AnimationPlayState of obj
        | Appearance of obj
        | BackfaceVisibility of obj
        | BackgroundBlendMode of obj
        | BackgroundColor of obj
        | BackgroundComposite of obj
        | BackgroundImage of obj
        | BackgroundOrigin of obj
        | BackgroundPositionX of obj
        | BackgroundRepeat of obj
        | BaselineShift of obj
        | Behavior of obj
        | Border of obj
        | BorderBottomLeftRadius of obj
        | BorderBottomRightRadius of obj
        | BorderBottomWidth of obj
        | BorderCollapse of obj
        | BorderColor of obj
        | BorderCornerShape of obj
        | BorderImageSource of obj
        | BorderImageWidth of obj
        | BorderLeft of obj
        | BorderLeftColor of obj
        | BorderLeftStyle of obj
        | BorderLeftWidth of obj
        | BorderRight of obj
        | BorderRightColor of obj
        | BorderRightStyle of obj
        | BorderRightWidth of obj
        | BorderSpacing of obj
        | BorderStyle of obj
        | BorderTop of obj
        | BorderTopColor of obj
        | BorderTopLeftRadius of obj
        | BorderTopRightRadius of obj
        | BorderTopStyle of obj
        | BorderTopWidth of obj
        | BorderWidth of obj
        | Bottom of obj
        | BoxAlign of obj
        | BoxDecorationBreak of obj
        | BoxDirection of obj
        | BoxLineProgression of obj
        | BoxLines of obj
        | BoxOrdinalGroup of obj
        | BreakAfter of obj
        | BreakBefore of obj
        | BreakInside of obj
        | Clear of obj
        | Clip of obj
        | ClipRule of obj
        | Color of obj
        | ColumnFill of obj
        | ColumnGap of obj
        | ColumnRule of obj
        | ColumnRuleColor of obj
        | ColumnRuleWidth of obj
        | ColumnSpan of obj
        | ColumnWidth of obj
        | Columns of obj
        | CounterIncrement of obj
        | CounterReset of obj
        | Cue of obj
        | CueAfter of obj
        | Direction of obj
        | Display of obj
        | Fill of obj
        | FillRule of obj
        | Filter of obj
        | FlexAlign of obj
        | FlexBasis of obj
        | FlexDirection of obj
        | FlexFlow of obj
        | FlexItemAlign of obj
        | FlexLinePack of obj
        | FlexOrder of obj
        | FlexWrap of obj
        | Float of obj
        | FlowFrom of obj
        | Font of obj
        | FontFamily of obj
        | FontKerning of obj
        | FontSizeAdjust of obj
        | FontStretch of obj
        | FontStyle of obj
        | FontSynthesis of obj
        | FontVariant of obj
        | FontVariantAlternates of obj
        | GridArea of obj
        | GridColumn of obj
        | GridColumnEnd of obj
        | GridColumnStart of obj
        | GridRow of obj
        | GridRowEnd of obj
        | GridRowPosition of obj
        | GridRowSpan of obj
        | GridTemplateAreas of obj
        | GridTemplateColumns of obj
        | GridTemplateRows of obj
        | Height of obj
        | HyphenateLimitChars of obj
        | HyphenateLimitLines of obj
        | HyphenateLimitZone of obj
        | Hyphens of obj
        | ImeMode of obj
        | JustifyContent of obj
        | LayoutGrid of obj
        | LayoutGridChar of obj
        | LayoutGridLine of obj
        | LayoutGridMode of obj
        | LayoutGridType of obj
        | Left of obj
        | LetterSpacing of obj
        | LineBreak of obj
        | ListStyle of obj
        | ListStyleImage of obj
        | ListStylePosition of obj
        | ListStyleType of obj
        | Margin of obj
        | MarginBottom of obj
        | MarginLeft of obj
        | MarginRight of obj
        | MarginTop of obj
        | MarqueeDirection of obj
        | MarqueeStyle of obj
        | Mask of obj
        | MaskBorder of obj
        | MaskBorderRepeat of obj
        | MaskBorderSlice of obj
        | MaskBorderSource of obj
        | MaskBorderWidth of obj
        | MaskClip of obj
        | MaskOrigin of obj
        | MaxFontSize of obj
        | MaxHeight of obj
        | MaxWidth of obj
        | MinHeight of obj
        | MinWidth of obj
        | Outline of obj
        | OutlineColor of obj
        | OutlineOffset of obj
        | Overflow of obj
        | OverflowStyle of obj
        | OverflowX of obj
        | Padding of obj
        | PaddingBottom of obj
        | PaddingLeft of obj
        | PaddingRight of obj
        | PaddingTop of obj
        | PageBreakAfter of obj
        | PageBreakBefore of obj
        | PageBreakInside of obj
        | Pause of obj
        | PauseAfter of obj
        | PauseBefore of obj
        | Perspective of obj
        | PerspectiveOrigin of obj
        | PointerEvents of obj
        | Position of obj
        | PunctuationTrim of obj
        | Quotes of obj
        | RegionFragment of obj
        | RestAfter of obj
        | RestBefore of obj
        | Right of obj
        | RubyAlign of obj
        | RubyPosition of obj
        | ShapeImageThreshold of obj
        | ShapeInside of obj
        | ShapeMargin of obj
        | ShapeOutside of obj
        | Speak of obj
        | SpeakAs of obj
        | TabSize of obj
        | TableLayout of obj
        | TextAlign of obj
        | TextAlignLast of obj
        | TextDecoration of obj
        | TextDecorationColor of obj
        | TextDecorationLine of obj
        | TextDecorationLineThrough of obj
        | TextDecorationNone of obj
        | TextDecorationOverline of obj
        | TextDecorationSkip of obj
        | TextDecorationStyle of obj
        | TextDecorationUnderline of obj
        | TextEmphasis of obj
        | TextEmphasisColor of obj
        | TextEmphasisStyle of obj
        | TextHeight of obj
        | TextIndent of obj
        | TextJustifyTrim of obj
        | TextKashidaSpace of obj
        | TextLineThrough of obj
        | TextLineThroughColor of obj
        | TextLineThroughMode of obj
        | TextLineThroughStyle of obj
        | TextLineThroughWidth of obj
        | TextOverflow of obj
        | TextOverline of obj
        | TextOverlineColor of obj
        | TextOverlineMode of obj
        | TextOverlineStyle of obj
        | TextOverlineWidth of obj
        | TextRendering of obj
        | TextScript of obj
        | TextShadow of obj
        | TextTransform of obj
        | TextUnderlinePosition of obj
        | TextUnderlineStyle of obj
        | Top of obj
        | TouchAction of obj
        | Transform of obj
        | TransformOrigin of obj
        | TransformOriginZ of obj
        | TransformStyle of obj
        | Transition of obj
        | TransitionDelay of obj
        | TransitionDuration of obj
        | TransitionProperty of obj
        | TransitionTimingFunction of obj
        | UnicodeBidi of obj
        | UnicodeRange of obj
        | UserFocus of obj
        | UserInput of obj
        | VerticalAlign of obj
        | Visibility of obj
        | VoiceBalance of obj
        | VoiceDuration of obj
        | VoiceFamily of obj
        | VoicePitch of obj
        | VoiceRange of obj
        | VoiceRate of obj
        | VoiceStress of obj
        | VoiceVolume of obj
        | WhiteSpace of obj
        | WhiteSpaceTreatment of obj
        | Width of obj
        | WordBreak of obj
        | WordSpacing of obj
        | WordWrap of obj
        | WrapFlow of obj
        | WrapMargin of obj
        | WrapOption of obj
        | WritingMode of obj
        interface ICSSProp

    [<KeyValueList>]
    type IProp =
        interface end

    [<KeyValueList>]
    type IHTMLProp =
        inherit IProp

    [<KeyValueList>]
    type Prop =
        | Key of string
        | Ref of (Browser.Element->unit)
        interface IHTMLProp

    [<KeyValueList>]
    type Events =
        | [<Erase>] Extra of string * (obj -> unit)
        | Copy of (ClipboardEvent -> unit)
        | Cut of (ClipboardEvent -> unit)
        | Paste of (ClipboardEvent -> unit)
        | CompositionEnd of (CompositionEvent -> unit)
        | CompositionStart of (CompositionEvent -> unit)
        | CompositionUpdate of (CompositionEvent -> unit)
        | Focus of (FocusEvent -> unit)
        | Blur of (FocusEvent -> unit)
        | Change of (Event -> unit)
        | Input of (Event -> unit)
        | Submit of (Event -> unit)
        | Load of (Event -> unit)
        | Error of (Event -> unit)
        | KeyDown of (KeyboardEvent -> unit)
        | KeyPress of (KeyboardEvent -> unit)
        | KeyUp of (KeyboardEvent -> unit)
        | Abort of (Event -> unit)
        | CanPlay of (Event -> unit)
        | CanPlayThrough of (Event -> unit)
        | DurationChange of (Event -> unit)
        | Emptied of (Event -> unit)
        | Encrypted of (Event -> unit)
        | Ended of (Event -> unit)
        | LoadedData of (Event -> unit)
        | LoadedMetadata of (Event -> unit)
        | LoadStart of (Event -> unit)
        | Pause of (Event -> unit)
        | Play of (Event -> unit)
        | Playing of (Event -> unit)
        | Progress of (Event -> unit)
        | RateChange of (Event -> unit)
        | Seeked of (Event -> unit)
        | Seeking of (Event -> unit)
        | Stalled of (Event -> unit)
        | Suspend of (Event -> unit)
        | TimeUpdate of (Event -> unit)
        | VolumeChange of (Event -> unit)
        | Waiting of (Event -> unit)
        | Click of (MouseEvent -> unit)
        | ContextMenu of (MouseEvent -> unit)
        | DoubleClick of (MouseEvent -> unit)
        | Drag of (DragEvent -> unit)
        | DragEnd of (DragEvent -> unit)
        | DragEnter of (DragEvent -> unit)
        | DragExit of (DragEvent -> unit)
        | DragLeave of (DragEvent -> unit)
        | DragOver of (DragEvent -> unit)
        | DragStart of (DragEvent -> unit)
        | Drop of (DragEvent -> unit)
        | MouseDown of (MouseEvent -> unit)
        | MouseEnter of (MouseEvent -> unit)
        | MouseLeave of (MouseEvent -> unit)
        | MouseMove of (MouseEvent -> unit)
        | MouseOut of (MouseEvent -> unit)
        | MouseOver of (MouseEvent -> unit)
        | MouseUp of (MouseEvent -> unit)
        | Select of (Event -> unit)
        | TouchCancel of (TouchEvent -> unit)
        | TouchEnd of (TouchEvent -> unit)
        | TouchMove of (TouchEvent -> unit)
        | TouchStart of (TouchEvent -> unit)
        | Scroll of (UIEvent -> unit)
        | Wheel of (WheelEvent -> unit)
        interface IHTMLProp

    [<KeyValueList>]
    type HTMLAttr =
        | [<Erase>] Extra of string * string
        | DefaultChecked of bool
        | DefaultValue of U2<string, ResizeArray<string>>
        | Accept of string
        | AcceptCharset of string
        | AccessKey of string
        | Action of string
        | AllowFullScreen of bool
        | AllowTransparency of bool
        | Alt of string
        | [<CompiledName("aria-haspopup")>] AriaHasPopup of bool
        | [<CompiledName("aria-expanded")>] AriaExpanded of bool
        | Async of bool
        | AutoComplete of string
        | AutoFocus of bool
        | AutoPlay of bool
        | Capture of bool
        | CellPadding of U2<float, string>
        | CellSpacing of U2<float, string>
        | CharSet of string
        | Challenge of string
        | Checked of bool
        | ClassID of string
        | ClassName of string
        | Cols of float
        | ColSpan of float
        | Content of string
        | ContentEditable of bool
        | ContextMenu of string
        | Controls of bool
        | Coords of string
        | CrossOrigin of string
        | Data of string
        | [<CompiledName("data-toggle")>] DataToggle of string
        | DateTime of string
        | Default of bool
        | Defer of bool
        | Dir of string
        | Disabled of bool
        | Download of obj
        | Draggable of bool
        | EncType of string
        | Form of string
        | FormAction of string
        | FormEncType of string
        | FormMethod of string
        | FormNoValidate of bool
        | FormTarget of string
        | FrameBorder of U2<float, string>
        | Headers of string
        // | Height of U2<float, string> // Conflicts with CSSProp, shouldn't be used in HTML5
        | Hidden of bool
        | High of float
        | Href of string
        | HrefLang of string
        | HtmlFor of string
        | HttpEquiv of string
        | Icon of string
        | Id of string
        | InputMode of string
        | Integrity of string
        | Is of string
        | KeyParams of string
        | KeyType of string
        | Kind of string
        | Label of string
        | Lang of string
        | List of string
        | Loop of bool
        | Low of float
        | Manifest of string
        | MarginHeight of float
        | MarginWidth of float
        | Max of U2<float, string>
        | MaxLength of float
        | Media of string
        | MediaGroup of string
        | Method of string
        | Min of U2<float, string>
        | MinLength of float
        | Multiple of bool
        | Muted of bool
        | Name of string
        | NoValidate of bool
        | Open of bool
        | Optimum of float
        | Pattern of string
        | Placeholder of string
        | Poster of string
        | Preload of string
        | RadioGroup of string
        | ReadOnly of bool
        | Rel of string
        | Required of bool
        | Role of string
        | Rows of float
        | RowSpan of float
        | Sandbox of string
        | Scope of string
        | Scoped of bool
        | Scrolling of string
        | Seamless of bool
        | Selected of bool
        | Shape of string
        | Size of float
        | Sizes of string
        | Span of float
        | SpellCheck of bool
        | Src of string
        | SrcDoc of string
        | SrcLang of string
        | SrcSet of string
        | Start of float
        | Step of U2<float, string>
        | Style of ICSSProp list
        | Summary of string
        | TabIndex of float
        | Target of string
        | Title of string
        | Type of string
        | UseMap of string
        | Value of U2<string, ResizeArray<string>>
        | Width of U2<float, string>
        | Wmode of string
        | Wrap of string
        | About of string
        | Datatype of string
        | Inlist of obj
        | Prefix of string
        | Property of string
        | Resource of string
        | Typeof of string
        | Vocab of string
        | AutoCapitalize of string
        | AutoCorrect of string
        | AutoSave of string
        // | Color of string // Conflicts with CSSProp, shouldn't be used in HTML5
        | ItemProp of string
        | ItemScope of bool
        | ItemType of string
        | ItemID of string
        | ItemRef of string
        | Results of float
        | Security of string
        | Unselectable of bool
        interface IHTMLProp

    [<KeyValueList>]
    type SVGAttr =
        | [<Erase>] Extra of string * string
        | ClipPath of string
        | Cx of U2<float, string>
        | Cy of U2<float, string>
        | D of string
        | Dx of U2<float, string>
        | Dy of U2<float, string>
        | Fill of string
        | FillOpacity of U2<float, string>
        | FontFamily of string
        | FontSize of U2<float, string>
        | Fx of U2<float, string>
        | Fy of U2<float, string>
        | GradientTransform of string
        | GradientUnits of string
        | MarkerEnd of string
        | MarkerMid of string
        | MarkerStart of string
        | Offset of U2<float, string>
        | Opacity of U2<float, string>
        | PatternContentUnits of string
        | PatternUnits of string
        | Points of string
        | PreserveAspectRatio of string
        | R of U2<float, string>
        | Rx of U2<float, string>
        | Ry of U2<float, string>
        | SpreadMethod of string
        | StopColor of string
        | StopOpacity of U2<float, string>
        | Stroke of string
        | StrokeDasharray of string
        | StrokeLinecap of string
        | StrokeMiterlimit of string
        | StrokeOpacity of U2<float, string>
        | StrokeWidth of U2<float, string>
        | TextAnchor of string
        | Transform of string
        | Version of string
        | ViewBox of string
        | X1 of U2<float, string>
        | X2 of U2<float, string>
        | X of U2<float, string>
        | XlinkActuate of string
        | XlinkArcrole of string
        | XlinkHref of string
        | XlinkRole of string
        | XlinkShow of string
        | XlinkTitle of string
        | XlinkType of string
        | XmlBase of string
        | XmlLang of string
        | XmlSpace of string
        | Y1 of U2<float, string>
        | Y2 of U2<float, string>
        | Y of U2<float, string>
        interface IProp

    [<KeyValueList>]
    type IClass =
        | [<Erase>] Extra of string * bool

    [<KeyValueList>]
    type VNodeData =
        | Props of IProp list
        | Attrs of HTMLAttr list
        | Class of IClass list
        | Style of ICSSProp list
        | Dataset of obj
        | On of Events list
        | Hero of obj
        | AttachData of obj
        | Hook of obj//Hooks
        | Key of obj//Key
        | Ns of string
        | Fn of obj//(unit, VNode>
        | Args of ResizeArray<obj>

open Props
open Fable.Snabbdom.Internal
open Fable.Import.Snabbdom

[<Emit(typeof<Emitter>, "From")>]
let hyperscript (tag: string) (props: VNodeData list) (children: VNode list): VNode = jsNative

[<Emit(typeof<Emitter>, "Tagged", "a")>]
let a b c = hyperscript "a" b c
[<Emit(typeof<Emitter>, "Tagged", "abbr")>]
let abbr b c = hyperscript "abbr" b c
[<Emit(typeof<Emitter>, "Tagged", "address")>]
let address b c = hyperscript "address" b c
[<Emit(typeof<Emitter>, "Tagged", "area")>]
let area b c = hyperscript "area" b c
[<Emit(typeof<Emitter>, "Tagged", "article")>]
let article b c = hyperscript "article" b c
[<Emit(typeof<Emitter>, "Tagged", "aside")>]
let aside b c = hyperscript "aside" b c
[<Emit(typeof<Emitter>, "Tagged", "audio")>]
let audio b c = hyperscript "audio" b c
[<Emit(typeof<Emitter>, "Tagged", "b")>]
let b b' c = hyperscript "b" b' c
[<Emit(typeof<Emitter>, "Tagged", "base")>]
let ``base`` b c = hyperscript "base" b c
[<Emit(typeof<Emitter>, "Tagged", "bdi")>]
let bdi b c = hyperscript "bdi" b c
[<Emit(typeof<Emitter>, "Tagged", "bdo")>]
let bdo b c = hyperscript "bdo" b c
[<Emit(typeof<Emitter>, "Tagged", "big")>]
let big b c = hyperscript "big" b c
[<Emit(typeof<Emitter>, "Tagged", "blockquote")>]
let blockquote b c = hyperscript "blockquote" b c
[<Emit(typeof<Emitter>, "Tagged", "body")>]
let body b c = hyperscript "body" b c
[<Emit(typeof<Emitter>, "Tagged", "br")>]
let br b c = hyperscript "br" b c
[<Emit(typeof<Emitter>, "Tagged", "button")>]
let button b c = hyperscript "button" b c
[<Emit(typeof<Emitter>, "Tagged", "canvas")>]
let canvas b c = hyperscript "canvas" b c
[<Emit(typeof<Emitter>, "Tagged", "caption")>]
let caption b c = hyperscript "caption" b c
[<Emit(typeof<Emitter>, "Tagged", "cite")>]
let cite b c = hyperscript "cite" b c
[<Emit(typeof<Emitter>, "Tagged", "code")>]
let code b c = hyperscript "code" b c
[<Emit(typeof<Emitter>, "Tagged", "col")>]
let col b c = hyperscript "col" b c
[<Emit(typeof<Emitter>, "Tagged", "colgroup")>]
let colgroup b c = hyperscript "colgroup" b c
[<Emit(typeof<Emitter>, "Tagged", "data")>]
let data b c = hyperscript "data" b c
[<Emit(typeof<Emitter>, "Tagged", "datalist")>]
let datalist b c = hyperscript "datalist" b c
[<Emit(typeof<Emitter>, "Tagged", "dd")>]
let dd b c = hyperscript "dd" b c
[<Emit(typeof<Emitter>, "Tagged", "del")>]
let del b c = hyperscript "del" b c
[<Emit(typeof<Emitter>, "Tagged", "details")>]
let details b c = hyperscript "details" b c
[<Emit(typeof<Emitter>, "Tagged", "dfn")>]
let dfn b c = hyperscript "dfn" b c
[<Emit(typeof<Emitter>, "Tagged", "dialog")>]
let dialog b c = hyperscript "dialog" b c
[<Emit(typeof<Emitter>, "Tagged", "div")>]
let div b c = hyperscript "div" b c
[<Emit(typeof<Emitter>, "Tagged", "dl")>]
let dl b c = hyperscript "dl" b c
[<Emit(typeof<Emitter>, "Tagged", "dt")>]
let dt b c = hyperscript "dt" b c
[<Emit(typeof<Emitter>, "Tagged", "em")>]
let em b c = hyperscript "em" b c
[<Emit(typeof<Emitter>, "Tagged", "embed")>]
let embed b c = hyperscript "embed" b c
[<Emit(typeof<Emitter>, "Tagged", "fieldset")>]
let fieldset b c = hyperscript "fieldset" b c
[<Emit(typeof<Emitter>, "Tagged", "figcaption")>]
let figcaption b c = hyperscript "figcaption" b c
[<Emit(typeof<Emitter>, "Tagged", "figure")>]
let figure b c = hyperscript "figure" b c
[<Emit(typeof<Emitter>, "Tagged", "footer")>]
let footer b c = hyperscript "footer" b c
[<Emit(typeof<Emitter>, "Tagged", "form")>]
let form b c = hyperscript "form" b c
[<Emit(typeof<Emitter>, "Tagged", "h1")>]
let h1 b c = hyperscript "h1" b c
[<Emit(typeof<Emitter>, "Tagged", "h2")>]
let h2 b c = hyperscript "h2" b c
[<Emit(typeof<Emitter>, "Tagged", "h3")>]
let h3 b c = hyperscript "h3" b c
[<Emit(typeof<Emitter>, "Tagged", "h4")>]
let h4 b c = hyperscript "h4" b c
[<Emit(typeof<Emitter>, "Tagged", "h5")>]
let h5 b c = hyperscript "h5" b c
[<Emit(typeof<Emitter>, "Tagged", "h6")>]
let h6 b c = hyperscript "h6" b c
[<Emit(typeof<Emitter>, "Tagged", "head")>]
let head b c = hyperscript "head" b c
[<Emit(typeof<Emitter>, "Tagged", "header")>]
let header b c = hyperscript "header" b c
[<Emit(typeof<Emitter>, "Tagged", "hgroup")>]
let hgroup b c = hyperscript "hgroup" b c
[<Emit(typeof<Emitter>, "Tagged", "hr")>]
let hr b c = hyperscript "hr" b c
[<Emit(typeof<Emitter>, "Tagged", "html")>]
let html b c = hyperscript "html" b c
[<Emit(typeof<Emitter>, "Tagged", "i")>]
let i b c = hyperscript "i" b c
[<Emit(typeof<Emitter>, "Tagged", "iframe")>]
let iframe b c = hyperscript "iframe" b c
[<Emit(typeof<Emitter>, "Tagged", "img")>]
let img b c = hyperscript "img" b c
[<Emit(typeof<Emitter>, "Tagged", "input")>]
let input b c = hyperscript "input" b c
[<Emit(typeof<Emitter>, "Tagged", "ins")>]
let ins b c = hyperscript "ins" b c
[<Emit(typeof<Emitter>, "Tagged", "kbd")>]
let kbd b c = hyperscript "kbd" b c
[<Emit(typeof<Emitter>, "Tagged", "keygen")>]
let keygen b c = hyperscript "keygen" b c
[<Emit(typeof<Emitter>, "Tagged", "label")>]
let label b c = hyperscript "label" b c
[<Emit(typeof<Emitter>, "Tagged", "legend")>]
let legend b c = hyperscript "legend" b c
[<Emit(typeof<Emitter>, "Tagged", "li")>]
let li b c = hyperscript "li" b c
[<Emit(typeof<Emitter>, "Tagged", "link")>]
let link b c = hyperscript "link" b c
[<Emit(typeof<Emitter>, "Tagged", "main")>]
let main b c = hyperscript "main" b c
[<Emit(typeof<Emitter>, "Tagged", "map")>]
let map b c = hyperscript "map" b c
[<Emit(typeof<Emitter>, "Tagged", "mark")>]
let mark b c = hyperscript "mark" b c
[<Emit(typeof<Emitter>, "Tagged", "menu")>]
let menu b c = hyperscript "menu" b c
[<Emit(typeof<Emitter>, "Tagged", "menuitem")>]
let menuitem b c = hyperscript "menuitem" b c
[<Emit(typeof<Emitter>, "Tagged", "meta")>]
let meta b c = hyperscript "meta" b c
[<Emit(typeof<Emitter>, "Tagged", "meter")>]
let meter b c = hyperscript "meter" b c
[<Emit(typeof<Emitter>, "Tagged", "nav")>]
let nav b c = hyperscript "nav" b c
[<Emit(typeof<Emitter>, "Tagged", "noscript")>]
let noscript b c = hyperscript "noscript" b c
[<Emit(typeof<Emitter>, "Tagged", "object")>]
let ``object`` b c = hyperscript "object" b c
[<Emit(typeof<Emitter>, "Tagged", "ol")>]
let ol b c = hyperscript "ol" b c
[<Emit(typeof<Emitter>, "Tagged", "optgroup")>]
let optgroup b c = hyperscript "optgroup" b c
[<Emit(typeof<Emitter>, "Tagged", "option")>]
let option b c = hyperscript "option" b c
[<Emit(typeof<Emitter>, "Tagged", "output")>]
let output b c = hyperscript "output" b c
[<Emit(typeof<Emitter>, "Tagged", "p")>]
let p b c = hyperscript "p" b c
[<Emit(typeof<Emitter>, "Tagged", "param")>]
let param b c = hyperscript "param" b c
[<Emit(typeof<Emitter>, "Tagged", "picture")>]
let picture b c = hyperscript "picture" b c
[<Emit(typeof<Emitter>, "Tagged", "pre")>]
let pre b c = hyperscript "pre" b c
[<Emit(typeof<Emitter>, "Tagged", "progress")>]
let progress b c = hyperscript "progress" b c
[<Emit(typeof<Emitter>, "Tagged", "q")>]
let q b c = hyperscript "q" b c
[<Emit(typeof<Emitter>, "Tagged", "rp")>]
let rp b c = hyperscript "rp" b c
[<Emit(typeof<Emitter>, "Tagged", "rt")>]
let rt b c = hyperscript "rt" b c
[<Emit(typeof<Emitter>, "Tagged", "ruby")>]
let ruby b c = hyperscript "ruby" b c
[<Emit(typeof<Emitter>, "Tagged", "s")>]
let s b c = hyperscript "s" b c
[<Emit(typeof<Emitter>, "Tagged", "samp")>]
let samp b c = hyperscript "samp" b c
[<Emit(typeof<Emitter>, "Tagged", "script")>]
let script b c = hyperscript "script" b c
[<Emit(typeof<Emitter>, "Tagged", "section")>]
let section b c = hyperscript "section" b c
[<Emit(typeof<Emitter>, "Tagged", "select")>]
let select b c = hyperscript "select" b c
[<Emit(typeof<Emitter>, "Tagged", "small")>]
let small b c = hyperscript "small" b c
[<Emit(typeof<Emitter>, "Tagged", "source")>]
let source b c = hyperscript "source" b c
[<Emit(typeof<Emitter>, "Tagged", "span")>]
let span b c = hyperscript "span" b c
[<Emit(typeof<Emitter>, "Tagged", "strong")>]
let strong b c = hyperscript "strong" b c
[<Emit(typeof<Emitter>, "Tagged", "style")>]
let style b c = hyperscript "style" b c
[<Emit(typeof<Emitter>, "Tagged", "sub")>]
let sub b c = hyperscript "sub" b c
[<Emit(typeof<Emitter>, "Tagged", "summary")>]
let summary b c = hyperscript "summary" b c
[<Emit(typeof<Emitter>, "Tagged", "sup")>]
let sup b c = hyperscript "sup" b c
[<Emit(typeof<Emitter>, "Tagged", "table")>]
let table b c = hyperscript "table" b c
[<Emit(typeof<Emitter>, "Tagged", "tbody")>]
let tbody b c = hyperscript "tbody" b c
[<Emit(typeof<Emitter>, "Tagged", "td")>]
let td b c = hyperscript "td" b c
[<Emit(typeof<Emitter>, "Tagged", "textarea")>]
let textarea b c = hyperscript "textarea" b c
[<Emit(typeof<Emitter>, "Tagged", "tfoot")>]
let tfoot b c = hyperscript "tfoot" b c
[<Emit(typeof<Emitter>, "Tagged", "th")>]
let th b c = hyperscript "th" b c
[<Emit(typeof<Emitter>, "Tagged", "thead")>]
let thead b c = hyperscript "thead" b c
[<Emit(typeof<Emitter>, "Tagged", "time")>]
let time b c = hyperscript "time" b c
[<Emit(typeof<Emitter>, "Tagged", "title")>]
let title b c = hyperscript "title" b c
[<Emit(typeof<Emitter>, "Tagged", "tr")>]
let tr b c = hyperscript "tr" b c
[<Emit(typeof<Emitter>, "Tagged", "track")>]
let track b c = hyperscript "track" b c
[<Emit(typeof<Emitter>, "Tagged", "u")>]
let u b c = hyperscript "u" b c
[<Emit(typeof<Emitter>, "Tagged", "ul")>]
let ul b c = hyperscript "ul" b c
[<Emit(typeof<Emitter>, "Tagged", "var")>]
let var b c = hyperscript "var" b c
[<Emit(typeof<Emitter>, "Tagged", "video")>]
let video b c = hyperscript "video" b c
[<Emit(typeof<Emitter>, "Tagged", "wbr")>]
let wbr b c = hyperscript "wbr" b c
[<Emit(typeof<Emitter>, "Tagged", "svg")>]
let svg b c = hyperscript "svg" b c
[<Emit(typeof<Emitter>, "Tagged", "circle")>]
let circle b c = hyperscript "circle" b c
[<Emit(typeof<Emitter>, "Tagged", "clipPath")>]
let clipPath b c = hyperscript "clipPath" b c
[<Emit(typeof<Emitter>, "Tagged", "defs")>]
let defs b c = hyperscript "defs" b c
[<Emit(typeof<Emitter>, "Tagged", "ellipse")>]
let ellipse b c = hyperscript "ellipse" b c
[<Emit(typeof<Emitter>, "Tagged", "g")>]
let g b c = hyperscript "g" b c
[<Emit(typeof<Emitter>, "Tagged", "image")>]
let image b c = hyperscript "image" b c
[<Emit(typeof<Emitter>, "Tagged", "line")>]
let line b c = hyperscript "line" b c
[<Emit(typeof<Emitter>, "Tagged", "linearGradient")>]
let linearGradient b c = hyperscript "linearGradient" b c
[<Emit(typeof<Emitter>, "Tagged", "mask")>]
let mask b c = hyperscript "mask" b c
[<Emit(typeof<Emitter>, "Tagged", "path")>]
let path b c = hyperscript "path" b c
[<Emit(typeof<Emitter>, "Tagged", "pattern")>]
let pattern b c = hyperscript "pattern" b c
[<Emit(typeof<Emitter>, "Tagged", "polygon")>]
let polygon b c = hyperscript "polygon" b c
[<Emit(typeof<Emitter>, "Tagged", "polyline")>]
let polyline b c = hyperscript "polyline" b c
[<Emit(typeof<Emitter>, "Tagged", "radialGradient")>]
let radialGradient b c = hyperscript "radialGradient" b c
[<Emit(typeof<Emitter>, "Tagged", "rect")>]
let rect b c = hyperscript "rect" b c
[<Emit(typeof<Emitter>, "Tagged", "stop")>]
let stop b c = hyperscript "stop" b c
[<Emit(typeof<Emitter>, "Tagged", "text")>]
let text b c = hyperscript "text" b c
[<Emit(typeof<Emitter>, "Tagged", "tspan")>]
let tspan b c = hyperscript "tspan" b c

/// Cast a string to a VNode element (erased in runtime)
// let [<Emit("$0")>] str (s: string): VNode = unbox s
/// Cast an option value to a VNode element (erased in runtime)
// let [<Emit("$0")>] opt (o: VNode option): VNode = unbox o
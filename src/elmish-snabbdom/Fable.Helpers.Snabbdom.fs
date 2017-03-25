module Fable.Helpers.Snabbdom

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser

module Props =

    type CSSProp =
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
        static member Unsafe(key: string, value: string): CSSProp = !!(key, value)

    type Events =
        | [<CompiledName("copy")>] OnCopy of (ClipboardEvent -> unit)
        | [<CompiledName("cut")>] OnCut of (ClipboardEvent -> unit)
        | [<CompiledName("paste")>] OnPaste of (ClipboardEvent -> unit)
        | [<CompiledName("compositionend")>] OnCompositionEnd of (CompositionEvent -> unit)
        | [<CompiledName("compositionstart")>] OnCompositionStart of (CompositionEvent -> unit)
        | [<CompiledName("compositionupdate")>] OnCompositionUpdate of (CompositionEvent -> unit)
        | [<CompiledName("focus")>] OnFocus of (FocusEvent -> unit)
        | [<CompiledName("blur")>] OnBlur of (FocusEvent -> unit)
        | [<CompiledName("change")>] OnChange of (Event -> unit)
        | [<CompiledName("input")>] OnInput of (Event -> unit)
        | [<CompiledName("submit")>] OnSubmit of (Event -> unit)
        | [<CompiledName("load")>] OnLoad of (Event -> unit)
        | [<CompiledName("error")>] OnError of (Event -> unit)
        | [<CompiledName("keydown")>] OnKeyDown of (KeyboardEvent -> unit)
        | [<CompiledName("keypress")>] OnKeyPress of (KeyboardEvent -> unit)
        | [<CompiledName("keyup")>] OnKeyUp of (KeyboardEvent -> unit)
        | [<CompiledName("abort")>] OnAbort of (Event -> unit)
        | [<CompiledName("canplay")>] OnCanPlay of (Event -> unit)
        | [<CompiledName("canplaythrough")>] OnCanPlayThrough of (Event -> unit)
        | [<CompiledName("durationchange")>] OnDurationChange of (Event -> unit)
        | [<CompiledName("emptied")>] OnEmptied of (Event -> unit)
        | [<CompiledName("encrypted")>] OnEncrypted of (Event -> unit)
        | [<CompiledName("ended")>] OnEnded of (Event -> unit)
        | [<CompiledName("loadeddata")>] OnLoadedData of (Event -> unit)
        | [<CompiledName("loadedmetadata")>] OnLoadedMetadata of (Event -> unit)
        | [<CompiledName("loadstart")>] OnLoadStart of (Event -> unit)
        | [<CompiledName("pause")>] OnPause of (Event -> unit)
        | [<CompiledName("play")>] OnPlay of (Event -> unit)
        | [<CompiledName("playing")>] OnPlaying of (Event -> unit)
        | [<CompiledName("progress")>] OnProgress of (Event -> unit)
        | [<CompiledName("ratechange")>] OnRateChange of (Event -> unit)
        | [<CompiledName("seeked")>] OnSeeked of (Event -> unit)
        | [<CompiledName("seeking")>] OnSeeking of (Event -> unit)
        | [<CompiledName("stalled")>] OnStalled of (Event -> unit)
        | [<CompiledName("suspend")>] OnSuspend of (Event -> unit)
        | [<CompiledName("timeupdate")>] OnTimeUpdate of (Event -> unit)
        | [<CompiledName("volumechange")>] OnVolumeChange of (Event -> unit)
        | [<CompiledName("waiting")>] OnWaiting of (Event -> unit)
        | [<CompiledName("click")>] OnClick of (MouseEvent -> unit)
        | [<CompiledName("contextmenu")>] OnContextMenu of (MouseEvent -> unit)
        | [<CompiledName("dblclick")>] OnDoubleClick of (MouseEvent -> unit)
        | [<CompiledName("drag")>] OnDrag of (DragEvent -> unit)
        | [<CompiledName("dragend")>] OnDragEnd of (DragEvent -> unit)
        | [<CompiledName("dragenter")>] OnDragEnter of (DragEvent -> unit)
        | [<CompiledName("dragexit")>] OnDragExit of (DragEvent -> unit)
        | [<CompiledName("dragleave")>] OnDragLeave of (DragEvent -> unit)
        | [<CompiledName("dragover")>] OnDragOver of (DragEvent -> unit)
        | [<CompiledName("dragstart")>] OnDragStart of (DragEvent -> unit)
        | [<CompiledName("drop")>] OnDrop of (DragEvent -> unit)
        | [<CompiledName("mousedown")>] OnMouseDown of (MouseEvent -> unit)
        | [<CompiledName("mouseenter")>] OnMouseEnter of (MouseEvent -> unit)
        | [<CompiledName("mouseleave")>] OnMouseLeave of (MouseEvent -> unit)
        | [<CompiledName("mousemove")>] OnMouseMove of (MouseEvent -> unit)
        | [<CompiledName("mouseout")>] OnMouseOut of (MouseEvent -> unit)
        | [<CompiledName("mouseover")>] OnMouseOver of (MouseEvent -> unit)
        | [<CompiledName("mouseup")>] OnMouseUp of (MouseEvent -> unit)
        | [<CompiledName("select")>] OnSelect of (Event -> unit)
        | [<CompiledName("touchcancel")>] OnTouchCancel of (TouchEvent -> unit)
        | [<CompiledName("touchend")>] OnTouchEnd of (TouchEvent -> unit)
        | [<CompiledName("touchmove")>] OnTouchMove of (TouchEvent -> unit)
        | [<CompiledName("touchstart")>] OnTouchStart of (TouchEvent -> unit)
        | [<CompiledName("scroll")>] OnScroll of (UIEvent -> unit)
        | [<CompiledName("wheel")>] OnWheel of (WheelEvent -> unit)
        static member Unsafe(key: string, handler: (obj -> unit)): Events = !!(key, handler)

    type IAttr = interface end
    type IProp = interface end

    type HTMLAttr =
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
        | Style of CSSProp list
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
        interface IAttr
        interface IProp
        static member Unsafe(key: string, value: string): HTMLAttr = !!(key, value)

    type SVGAttr =
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
        interface IAttr
        static member Unsafe(key: string, value: string): SVGAttr = !!(key, value)

    type VNodeData = class end
    let inline props (props: IProp list): VNodeData = !!("props", keyValueList CaseRules.LowerFirst props)
    let inline attrs (attrs: IAttr list): VNodeData = !!("attrs", keyValueList CaseRules.LowerFirst attrs)
    let inline classes (classes: (string*bool) list): VNodeData = !!("class", keyValueList CaseRules.None classes)
    let inline style (style: CSSProp list): VNodeData = !!("style", keyValueList CaseRules.LowerFirst style)
    let inline events (events: Events list): VNodeData = !!("on", keyValueList CaseRules.LowerFirst events)
    let inline dataset (dataset: obj): VNodeData = !!("dataset", dataset)
    let inline hero (hero: obj): VNodeData = !!("hero", hero)
    let inline attachData (attachData: obj): VNodeData = !!("attachData", attachData)
    let inline hook (hook: obj): VNodeData = !!("hook", hook)
    let inline key (key: obj): VNodeData = !!("key", key)
    let inline ns (ns: string): VNodeData = !!("ns", ns)
    let inline fn (fn: obj): VNodeData = !!("fn", fn)
    let inline args (args: obj[]): VNodeData = !!("args", args)

open Props
open Fable.Import.Snabbdom

let classBaseList std classes =
    classes
    |> List.fold (fun complete -> function | (name,true) -> complete + " " + name | _ -> complete) std
    |> ClassName

let classList classes = classBaseList "" classes

[<Import("h", from="snabbdom")>]
let createEl = obj()

let inline hyperscript (tag: string) (props: VNodeData list) (children: VNode list): VNode =
    !!(createEl $ (tag, keyValueList CaseRules.None props, List.toArray children))

let inline hyperscript2 (tag: string) (props: VNodeData list): VNode =
    !!(createEl $ (tag, keyValueList CaseRules.None props))

let inline a b c = hyperscript "a" b c
let inline abbr b c = hyperscript "abbr" b c
let inline address b c = hyperscript "address" b c
let inline article b c = hyperscript "article" b c
let inline aside b c = hyperscript "aside" b c
let inline audio b c = hyperscript "audio" b c
let inline b b' c = hyperscript "b" b' c
let inline bdi b c = hyperscript "bdi" b c
let inline bdo b c = hyperscript "bdo" b c
let inline big b c = hyperscript "big" b c
let inline blockquote b c = hyperscript "blockquote" b c
let inline body b c = hyperscript "body" b c
let inline button b c = hyperscript "button" b c
let inline canvas b c = hyperscript "canvas" b c
let inline caption b c = hyperscript "caption" b c
let inline cite b c = hyperscript "cite" b c
let inline code b c = hyperscript "code" b c
let inline colgroup b c = hyperscript "colgroup" b c
let inline data b c = hyperscript "data" b c
let inline datalist b c = hyperscript "datalist" b c
let inline dd b c = hyperscript "dd" b c
let inline del b c = hyperscript "del" b c
let inline details b c = hyperscript "details" b c
let inline dfn b c = hyperscript "dfn" b c
let inline dialog b c = hyperscript "dialog" b c
let inline div b c = hyperscript "div" b c
let inline dl b c = hyperscript "dl" b c
let inline dt b c = hyperscript "dt" b c
let inline em b c = hyperscript "em" b c
let inline fieldset b c = hyperscript "fieldset" b c
let inline figcaption b c = hyperscript "figcaption" b c
let inline figure b c = hyperscript "figure" b c
let inline footer b c = hyperscript "footer" b c
let inline form b c = hyperscript "form" b c
let inline h1 b c = hyperscript "h1" b c
let inline h2 b c = hyperscript "h2" b c
let inline h3 b c = hyperscript "h3" b c
let inline h4 b c = hyperscript "h4" b c
let inline h5 b c = hyperscript "h5" b c
let inline h6 b c = hyperscript "h6" b c
let inline head b c = hyperscript "head" b c
let inline header b c = hyperscript "header" b c
let inline hgroup b c = hyperscript "hgroup" b c
let inline html b c = hyperscript "html" b c
let inline i b c = hyperscript "i" b c
let inline iframe b c = hyperscript "iframe" b c
let inline ins b c = hyperscript "ins" b c
let inline kbd b c = hyperscript "kbd" b c
let inline label b c = hyperscript "label" b c
let inline legend b c = hyperscript "legend" b c
let inline li b c = hyperscript "li" b c
let inline main b c = hyperscript "main" b c
let inline map b c = hyperscript "map" b c
let inline mark b c = hyperscript "mark" b c
let inline menu b c = hyperscript "menu" b c
let inline meter b c = hyperscript "meter" b c
let inline nav b c = hyperscript "nav" b c
let inline noscript b c = hyperscript "noscript" b c
let inline ``object`` b c = hyperscript "object" b c
let inline ol b c = hyperscript "ol" b c
let inline optgroup b c = hyperscript "optgroup" b c
let inline option b c = hyperscript "option" b c
let inline output b c = hyperscript "output" b c
let inline p b c = hyperscript "p" b c
let inline picture b c = hyperscript "picture" b c
let inline pre b c = hyperscript "pre" b c
let inline progress b c = hyperscript "progress" b c
let inline q b c = hyperscript "q" b c
let inline rp b c = hyperscript "rp" b c
let inline rt b c = hyperscript "rt" b c
let inline ruby b c = hyperscript "ruby" b c
let inline s b c = hyperscript "s" b c
let inline samp b c = hyperscript "samp" b c
let inline script b c = hyperscript "script" b c
let inline section b c = hyperscript "section" b c
let inline select b c = hyperscript "select" b c
let inline small b c = hyperscript "small" b c
let inline span b c = hyperscript "span" b c
let inline strong b c = hyperscript "strong" b c
let inline style b c = hyperscript "style" b c
let inline sub b c = hyperscript "sub" b c
let inline summary b c = hyperscript "summary" b c
let inline sup b c = hyperscript "sup" b c
let inline table b c = hyperscript "table" b c
let inline tbody b c = hyperscript "tbody" b c
let inline td b c = hyperscript "td" b c
let inline textarea b c = hyperscript "textarea" b c
let inline tfoot b c = hyperscript "tfoot" b c
let inline th b c = hyperscript "th" b c
let inline thead b c = hyperscript "thead" b c
let inline time b c = hyperscript "time" b c
let inline title b c = hyperscript "title" b c
let inline tr b c = hyperscript "tr" b c
let inline u b c = hyperscript "u" b c
let inline ul b c = hyperscript "ul" b c
let inline var b c = hyperscript "var" b c
let inline video b c = hyperscript "video" b c
// Void Elements
let inline area (b: VNodeData list) : VNode = hyperscript2 "area" b
let inline ``base`` (b: VNodeData list) : VNode = hyperscript2 "base" b
let inline br (b: VNodeData list) : VNode = hyperscript2 "br" b
let inline col (b: VNodeData list) : VNode = hyperscript2 "col" b
let inline embed (b: VNodeData list) : VNode = hyperscript2 "embed" b
let inline hr (b: VNodeData list) : VNode = hyperscript2 "hr" b
let inline img (b: VNodeData list) : VNode = hyperscript2 "img" b
let inline input (b: VNodeData list) : VNode = hyperscript2 "input" b
let inline keygen (b: VNodeData list) : VNode = hyperscript2 "keygen" b
let inline link (b: VNodeData list) : VNode = hyperscript2 "link" b
let inline menuitem (b: VNodeData list) : VNode = hyperscript2 "menuitem" b
let inline meta (b: VNodeData list) : VNode = hyperscript2 "meta" b
let inline param (b: VNodeData list) : VNode = hyperscript2 "param" b
let inline source (b: VNodeData list) : VNode = hyperscript2 "source" b
let inline track (b: VNodeData list) : VNode = hyperscript2 "track" b
let inline wbr (b: VNodeData list) : VNode = hyperscript2 "wbr" b
// SVG api
let inline svg b c = hyperscript "svg" b c
let inline circle b c = hyperscript "circle" b c
let inline clipPath b c = hyperscript "clipPath" b c
let inline defs b c = hyperscript "defs" b c
let inline ellipse b c = hyperscript "ellipse" b c
let inline g b c = hyperscript "g" b c
let inline image b c = hyperscript "image" b c
let inline line b c = hyperscript "line" b c
let inline linearGradient b c = hyperscript "linearGradient" b c
let inline mask b c = hyperscript "mask" b c
let inline path b c = hyperscript "path" b c
let inline pattern b c = hyperscript "pattern" b c
let inline polygon b c = hyperscript "polygon" b c
let inline polyline b c = hyperscript "polyline" b c
let inline radialGradient b c = hyperscript "radialGradient" b c
let inline rect b c = hyperscript "rect" b c
let inline stop b c = hyperscript "stop" b c
let inline text b c = hyperscript "text" b c
let inline tspan b c = hyperscript "tspan" b c

/// Cast a string to a VNode
let inline str (s: string): VNode = unbox s
/// Cast an option value to a VNode
let inline opt (o: VNode option): VNode = unbox o
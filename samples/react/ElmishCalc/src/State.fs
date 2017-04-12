module Calc.State

open Calc.Types

/// Active pattern that matches with an operation
let (|Operation|_|) = function 
    | Plus -> Some Plus
    | Minus -> Some Minus
    | Times -> Some Times
    | Div -> Some Div
    | _ -> None

/// Given a model, calculate the answer
let solve (state : Input list) = 
  match state with
  | [Const x; Operation op; Const y] ->
      match op with
      | Plus -> Some (x + y)
      | Minus -> Some (x - y)
      | Times -> Some (x * y)
      | Div when y = 0 -> None // division by zero not allowed
      | Div -> Some (x / y)
      | _ -> None
  | _ -> None


/// Given two integers, concat their string representation and parse as an integer
/// concatInts 3 5 -> 35 
/// concatInts 1 1 -> 11
let concatInts x y = int (sprintf "%d%d" x y)

let initialState() : Model = InputStack []

/// Given the input message and the state of the app, calculate the next state. This is known as the update function
let update (PushInput input) (InputStack xs)  =
    if input = Clear then InputStack []
    else
    match xs with
    | [] -> 
        match input with
        | Minus -> InputStack [Minus]
        | Operation op -> InputStack [ ]
        | Equals -> InputStack []
        | _ -> InputStack [input]
    | [Minus] ->
        match input with 
        | Const x -> InputStack [ Const (-x) ]
        | _ -> InputStack xs
    | [Const x] ->
        match input with
        | Const y -> InputStack [Const (concatInts x y)]
        | Operation op -> InputStack [Const x; op]
        | _ -> InputStack xs
    | [Const x; Operation op] ->  
        match input with
        | Const y -> InputStack [Const x; op; Const y] // push Const y to stack
        | Minus when op = Minus -> InputStack [Const x; Plus] // Minus Minus = Plus
        | Minus -> InputStack [Const x; op; Minus]
        | Operation otherOp -> InputStack [Const x; otherOp] // replace op with otherOp
        | _ -> InputStack xs // do nothing
    | [Const x; Operation op; Minus] ->
        match input with
        | Const y -> InputStack [Const x; op; Const (-y)]
        | _ -> InputStack xs
    | [Const x; Operation op; Const y] ->
        match input with
        | Const y' -> InputStack [Const x; op; Const (concatInts y y')]
        | Equals -> 
            match solve xs with
            | Some answer -> InputStack [Const answer]
            | None -> InputStack xs
        | Operation op -> 
            match solve xs with
            | Some answer -> InputStack [Const answer; op]
            | None -> InputStack xs
        | _ -> InputStack xs
    | _ -> InputStack xs
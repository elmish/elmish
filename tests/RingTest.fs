module Elmish.RingTests

open FsCheck.NUnit
open System.Collections.Generic
open Swensen.Unquote

type Op =
    | Pop
    | Push of int

type OpResult =
    | Poped of int option
    | Pushed

let internal applyQOps (ops:Op list) (q:int Queue) =
    ops
    |> List.map (function 
                 | Pop -> 
                   match q.TryDequeue() with (true, v) -> Some v | _ -> None
                   |> Poped
                 | Push v -> 
                   q.Enqueue v
                   Pushed)

let internal applyRBOps (ops:Op list) (rb:int RingBuffer) =
    ops
    |> List.map (function 
                 | Pop -> 
                   Poped <| rb.Pop()
                 | Push v -> 
                   rb.Push v
                   Pushed)
                   
[<Property(MaxTest = 1000, EndSize = 1000)>]
let actsLikeAQueue (ops:Op list) =  
    let q = Queue()
    let rb = RingBuffer 10
    (q |> applyQOps ops) =! (rb |> applyRBOps ops)

// have to have these here or dotnet won't find the nunit tests :/
open NUnit.Framework
[<Test>]
let ok() =
  1 =! 1
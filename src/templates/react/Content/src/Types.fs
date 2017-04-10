module Types.App

open Types
open Global

type Msg =
  | CounterMsg of Counter.Msg
  | HomeMsg of Home.Msg

type Model = {
    currentPage: Page
    counter: Counter.Model
    home: Home.Model
  }

### 0.1.0-alpha.2

* Prefix events by On for better DSL
* Fix emitter when dealing with partial functions
* Add helpers to write unsafe code
* Add helpers to work with classes
```fsharp
let text = [ unbox "..." ]

let test model dispatch =
  div
    [ centerStyle "column" ]
    text
```

### 0.1.0-alpha.1

* Kickstart the project

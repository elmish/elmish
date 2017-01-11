// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NpmHelper

// Directories
let buildDir  = "./build/"

// Filesets
let projects  =
      !! "src/*/fableconfig.json"
let installs  =
      !! "package.json"
      ++ "src/*/package.json"
      ++ "samples/*/package.json"

// Fable projects
let fables  =
      !! "samples/*/*/fableconfig.json"

// Artifact packages
let packages  =
      !! "src/*/package.json"

// version info
let version = "0.5"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Install" (fun _ ->
    installs
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Installing: %s\n" dir
                    Npm (fun p ->
                        { p with
                            Command = Install Standard
                            WorkingDirectory = dir
                        }))
)


Target "Build" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Building: %s\n" dir
                    Npm (fun p ->
                        { p with
                            Command = Run "build"
                            WorkingDirectory = dir
                        }))
)

Target "Samples" (fun _ ->
    fables
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Installing: %s\n" dir
                    printf "Building: %s\n" dir
                    Npm (fun p ->
                        { p with
                            Command = Run "build"
                            WorkingDirectory = dir
                        }))
)

Target "Publish-Elmish" (fun _ ->
    Npm (fun p ->
            { p with
                Command = Custom "publish"
                WorkingDirectory = "./src/elmish"
            })
)

Target "Publish-Elmish-React" (fun _ ->
    Npm (fun p ->
            { p with
                Command = Custom "publish"
                WorkingDirectory = "./src/elmish-react"
            })
)

// Build order
"Clean"
  ==> "Install"
  ==> "Build"
  
// start build
RunTargetOrDefault "Build"

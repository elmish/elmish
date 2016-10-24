// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NpmHelper

// Directories
let buildDir  = "./build/"

// Filesets
let projects  =
      !! "src/*/*.fsproj"

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

Target "Build" (fun _ ->
    // compile all projects below src/
    MSBuildDebug buildDir "Build" projects
        |> Log "AppBuild-Output: "
)

Target "Samples" (fun _ ->
    fables
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Building: %s\n" dir
                    Npm (fun p ->
                        { p with
                            Command = Run "firstbuild"
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
  ==> "Build"
  
// start build
RunTargetOrDefault "Build"

// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NpmHelper

// Directories
let buildDir  = "./build/"


// Filesets
let sources  =
      !! "src/*.fsproj"

// Samples
let samples  =
      !! "samples/**/fableconfig.json"

// version info
let version = "0.3"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/
    MSBuildDebug buildDir "Build" sources
        |> Log "AppBuild-Output: "
)

Target "Samples" (fun _ ->
    // compile all sample scripts
    samples
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    Npm (fun p ->
                        { p with
                            Command = Run "firstbuild"
                            WorkingDirectory = dir
                        }))
)


Target "Npm" (fun _ ->
    Npm (fun p ->
            { p with
                Command = Install Standard
                WorkingDirectory = "./src"
            })
)

Target "Publish" (fun _ ->
    Npm (fun p ->
            { p with
                Command = Custom "publish"
                WorkingDirectory = "./src"
            })
)

// Build order
"Clean"
  ==> "Npm"
  ==> "Build"

// start build
RunTargetOrDefault "Build"

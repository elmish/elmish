// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NpmHelper

// Directories
let buildDir  = "./build/"


// Filesets
let appReferences  =
      !! "src/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "Npm" (fun _ ->
    Npm (fun p ->
            { p with
                Command = Install Standard
                WorkingDirectory = "./src"
            })
)

// Build order
"Clean"
  ==> "Npm"
  ==> "Build"

// start build
RunTargetOrDefault "Build"

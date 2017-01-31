// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NpmHelper

let yarn = 
    if EnvironmentHelper.isWindows then "yarn.cmd" else "yarn"
    |> ProcessHelper.tryFindFileOnPath
    |> function
       | Some yarn -> yarn
       | ex -> failwith ( sprintf "yarn not found (%A)\n" ex )

// Directories
let buildDir  = "./build/"

// Install prereqs
let installs  =
        !! "package.json"

let samplesInstalls  =
        !! "samples/*/package.json"
        ++ "samples/react-native/*/package.json"
 
// Filesets
let projects  =
      !! "src/*/fableconfig.json"

// Fable projects
let fables  =
      !! "samples/*/*/fableconfig.json"

// Artifact packages
let packages  =
      !! "src/*/package.json"

// version info
let version = "0.5"  // or retrieve from CI server

Target "Install" (fun _ ->
    installs
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Installing: %s\n" dir
                    Npm (fun p ->
                        { p with
                            NpmFilePath = yarn
                            Command = Install Standard
                            WorkingDirectory = dir
                        }))
)

Target "InstallSamples" (fun _ ->
    samplesInstalls
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Installing for samples: %s\n" dir
                    Npm (fun p ->
                        { p with
                            NpmFilePath = yarn
                            Command = Install Standard
                            WorkingDirectory = dir
                        }))
)

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Building: %s\n" dir
                    Npm (fun p ->
                        { p with
                            NpmFilePath = yarn
                            Command = Run "build"
                            WorkingDirectory = dir
                        }))
)

Target "Samples" (fun _ ->
    fables
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Building: %s\n" dir
                    Npm (fun p ->
                        { p with
                            NpmFilePath = yarn
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

Target "All" ignore

// Build order
"Clean"
  ==> "Install"
  ==> "Build"

"InstallSamples"
  ==> "Samples"

"Build"
  ==> "Samples"
  ==> "All"
  
// start build
RunTargetOrDefault "Build"

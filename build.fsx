// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem"

open System
open System.IO
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
        ++ "samples/*/*/package.json"
 
// Filesets
let projects  =
      !! "src/*/*.fsproj"

// Fable projects
let fables  =
      !! "samples/**/fableconfig.json"

// Artifact packages
let packages  =
      !! "src/*/package.json"

let dotnetcliVersion = "1.0.1"
let dotnetSDKPath = 
    System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) </> "dotnetcore" 
    |> FullName

let dotnetExePath =
    dotnetSDKPath </> (if isWindows then "dotnet.exe" else "dotnet")
    |> FullName


Target "InstallDotNetCore" (fun _ ->
    let correctVersionInstalled = 
        try
            let processResult = 
                ExecProcessAndReturnMessages (fun info ->  
                info.FileName <- dotnetExePath
                info.WorkingDirectory <- Environment.CurrentDirectory
                info.Arguments <- "--version") (TimeSpan.FromMinutes 30.)

            processResult.Messages |> separated "" = dotnetcliVersion
        with 
        | _ -> false

    if correctVersionInstalled then
        tracefn "dotnetcli %s already installed" dotnetcliVersion
    else
        CleanDir dotnetSDKPath
        let archiveFileName = 
            if isWindows then
                sprintf "dotnet-dev-win-x64.%s.zip" dotnetcliVersion
            elif isLinux then
                sprintf "dotnet-dev-ubuntu-x64.%s.tar.gz" dotnetcliVersion
            else
                sprintf "dotnet-dev-osx-x64.%s.tar.gz" dotnetcliVersion
        let downloadPath = 
                sprintf "https://dotnetcli.azureedge.net/dotnet/Sdk/%s/%s" dotnetcliVersion archiveFileName
        let localPath = Path.Combine(dotnetSDKPath, archiveFileName)

        tracefn "Installing '%s' to '%s'" downloadPath localPath
        
        use webclient = new Net.WebClient()
        webclient.DownloadFile(downloadPath, localPath)

        if not isWindows then
            let assertExitCodeZero x =
                if x = 0 then () else
                failwithf "Command failed with exit code %i" x

            Shell.Exec("tar", sprintf """-xvf "%s" -C "%s" """ localPath dotnetSDKPath)
            |> assertExitCodeZero
        else  
            Compression.ZipFile.ExtractToDirectory(localPath, dotnetSDKPath)
        
        tracefn "dotnet cli path - %s" dotnetSDKPath
        System.IO.Directory.EnumerateFiles dotnetSDKPath
        |> Seq.iter (fun path -> tracefn " - %s" path)
        System.IO.Directory.EnumerateDirectories dotnetSDKPath
        |> Seq.iter (fun path -> tracefn " - %s%c" path System.IO.Path.DirectorySeparatorChar)

    let oldPath = System.Environment.GetEnvironmentVariable("PATH")
    System.Environment.SetEnvironmentVariable("PATH", sprintf "%s%s%s" dotnetSDKPath (System.IO.Path.PathSeparator.ToString()) oldPath)
)


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
                    let result =
                        ExecProcess (fun info ->
                            info.FileName <- dotnetExePath
                            info.WorkingDirectory <- dir
                            info.Arguments <- "build") TimeSpan.MaxValue
                    if result <> 0 then failwith "Build failed")
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
  ==> "InstallDotNetCore"
  ==> "Install"
  ==> "Build"

"InstallSamples"
  ==> "Samples"

"All"
  <== ["Build"; "Samples"]
  
// start build
RunTargetOrDefault "Build"

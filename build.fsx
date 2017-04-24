// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem"

open System
open System.IO
open Fake
open Fake.NpmHelper
open Fake.ReleaseNotesHelper

let yarn = 
    if EnvironmentHelper.isWindows then "yarn.cmd" else "yarn"
    |> ProcessHelper.tryFindFileOnPath
    |> function
       | Some yarn -> yarn
       | ex -> failwith ( sprintf "yarn not found (%A)\n" ex )

// Directories
let buildDir  = "./build/"

// Filesets
let projects  =
      !! "src/**.fsproj"

// Artifact packages
let packages  =
      !! "src/package.json"

let toPackage  =
      !! "src/*.fsproj"
      ++ "src/*.fs"
      ++ "src/package.json"
      ++ "README.md"
      ++ "RELEASE_NOTES.md"

let dotnetcliVersion = "1.0.1"
let mutable dotnetExePath = "dotnet"

let runDotnet workingDir args =
    printfn "CWD: %s" workingDir
    // printfn "dotnet %s" args
    let result =
        ExecProcess (fun info ->
            info.FileName <- dotnetExePath
            info.WorkingDirectory <- workingDir
            info.Arguments <- args) TimeSpan.MaxValue
    if result <> 0 then failwithf "Command failed: dotnet %s" args
    

Target "InstallDotNetCore" (fun _ ->
    let dotnetSDKPath = FullName "./dotnetsdk"
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

        dotnetExePath <- dotnetSDKPath </> (if isWindows then "dotnet.exe" else "dotnet")

    // let oldPath = System.Environment.GetEnvironmentVariable("PATH")
    // System.Environment.SetEnvironmentVariable("PATH", sprintf "%s%s%s" dotnetSDKPath (System.IO.Path.PathSeparator.ToString()) oldPath)
)


Target "Install" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
        let dir = IO.Path.GetDirectoryName s
        printf "Installing: %s\n" dir
        Npm (fun p ->
            { p with
                NpmFilePath = yarn
                Command = Install Standard
                WorkingDirectory = dir
            })
        runDotnet dir "restore"
    )
)


// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
        let dir = IO.Path.GetDirectoryName s
        runDotnet dir "build")
)

let release = LoadReleaseNotes "RELEASE_NOTES.md"

Target "Version" (fun _ ->
    Npm (fun p ->
    { p with
        NpmFilePath = yarn
        Command = Custom (sprintf "version --new-version %s" (string release.SemVer))
        WorkingDirectory = "src"
    })
)

Target "Publish" (fun _ ->
    toPackage |> CopyFiles buildDir
    let tag = getBuildParam "--tag" 
    printf "Publishing %s...\n" tag
    let tagArg = match tag with "" -> "" | _ -> sprintf "--tag %s" tag
    Npm (fun p ->
    { p with
        Command = Custom (sprintf "publish %s" tagArg)
        WorkingDirectory = buildDir
    })
)

// Build order
"Clean"
  ==> "InstallDotNetCore"
  ==> "Install"
  ==> "Build"
  ==> "Version"
  ==> "Publish"
  
// start build
RunTargetOrDefault "Build"

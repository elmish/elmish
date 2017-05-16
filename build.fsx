// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem"

open System
open System.IO
open Fake
open Fake.NpmHelper
open Fake.ReleaseNotesHelper
open Fake.Git


// Filesets
let projects  =
      !! "src/**.fsproj"


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
)


Target "Install" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
        let dir = IO.Path.GetDirectoryName s
        runDotnet dir "restore"
    )
)

Target "Build" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
        let dir = IO.Path.GetDirectoryName s
        runDotnet dir "build")
)

let release = LoadReleaseNotes "RELEASE_NOTES.md"

Target "Meta" (fun _ ->
    [ "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
      "<PropertyGroup>"
      "<Description>Elmish core for Fable apps</Description>"
      "<PackageProjectUrl>https://github.com/fable-elmish/elmish</PackageProjectUrl>"
      "<PackageLicenseUrl>https://raw.githubusercontent.com/fable-elmish/elmish/master/LICENSE.md</PackageLicenseUrl>"
      "<PackageIconUrl>https://raw.githubusercontent.com/fable-elmish/elmish/master/docs/files/img/logo.png</PackageIconUrl>"
      "<RepositoryUrl>https://github.com/fable-elmish/elmish.git</RepositoryUrl>"
      "<PackageTags>fable;elm;fsharp</PackageTags>"
      "<Authors>Eugene Tolmachev</Authors>" 
      sprintf "<Version>%s</Version>" (string release.SemVer)
      "</PropertyGroup>"
      "</Project>"]
    |> WriteToFile false "Meta.props"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

Target "Package" (fun _ ->
    runDotnet "src" "pack"
)

Target "PublishNuget" (fun _ ->
    runDotnet "src" "push"
)


// --------------------------------------------------------------------------------------
// Generate the documentation
let gitName = "elmish"
let gitOwner = "fable-elmish"
let gitHome = sprintf "https://github.com/%s" gitOwner

let fakePath = "packages" </> "build" </> "FAKE" </> "tools" </> "FAKE.exe"
let fakeStartInfo script workingDirectory args fsiargs environmentVars =
    (fun (info: System.Diagnostics.ProcessStartInfo) ->
        info.FileName <- System.IO.Path.GetFullPath fakePath
        info.Arguments <- sprintf "%s --fsiargs -d:FAKE %s \"%s\"" args fsiargs script
        info.WorkingDirectory <- workingDirectory
        let setVar k v =
            info.EnvironmentVariables.[k] <- v
        for (k, v) in environmentVars do
            setVar k v
        setVar "MSBuild" msBuildExe
        setVar "GIT" Git.CommandHelper.gitPath
        setVar "FSI" fsiPath)

/// Run the given buildscript with FAKE.exe
let executeFAKEWithOutput workingDirectory script fsiargs envArgs =
    let exitCode =
        ExecProcessWithLambdas
            (fakeStartInfo script workingDirectory "" fsiargs envArgs)
            TimeSpan.MaxValue false ignore ignore
    System.Threading.Thread.Sleep 1000
    exitCode

let copyFiles() =
    let header = 
        splitStr "\n" """(*** hide ***)
#I "../../src/bin/Debug"
#r "Fable.Core.dll"
#r "Fable.PowerPack.dll"
#r "Fable.Elmish.dll"

(**
*)"""

    !!"src/*.fs"
    |> Seq.map (fun fn -> ReadFile fn |> Seq.append header, fn)
    |> Seq.iter (fun (lines,fn) -> 
        let fsx = Path.Combine("docs/content",Path.ChangeExtension(fn |> Path.GetFileName, "fsx")) 
        lines |> WriteFile fsx)

// Documentation
let buildDocumentationTarget fsiargs target =
    trace (sprintf "Building documentation (%s), this could take some time, please wait..." target)
    let exit = executeFAKEWithOutput "docs/tools" "generate.fsx" fsiargs ["target", target]
    if exit <> 0 then
        failwith "generating reference documentation failed"
    ()

let generateHelp fail debug =
    copyFiles()
    CleanDir "docs/tools/.fake"
    let args =
        if debug then "--define:HELP"
        else "--define:RELEASE --define:HELP"
    try
        buildDocumentationTarget args "Default"
        traceImportant "Help generated"
    with
    | e when not fail ->
        traceImportant "generating help documentation failed"

Target "GenerateDocs" (fun _ ->
    generateHelp true false
)

Target "WatchDocs" (fun _ ->
    use watcher = !! "docs/content/**/*.*" |> WatchChanges (fun changes ->
         generateHelp true true
    )

    traceImportant "Waiting for help edits. Press any key to stop."

    System.Console.ReadKey() |> ignore

    watcher.Dispose()
)

// --------------------------------------------------------------------------------------
// Release Scripts

Target "ReleaseDocs" (fun _ ->
    let tempDocsDir = "temp/gh-pages"
    CleanDir tempDocsDir
    Repository.cloneSingleBranch "" (gitHome + "/" + gitName + ".git") "gh-pages" tempDocsDir

    CopyRecursive "docs/output" tempDocsDir true |> tracefn "%A"
    StageAll tempDocsDir
    Git.Commit.Commit tempDocsDir (sprintf "Update generated documentation for version %s" release.NugetVersion)
    Branches.push tempDocsDir
)

#load "paket-files/build/fsharp/FAKE/modules/Octokit/Octokit.fsx"
open Octokit

Target "Release" (fun _ ->
    let user =
        match getBuildParam "github-user" with
        | s when not (String.IsNullOrWhiteSpace s) -> s
        | _ -> getUserInput "Username: "
    let pw =
        match getBuildParam "github-pw" with
        | s when not (String.IsNullOrWhiteSpace s) -> s
        | _ -> getUserPassword "Password: "
    let remote =
        Git.CommandHelper.getGitResult "" "remote -v"
        |> Seq.filter (fun (s: string) -> s.EndsWith("(push)"))
        |> Seq.tryFind (fun (s: string) -> s.Contains(gitOwner + "/" + gitName))
        |> function None -> gitHome + "/" + gitName | Some (s: string) -> s.Split().[0]

    StageAll ""
    Git.Commit.Commit "" (sprintf "Bump version to %s" release.NugetVersion)
    Branches.pushBranch "" remote (Information.getBranchName "")

    Branches.tag "" release.NugetVersion
    Branches.pushTag "" remote release.NugetVersion

    // release on github
    createClient user pw
    |> createDraft gitOwner gitName release.NugetVersion (release.SemVer.PreRelease <> None) release.Notes
    |> releaseDraft
    |> Async.RunSynchronously
)

Target "Publish" DoNothing

// Build order
"Meta"
  ==> "InstallDotNetCore"
  ==> "Install"
  ==> "Build"
  ==> "Package"

"Build"
  ==> "GenerateDocs"
  ==> "ReleaseDocs"

"Publish"
  <== [ "Build"
        "PublishNuget"
        "ReleaseDocs" ]
  
  
// start build
RunTargetOrDefault "Build"

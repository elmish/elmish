#r "paket:
storage: packages
nuget FSharp.Core 4.7
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.Core.Target
nuget Fake.Core.ReleaseNotes
nuget Fake.Tools.Git
nuget Fake.DotNet.FSFormatting //"
#if !FAKE
#load ".fake/build.fsx/intellisense.fsx"
#r "Facades/netstandard"
#endif

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.Tools
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open System
open System.IO


// Filesets
let projects  =
    !! "src/**.fsproj"
    ++ "netstandard/**.fsproj"

Target.create "Clean" (fun _ ->
    Shell.cleanDir "src/obj"
    Shell.cleanDir "src/bin"
    Shell.cleanDir "netstandard/obj"
    Shell.cleanDir "netstandard/bin"
)

Target.create "Restore" (fun _ ->
    projects
    |> Seq.iter (Path.GetDirectoryName >> DotNet.restore id)
)

Target.create "Build" (fun _ ->
    projects
    |> Seq.iter (Path.GetDirectoryName >> DotNet.build id)
)

Target.create "Test" (fun _ ->
    DotNet.test (fun a -> a.WithCommon id) "tests"
)

let release = ReleaseNotes.load "RELEASE_NOTES.md"

Target.create "Meta" (fun _ ->
    [ "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
      "<ItemGroup>"
      "<None Include=\"../docs/files/img/logo.png\" Pack=\"true\" PackagePath=\"\\\"/>"
      "<PackageReference Include=\"Microsoft.SourceLink.GitHub\" Version=\"1.0.0\" PrivateAssets=\"All\"/>"
      "</ItemGroup>"
      "<PropertyGroup>"
      "<EmbedUntrackedSources>true</EmbedUntrackedSources>"
      "<AllowedOutputExtensionsInPackageBuildOutputFolder>$(AllowedOutputExtensionsInPackageBuildOutputFolder);.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder>"
      "<PackageProjectUrl>https://github.com/elmish/elmish</PackageProjectUrl>"
      "<PackageLicenseExpression>Apache-2.0</PackageLicenseExpression>"
      "<PackageIconUrl>https://raw.githubusercontent.com/elmish/elmish/master/docs/files/img/logo.png</PackageIconUrl>"
      "<PackageIcon>logo.png</PackageIcon>"
      "<RepositoryUrl>https://github.com/elmish/elmish.git</RepositoryUrl>"
      sprintf "<PackageReleaseNotes>%s</PackageReleaseNotes>" (List.head release.Notes)
      "<PackageTags>fable;elm;fsharp</PackageTags>"
      "<Authors>Eugene Tolmachev</Authors>"
      sprintf "<Version>%s</Version>" (string release.SemVer)
      "</PropertyGroup>"
      "</Project>"]
    |> File.write false "Directory.Build.props"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

Target.create "Package" (fun _ ->
    projects
    |> Seq.iter (Path.GetDirectoryName >> DotNet.pack id)
)

Target.create "PublishNuget" (fun _ ->
    let exec dir = DotNet.exec (DotNet.Options.withWorkingDirectory dir)

    let args = sprintf "push Fable.Elmish.%s.nupkg -s nuget.org -k %s" (string release.SemVer) (Environment.environVar "nugetkey")
    let result = exec "src/bin/Release" "nuget" args
    if (not result.OK) then failwithf "%A" result.Errors

    let args = sprintf "push Elmish.%s.nupkg -s nuget.org -k %s" (string release.SemVer) (Environment.environVar "nugetkey")
    let result = exec "netstandard/bin/Release" "nuget" args
    if (not result.OK) then
        failwithf "%A" result.Errors
)


// --------------------------------------------------------------------------------------
// Generate the documentation
Target.create "GenerateDocs" (fun _ ->
    let res = Shell.Exec("npm", "run docs:build")

    if res <> 0 then
        failwithf "Failed to generate docs"
)

Target.create "WatchDocs" (fun _ ->
    let res = Shell.Exec("npm", "run docs:watch")

    if res <> 0 then
        failwithf "Failed to watch docs: %d" res
)

// --------------------------------------------------------------------------------------
// Release Scripts

Target.create "ReleaseDocs" (fun _ ->
    let res = Shell.Exec("npm", "run docs:publish")

    if res <> 0 then
        failwithf "Failed to publish docs: %d" res
)

Target.create "Publish" ignore

// Build order
"Clean"
  ==> "Meta"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "Package"
  ==> "GenerateDocs"
  ==> "PublishNuget"
  ==> "ReleaseDocs"
  ==> "Publish"


// start build
Target.runOrDefault "Build"

#r "tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open System
open System.IO

let buildDir = __SOURCE_DIRECTORY__ + @"\build\"
let packages = __SOURCE_DIRECTORY__ + @"\src\packages"

let version = "0.0.1"    

Target "RestorePackages" (fun _ -> 
     "./src/basic-windows-installer.sln"
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             OutputPath = packages })
 )

Target "Clean" (fun _ ->
    CleanDirs [buildDir;]
)

Target "BuildApp" (fun _ ->
    CreateCSharpAssemblyInfo "./src/ExampleApp/Properties/AssemblyInfo.cs"
        [Attribute.Title "ExampleApp"
         Attribute.Description "ExampleApp"
         Attribute.Guid "079e6ad9-6706-4e1c-92c5-629173fcb67c"
         Attribute.Product "ExampleApp"
         Attribute.Version version
         Attribute.FileVersion version]
  
    !! "src/ExampleApp/ExampleApp.csproj"
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildInstaller" (fun _ ->
    CreateCSharpAssemblyInfo "./src/Installer/Properties/AssemblyInfo.cs"
        [Attribute.Title "ExampleApp"
         Attribute.Description "ExampleApp"
         Attribute.Guid "dd5319ab-a791-4164-af39-e79915f198b2"
         Attribute.Product "ExampleApp"
         Attribute.Version version
         Attribute.FileVersion version]
  
    !! "src/Installer/Installer.csproj"
     |> MSBuildRelease buildDir "Build"
     |> Log "BuildInstaller-Output: "
)

Target "Logo" (fun _ ->
    trace @"basic-windows-installer by @minton"
)

"Logo"
  ==> "Clean"
  ==> "RestorePackages"
  ==> "BuildApp"
  ==> "BuildInstaller"

RunTargetOrDefault "Logo"
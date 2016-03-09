@echo off
IF [%1]==[] GOTO DEFAULTTASK
  "src\.nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\FAKE\tools\Fake.exe" build.fsx %1
GOTO END
:DEFAULTTASK
  "src\.nuget\NuGet.exe" "Install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
"tools\FAKE\tools\Fake.exe" build.fsx
:end
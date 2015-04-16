@echo off
rem usage => make release
echo building....
msbuild ..\src\contest.sln  /p:configuration=%1 /m:4 /t:rebuild

rem merge assemblies into a single file
echo merging....
merge

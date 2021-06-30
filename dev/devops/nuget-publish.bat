@echo off

echo Rebuilding project (Release mode)...

del ..\..\src\AbfSharp\bin\Release\*.nupkg
del ..\..\src\AbfSharp\bin\Release\*.snupkg

dotnet build --configuration Release ..\..\src\AbfSharp

echo press ENTER 3 times to publish this package on NuGet...
pause
pause
pause

:: this script requires nuget.exe to be in the system path
:: https://www.nuget.org/downloads
:: and have your API key stored on your systeminfo
:: nuget SetApiKey 123456789
nuget push ..\..\src\AbfSharp\bin\Release\*.nupkg -Source https://api.nuget.org/v3/index.json
pause
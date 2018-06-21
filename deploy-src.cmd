REM https://docs.microsoft.com/pt-br/dotnet/core/tools/custom-templates
rd /s /q .\src\SpiderSharp\bin\Release
dotnet build -c Release .\src\SpiderSharp\SpiderSharp.csproj
nuget push .\src\SpiderSharp\bin\Release\*.nupkg -Source https://api.nuget.org/v3/index.json
del /f /s /q .\src\SpiderSharp\bin\Release\*.nupkg
REM https://docs.microsoft.com/pt-br/dotnet/core/tools/custom-templates
rd /s /q .\src\SpiderSharp\bin\Release
REM dotnet build -c Release .\src\SpiderSharp\SpiderSharp.csproj
dotnet build -c Release .\SpiderSharp.sln
nuget pack SpiderSharp.nuspec
nuget push .\*.nupkg -Source https://api.nuget.org/v3/index.json
del /f /s /q .\*.nupkg
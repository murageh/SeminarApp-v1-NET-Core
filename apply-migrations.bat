@echo off

REM Run the dotnet ef database update command
dotnet ef database update --project ./SeminarIntegration.csproj --startup-project ./SeminarIntegration.csproj

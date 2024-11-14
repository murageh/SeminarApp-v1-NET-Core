@echo off

REM Check if a migration name is provided
if "%1"=="" (
  echo Usage: %0 ^<MigrationName^>
  exit /b 1
)

REM Run the dotnet ef migrations add command
dotnet ef migrations add %1 --project ./SeminarIntegration.csproj --startup-project ./SeminarIntegration.csproj
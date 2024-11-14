#!/bin/bash

# Check if a migration name is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <MigrationName>"
  exit 1
fi

# Run the dotnet ef migrations add command
dotnet ef migrations add "$1" --project ./SeminarIntegration.csproj --startup-project ./SeminarIntegration.csproj

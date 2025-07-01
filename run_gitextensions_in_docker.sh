#!/bin/bash

# Make it executable: chmod +x run_gitextensions_in_docker.sh
# Path to the built GitExtensions.dll (adjust if needed)
APP_DIR="/mnt/c/1/_progr/repos/gitext_wt3/artifacts/Debug/bin/GitExtensions/net9.0"
DLL="GitExtensions.dll"

# Run in a .NET 9.0 SDK container (change tag if needed)
docker run --rm -it \
  -v "$APP_DIR":/app \
  mcr.microsoft.com/dotnet/runtime:9.0 \
  dotnet /app/$DLL
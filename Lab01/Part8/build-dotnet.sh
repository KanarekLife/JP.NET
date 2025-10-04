#!/usr/bin/env bash

##########################################################################
# Cake build script for .NET Core/5+ using dotnet tool
##########################################################################

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed or not in PATH"
    exit 1
fi

# Install Cake tool if not already installed
if ! dotnet tool list -g | grep -q "cake.tool"; then
    echo "Installing Cake dotnet tool globally..."
    dotnet tool install --global Cake.Tool
fi

# Pass all arguments directly to dotnet cake
echo "Running: dotnet cake build.cake $@"
echo ""

# Start Cake
dotnet cake build.cake "$@"
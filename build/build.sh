#!/usr/bin/env bash

set -e
set -o pipefail
set -u

# Default values
RUNTIME=${RUNTIME:-linux-x64}
VERSION=${VERSION:-4.0.0}
CONFIGURATION=${CONFIGURATION:-Release}

echo "Building Git Extensions for Linux"
echo "Runtime: $RUNTIME"
echo "Version: $VERSION"
echo "Configuration: $CONFIGURATION"

# Ensure we're in the project root
cd "$(dirname "$0")/.."

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf build/GitExtensions
rm -rf build/*.deb
rm -rf build/*.rpm
rm -rf build/*.AppImage

# Build the application
echo "Building Git Extensions..."
dotnet publish src/app/GitExtensions/GitExtensions.csproj \
    --configuration $CONFIGURATION \
    --runtime $RUNTIME \
    --self-contained true \
    --output build/GitExtensions \
    -p:PublishSingleFile=false \
    -p:PublishTrimmed=false

# Make the main executable... executable
chmod +x build/GitExtensions/GitExtensions

# Create a symlink with lowercase name for consistency
ln -sf GitExtensions build/GitExtensions/gitextensions

# Export environment variables for the packaging script
export RUNTIME
export VERSION

# Run the packaging script
echo "Running packaging script..."
cd build
bash scripts/package.linux.sh

echo "Build completed successfully!"
echo "Generated packages:"
ls -la *.deb *.rpm *.AppImage 2>/dev/null || echo "Some package types may not have been built"

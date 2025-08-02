#!/usr/bin/env bash

# Setup script to copy icon files from the main Git Extensions assets
# to the build resources directories

set -e

echo "Setting up icon files for Linux packaging..."

# Check if we're in the right directory
if [[ ! -f "build.sh" ]]; then
    echo "Error: This script must be run from the build directory"
    echo "Usage: cd build && ./setup-icons.sh"
    exit 1
fi

# Source icon path (relative to project root)
ICON_SOURCE="../setup/assets/Logo"
ICON_48_SOURCE="$ICON_SOURCE/git-extensions-logo-48px.png"
ICON_MAIN_SOURCE="$ICON_SOURCE/git-extensions-logo-256px.png"

# Target directories
COMMON_ICONS_DIR="resources/_common/icons/hicolor/48x48/apps"
APPIMAGE_ICONS_DIR="resources/appimage"

# Create directories if they don't exist
mkdir -p "$COMMON_ICONS_DIR"
mkdir -p "$APPIMAGE_ICONS_DIR"

# Copy icons if source files exist
if [[ -f "$ICON_48_SOURCE" ]]; then
    echo "Copying 48x48 icon..."
    cp "$ICON_48_SOURCE" "$COMMON_ICONS_DIR/gitextensions.png"
    echo "✓ Copied to $COMMON_ICONS_DIR/gitextensions.png"
else
    echo "⚠ Warning: $ICON_48_SOURCE not found"
    echo "  You'll need to manually copy a 48x48 PNG icon to:"
    echo "  $COMMON_ICONS_DIR/gitextensions.png"
fi

if [[ -f "$ICON_MAIN_SOURCE" ]]; then
    echo "Copying AppImage icon..."
    cp "$ICON_MAIN_SOURCE" "$APPIMAGE_ICONS_DIR/gitextensions.png"
    echo "✓ Copied to $APPIMAGE_ICONS_DIR/gitextensions.png"
else
    echo "⚠ Warning: $ICON_MAIN_SOURCE not found"
    echo "  You'll need to manually copy a PNG icon to:"
    echo "  $APPIMAGE_ICONS_DIR/gitextensions.png"
fi

# Remove placeholder files
rm -f "$COMMON_ICONS_DIR/.gitkeep" 2>/dev/null || true

echo ""
echo "Icon setup completed!"
echo "You can now run ./build.sh to build the Linux packages."

#!/bin/bash
# Common configuration for GitExtensions Linux packaging
# This file is sourced by all build and packaging scripts

# Application metadata
export APP_NAME="GitExtensions"
export APP_NAME_KEY="gitextensions"
export APP_NAME_SCM="gitextensions_scm"
export APP_VERSION="${VERSION:-5.9.1}"
export APP_DESCRIPTION="Git Extensions is a standalone UI tool for managing git repositories"
export APP_HOMEPAGE="https://gitextensions.github.io/"
export APP_MAINTAINER="Git Extensions Team <gitextensions@gmail.com>"
export APP_LICENSE="GPL-3.0+"

# Build configuration
export BUILD_RUNTIME="${RUNTIME:-linux-x64}"
export BUILD_CONFIGURATION="${CONFIGURATION:-Release}"
export BUILD_SOURCE_DIR="GitExtensions"

# Packaging configuration
export PACKAGE_SECTION="vcs"
export PACKAGE_PRIORITY="optional"
export PACKAGE_INSTALLED_SIZE="50000"

# Dependencies (shared between DEB and RPM)
export DEB_DEPENDENCIES="libgtk2.0-0, libglib2.0-0, libgdiplus, libx11-6, libxinerama1 | libXinerama1, libicu | libicu76 | libicu74 | libicu72 | libicu71 | libicu70 | libicu69 | libicu68 | libicu67 | libicu66 | libicu65 | libicu63 | libicu60 | libicu57 | libicu55 | libicu52"
export RPM_DEPENDENCIES="git, gtk2-devel, glib2, gdk-pixbuf2, libgdiplus, freetype, fontconfig, libX11, libXext, libXrender, libXrandr2, libXi, libXinerama, libXfixes, libXdamage, libXcomposite, libXcursor, libicu, glibc, libgcc, libstdc++"

# Paths
export ICON_SOURCE_DIR="../setup/assets/Logo"
export ICON_48PX="$ICON_SOURCE_DIR/git-extensions-logo-48px.png"
export ICON_256PX="$ICON_SOURCE_DIR/git-extensions-logo-256px.png"

# Architecture mapping function
get_architecture() {
    case "$BUILD_RUNTIME" in
        linux-x64)
            export ARCH="amd64"
            export APPIMAGE_ARCH="x86_64"
            export RPM_TARGET="x86_64"
            ;;
        linux-arm64)
            export ARCH="arm64"
            export APPIMAGE_ARCH="arm_aarch64"
            export RPM_TARGET="aarch64"
            ;;
        *)
            echo "Unknown runtime $BUILD_RUNTIME"
            exit 1
            ;;
    esac
}

# Common initialization function
initialize_common() {
    echo "Initializing GitExtensions build environment..."
    echo "Application: $APP_NAME v$APP_VERSION"
    echo "Runtime: $BUILD_RUNTIME"
    echo "Configuration: $BUILD_CONFIGURATION"
    
    # Set architecture variables
    get_architecture
    
    echo "Target Architecture: $ARCH"
    echo "AppImage Architecture: $APPIMAGE_ARCH"
    echo "RPM Target: $RPM_TARGET"
}

# Common cleanup function
cleanup_build() {
    echo "Cleaning previous builds..."
    rm -rf "$BUILD_SOURCE_DIR"
    rm -rf *.deb
    rm -rf *.rpm
    rm -rf *.AppImage
}

# Icon setup function
setup_icons() {
    echo "Setting up icons..."
    mkdir -p resources/_common/icons/hicolor/48x48/apps
    mkdir -p resources/appimage
    
    # Copy 48px icon for desktop integration
    if [[ -f "$ICON_48PX" ]]; then
        cp "$ICON_48PX" "resources/_common/icons/hicolor/48x48/apps/gitextensions.png"
        echo "✓ Copied 48px icon for desktop integration"
    else
        echo "⚠ Warning: 48px icon not found at $ICON_48PX"
    fi
    
    # Copy larger icon for AppImage
    if [[ -f "$ICON_256PX" ]]; then
        cp "$ICON_256PX" "resources/appimage/gitextensions.png"
        echo "✓ Copied 256px icon for AppImage"
    else
        echo "⚠ Warning: 256px icon not found at $ICON_256PX"
    fi
}

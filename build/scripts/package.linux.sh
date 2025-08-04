#!/usr/bin/env bash

set -e
set -o
set -u
set pipefail
echo current \$0 = "$0"
# Only run main function if script is executed directly (not sourced)
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    cd "$(dirname "$0")/.."
else
    # Ensure we're in the project root
    cd "$(dirname "$0")/.."
    cd build
fi
# Source common configuration
source "$PWD/common.sh"
initialize_common

# Source template generation functions
source "$PWD/generate-templates.sh"

generate_desktop_file

# Variables are now set by common.conf:
# - ARCH (arch)
# - APPIMAGE_ARCH (appimage_arch) 
# - RPM_TARGET (target)
# - APP_NAME, APP_NAME_KEY, APP_NAME_SCM
# - BUILD_RUNTIME, APP_VERSION, BUILD_SOURCE_DIR

APPIMAGETOOL_URL=https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage

# Copy icons from source if they don't exist
if [[ ! -f "resources/_common/icons/hicolor/48x48/apps/gitextensions.png" ]]; then
    echo "Copying icons from source..."
    mkdir -p resources/_common/icons/hicolor/48x48/apps
    mkdir -p resources/appimage
    
    # Copy 48px icon for desktop integration
    if [[ -f "../setup/assets/Logo/git-extensions-logo-48px.png" ]]; then
        cp "../setup/assets/Logo/git-extensions-logo-48px.png" "resources/_common/icons/hicolor/48x48/apps/gitextensions.png"
        echo "✓ Copied 48px icon for desktop integration"
    else
        echo "⚠ Warning: 48px icon not found at ../setup/assets/Logo/git-extensions-logo-48px.png"
    fi
    
    # Copy larger icon for AppImage
    if [[ -f "../setup/assets/Logo/git-extensions-logo-256px.png" ]]; then
        cp "../setup/assets/Logo/git-extensions-logo-256px.png" "resources/appimage/gitextensions.png"
        echo "✓ Copied 256px icon for AppImage"
    else
        echo "⚠ Warning: 256px icon not found at ../setup/assets/Logo/git-extensions-logo-256px.png"
    fi
fi

generate_rpm() {
    echo Build RPM package
echo need rpmbuild , install rpm first
generate_rpm_spec
rpmbuild -bb --target="$RPM_TARGET" resources/rpm/SPECS/build.spec --define "_topdir $(pwd)/resources/rpm" --define "_version $APP_VERSION"
mv "resources/rpm/RPMS/$RPM_TARGET/$APP_NAME_KEY-$APP_VERSION-1.$RPM_TARGET.rpm" ./
}

if [[ ! -f "appimagetool" ]]; then
    curl -o appimagetool -L "$APPIMAGETOOL_URL"
    chmod +x appimagetool
fi

# Use shared configuration variables
APPNAME=$APP_NAME
APPNAMEkey_scm=$APP_NAME_SCM
APPNAMEkey=$APP_NAME_KEY
APPNAMEopt="${APPNAMEkey}"
BUILDSRC=$BUILD_SOURCE_DIR
rm -f $BUILDSRC/*.dbg

echo Create AppImage structure
mkdir -p $APPNAME.AppDir/opt
mkdir -p $APPNAME.AppDir/usr/share/metainfo
mkdir -p $APPNAME.AppDir/usr/share/applications

rm -v -rf $APPNAME.AppDir/opt/$APPNAMEopt

cp -v -r $BUILDSRC $APPNAME.AppDir/opt/$APPNAMEopt
desktop-file-install resources/_common/applications/$APPNAMEkey.desktop \
     --dir $APPNAME.AppDir/usr/share/applications \
    --set-icon com.$APPNAMEkey_scm.$APPNAME --set-key=Exec --set-value=AppRun
mv -v $APPNAME.AppDir/usr/share/applications/{$APPNAMEkey,com.$APPNAMEkey_scm.$APPNAME}.desktop

# Copy icon
cp resources/appimage/gitextensions.png $APPNAME.AppDir/com.$APPNAMEkey_scm.$APPNAME.png
ln -v -rsf $APPNAME.AppDir/opt/$APPNAMEopt/$APPNAME $APPNAME.AppDir/AppRun
ln -rsf $APPNAME.AppDir/usr/share/applications/com.$APPNAMEkey_scm.$APPNAME.desktop $APPNAME.AppDir

# Copy appdata
cp resources/appimage/gitextensions.appdata.xml $APPNAME.AppDir/usr/share/metainfo/com.$APPNAMEkey_scm.$APPNAME.appdata.xml

echo Build AppImage
ARCH="$APPIMAGE_ARCH" ./appimagetool -v $APPNAME.AppDir "$APPNAMEkey-$APP_VERSION.linux.$ARCH.AppImage"

# Function to determine appropriate DEB build location
determine_deb_location() {
    local base_deb_dir="resources/deb"
    
    # Create test directory structure
    mkdir -p "$base_deb_dir/DEBIAN"
    
    # Try to set permissions on the DEBIAN directory
    chmod u=rwx,go=rx "$base_deb_dir/DEBIAN" 2>/dev/null
    
    # Verify that permissions were actually set correctly
    local actual_perms=$(stat -c "%a" "$base_deb_dir/DEBIAN" 2>/dev/null)
    if [[ "$actual_perms" != "755" ]]; then
        echo "Warning: Cannot set correct permissions on $base_deb_dir/DEBIAN (got $actual_perms, need 755)" >&2
        echo "This typically happens on NTFS filesystems. Using temporary location for DEB build..." >&2
        
        # Create a temporary directory in /tmp (which supports Unix permissions)
        local temp_deb_dir=$(mktemp -d -t gitextensions-deb-XXXXXX)
        echo "DEB build location: $temp_deb_dir" >&2
        
        # Clean up the failed attempt
        rm -rf "$base_deb_dir"
        
        echo "$temp_deb_dir"
        return 0
    else
        echo "DEB build location: $base_deb_dir (permissions OK: $actual_perms)" >&2
        echo "$base_deb_dir"
        return 0
    fi
}

echo Prepare DEB package structure
# Determine the appropriate location for DEB build
DEB_BUILD_DIR=$(determine_deb_location)

# Create DEB package structure in the determined location
mkdir -p "$DEB_BUILD_DIR/opt/$APPNAMEkey/"
mkdir -p "$DEB_BUILD_DIR/usr/bin"
mkdir -p "$DEB_BUILD_DIR/usr/share/applications"
mkdir -p "$DEB_BUILD_DIR/usr/share/icons"
mkdir -p "$DEB_BUILD_DIR/usr/share/pixmaps"
mkdir -p "$DEB_BUILD_DIR/DEBIAN"

# Copy application files
cp -fr $BUILDSRC/* "$DEB_BUILD_DIR/opt/$APPNAMEkey/"
ln -rsf "$DEB_BUILD_DIR/opt/$APPNAMEkey/$APPNAMEkey" "$DEB_BUILD_DIR/usr/bin/"

# Copy desktop files and icons
cp -r resources/_common/applications "$DEB_BUILD_DIR/usr/share/"
cp -r resources/_common/icons "$DEB_BUILD_DIR/usr/share/"
cp resources/_common/icons/hicolor/48x48/apps/gitextensions.png "$DEB_BUILD_DIR/usr/share/pixmaps/"

# Calculate installed size in KB
installed_size=$(du -sk "$DEB_BUILD_DIR" | cut -f1)

# Generate DEB control file in the determined location
generate_deb_control "$DEB_BUILD_DIR"

# Build deb package with gzip compression
dpkg-deb -Zgzip --root-owner-group --build "$DEB_BUILD_DIR" "${APPNAMEkey}_$APP_VERSION-1_$ARCH.deb"

# Clean up temporary directory if we used one
if [[ "$DEB_BUILD_DIR" != "resources/deb" ]]; then
    echo "Cleaning up temporary directory: $DEB_BUILD_DIR"
    rm -rf "$DEB_BUILD_DIR"
fi

generate_rpm
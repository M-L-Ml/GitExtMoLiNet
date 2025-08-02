#!/usr/bin/env bash

set -e
set -o
set -u
set pipefail

arch=
appimage_arch=
target=
case "$RUNTIME" in
    linux-x64)
        arch=amd64
        appimage_arch=x86_64
        target=x86_64;;
    linux-arm64)
        arch=arm64
        appimage_arch=arm_aarch64
        target=aarch64;;
    *)
        echo "Unknown runtime $RUNTIME"
        exit 1;;
esac

APPIMAGETOOL_URL=https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage

cd build

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

if [[ ! -f "appimagetool" ]]; then
    curl -o appimagetool -L "$APPIMAGETOOL_URL"
    chmod +x appimagetool
fi

APPNAME=GitExtensions
APPNAMEkey=gitextensions
APPNAMEkey_scm=gitextensions_scm
BUILDSRC=GitExtensions
rm -f $BUILDSRC/*.dbg

# Create AppImage structure
mkdir -p $APPNAME.AppDir/opt
mkdir -p $APPNAME.AppDir/usr/share/metainfo
mkdir -p $APPNAME.AppDir/usr/share/applications

cp -r $BUILDSRC $APPNAME.AppDir/opt/$APPNAMEkey
desktop-file-install resources/_common/applications/$APPNAMEkey.desktop --dir $APPNAME.AppDir/usr/share/applications \
    --set-icon com.$APPNAMEkey_scm.$APPNAME --set-key=Exec --set-value=AppRun
mv $APPNAME.AppDir/usr/share/applications/{$APPNAMEkey,com.$APPNAMEkey_scm.$APPNAME}.desktop

# Copy icon
cp resources/appimage/gitextensions.png $APPNAME.AppDir/com.$APPNAMEkey_scm.$APPNAME.png
ln -rsf $APPNAME.AppDir/opt/$APPNAMEkey/$APPNAMEkey $APPNAME.AppDir/AppRun
ln -rsf $APPNAME.AppDir/usr/share/applications/com.$APPNAMEkey_scm.$APPNAME.desktop $APPNAME.AppDir

# Copy appdata
cp resources/appimage/gitextensions.appdata.xml $APPNAME.AppDir/usr/share/metainfo/com.$APPNAMEkey_scm.$APPNAME.appdata.xml

# Build AppImage
ARCH="$appimage_arch" ./appimagetool -v $APPNAME.AppDir "$APPNAMEkey-$VERSION.linux.$arch.AppImage"

# Prepare DEB package structure
mkdir -p resources/deb/opt/$APPNAMEkey/
mkdir -p resources/deb/usr/bin
mkdir -p resources/deb/usr/share/applications
mkdir -p resources/deb/usr/share/icons
mkdir -p resources/deb/usr/share/pixmaps

# Copy application files
cp -f $BUILDSRC/* resources/deb/opt/$APPNAMEkey/
ln -rsf resources/deb/opt/$APPNAMEkey/$APPNAMEkey resources/deb/usr/bin/

# Copy desktop files and icons
cp -r resources/_common/applications resources/deb/usr/share/
cp -r resources/_common/icons resources/deb/usr/share/
cp resources/_common/icons/hicolor/48x48/apps/gitextensions.png resources/deb/usr/share/pixmaps/

# Calculate installed size in KB
installed_size=$(du -sk resources/deb | cut -f1)

# Update the control file
sed -i -e "s/^Version:.*/Version: $VERSION/" \
    -e "s/^Architecture:.*/Architecture: $arch/" \
    -e "s/^Installed-Size:.*/Installed-Size: $installed_size/" \
    resources/deb/DEBIAN/control

# Build deb package with gzip compression
dpkg-deb -Zgzip --root-owner-group --build resources/deb "$APPNAMEkey_$VERSION-1_$arch.deb"

# Build RPM package
rpmbuild -bb --target="$target" resources/rpm/SPECS/build.spec --define "_topdir $(pwd)/resources/rpm" --define "_version $VERSION"
mv "resources/rpm/RPMS/$target/$APPNAMEkey-$VERSION-1.$target.rpm" ./

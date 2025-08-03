#!/bin/bash
# Template generation script for GitExtensions packaging
# This script generates packaging files from templates using shared configuration

set -e
set -o pipefail
set -u

# Source common configuration
source "$(dirname "$0")/common.sh"
initialize_common

# Function to substitute variables in template files
substitute_template() {
    local template_file="$1"
    local output_file="$2"
    
    echo "Generating $output_file from $template_file..."
    
    # Use envsubst to substitute environment variables
    envsubst < "$template_file" > "$output_file"
    
    echo "✓ Generated $output_file"
}

# Generate DEB control file
generate_deb_control() {
    cat > "resources/deb/DEBIAN/control" << EOF
Package: $APP_NAME_KEY
Version: $APP_VERSION
Section: $PACKAGE_SECTION
Priority: $PACKAGE_PRIORITY
Architecture: $ARCH
Installed-Size: $PACKAGE_INSTALLED_SIZE
Depends: $DEB_DEPENDENCIES
Maintainer: $APP_MAINTAINER
Homepage: $APP_HOMEPAGE
Description: $APP_DESCRIPTION
 Git Extensions is a graphical user interface for Git that allows you to
 control Git without using the command line. It comes with a comprehensive
 manual to get you started with Git and Git Extensions quickly.
 .
EOF
    
    echo "✓ Generated DEB control file"
}

# Generate RPM spec file
generate_rpm_spec() {
    cat > "resources/rpm/SPECS/build.spec" << EOF
Name: $APP_NAME_KEY
Version: $APP_VERSION
Release: 1
Summary: $APP_DESCRIPTION
License: $APP_LICENSE
URL: $APP_HOMEPAGE
BuildArch: $RPM_TARGET
Requires: $RPM_DEPENDENCIES

%description
Git Extensions is a graphical user interface for Git that allows you to
control Git without using the command line. It comes with a comprehensive
manual to get you started with Git and Git Extensions quickly.

Features include:
- Feature rich user interface for Git
- Comprehensive Git repository management
- Built-in merge conflict resolution
- Git history visualization

%prep
# No preparation needed

%build
# No build needed - pre-built binaries

%install
mkdir -p %{buildroot}/opt/$APP_NAME_KEY
mkdir -p %{buildroot}/usr/bin
mkdir -p %{buildroot}/usr/share/applications
mkdir -p %{buildroot}/usr/share/icons
mkdir -p %{buildroot}/usr/share/pixmaps

# Copy application files
cp -r ../../../$BUILD_SOURCE_DIR/* %{buildroot}/opt/$APP_NAME_KEY/

# Create symlink
ln -sf /opt/$APP_NAME_KEY/$APP_NAME_KEY %{buildroot}/usr/bin/$APP_NAME_KEY

# Copy desktop file and icons
cp ../../../resources/_common/applications/$APP_NAME_KEY.desktop %{buildroot}/usr/share/applications/
cp -r ../../../resources/_common/icons/* %{buildroot}/usr/share/icons/
cp ../../../resources/_common/icons/hicolor/48x48/apps/$APP_NAME_KEY.png %{buildroot}/usr/share/pixmaps/

%files
/opt/$APP_NAME_KEY/*
/usr/bin/$APP_NAME_KEY
/usr/share/applications/$APP_NAME_KEY.desktop
/usr/share/icons/hicolor/*/apps/$APP_NAME_KEY.png
/usr/share/pixmaps/$APP_NAME_KEY.png

%changelog
* $(date '+%a %b %d %Y') $APP_MAINTAINER - $APP_VERSION-1
- Initial RPM package for Git Extensions
EOF
    
    echo "✓ Generated RPM spec file"
}

# Generate desktop file
generate_desktop_file() {
    cat > "resources/_common/applications/$APP_NAME_KEY.desktop" << EOF
[Desktop Entry]
Version=1.0
Type=Application
Name=$APP_NAME
Comment=$APP_DESCRIPTION
Exec=$APP_NAME_KEY
Icon=$APP_NAME_KEY
Terminal=false
Categories=Development;RevisionControl;
StartupNotify=true
MimeType=x-scheme-handler/$APP_NAME_KEY;
Keywords=git;vcs;version control;repository;
EOF
    
    echo "✓ Generated desktop file"
}

# Generate AppImage metadata
generate_appimage_metadata() {
    cat > "resources/appimage/$APP_NAME_KEY.appdata.xml" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<component type="desktop-application">
  <id>com.$APP_NAME_SCM.$APP_NAME</id>
  <metadata_license>CC0-1.0</metadata_license>
  <project_license>$APP_LICENSE</project_license>
  <name>$APP_NAME</name>
  <summary>$APP_DESCRIPTION</summary>
  <description>
    <p>
      Git Extensions is a graphical user interface for Git that allows you to
      control Git without using the command line. It comes with a comprehensive
      manual to get you started with Git and Git Extensions quickly.
    </p>
    <p>Features include:</p>
    <ul>
      <li>Feature rich user interface for Git</li>
      <li>Comprehensive Git repository management</li>
      <li>Built-in merge conflict resolution</li>
      <li>Git history visualization</li>
    </ul>
  </description>
  <launchable type="desktop-id">com.$APP_NAME_SCM.$APP_NAME.desktop</launchable>
  <url type="homepage">$APP_HOMEPAGE</url>
  <url type="bugtracker">https://github.com/gitextensions/gitextensions/issues</url>
  <url type="help">https://git-extensions-documentation.readthedocs.io/</url>
  <screenshots>
    <screenshot type="default">
      <image>https://github.com/M-L-Ml/gitextensions/blob/postmonoforms/Setup/assets/gitm-screenshot.png</image>
      <caption>Main $APP_NAME interface</caption>
    </screenshot>
  </screenshots>
  <categories>
    <category>Development</category>
    <category>RevisionControl</category>
  </categories>
  <keywords>
    <keyword>git</keyword>
    <keyword>vcs</keyword>
    <keyword>version control</keyword>
    <keyword>repository</keyword>
  </keywords>
</component>
EOF
    
    echo "✓ Generated AppImage metadata"
}

# Main execution
generate_templates_main() {
    echo "Generating packaging templates..."
    
    # Create necessary directories
    mkdir -p resources/deb/DEBIAN
    mkdir -p resources/rpm/SPECS
    mkdir -p resources/_common/applications
    mkdir -p resources/appimage
    
    # Generate all packaging files
    generate_deb_control
    generate_rpm_spec
    generate_desktop_file
    generate_appimage_metadata
    
    echo ""
    echo "All packaging templates generated successfully!"
    echo "Configuration used:"
    echo "  App Name: $APP_NAME"
    echo "  Version: $APP_VERSION"
    echo "  Architecture: $ARCH"
    echo "  Runtime: $BUILD_RUNTIME"
}

# Only run main function if script is executed directly (not sourced)
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    generate_templates_main "$@"
fi

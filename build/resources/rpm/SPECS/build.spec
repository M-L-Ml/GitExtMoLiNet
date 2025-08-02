Name: gitextensions
Version: %{_version}
Release: 1
Summary: Git Extensions is a standalone UI tool for managing git repositories
License: GPL-3.0+
URL: https://gitextensions.github.io/
BuildArch: x86_64
Requires: git, gtk2-devel, glib2, cairo, pango, atk, gdk-pixbuf2, libgdiplus, freetype, fontconfig, libX11, libXext, libXrender, libXrandr2, libXi, libXinerama, libXfixes, libXdamage, libXcomposite, libXcursor, libicu, libjpeg-turbo, libpng, libtiff, libwebp, giflib, libexif, glibc, libgcc, libstdc++

%description
Git Extensions is a graphical user interface for Git that allows you to
control Git without using the command line. It comes with a comprehensive
manual to get you started with Git and Git Extensions quickly.

Features include:
- Windows Explorer integration for Git
- Visual Studio plugin support  
- Feature rich user interface for Git
- Comprehensive Git repository management
- Built-in merge conflict resolution
- Git history visualization

%prep
# No preparation needed

%build
# No build needed - pre-built binaries

%install
mkdir -p %{buildroot}/opt/gitextensions
mkdir -p %{buildroot}/usr/bin
mkdir -p %{buildroot}/usr/share/applications
mkdir -p %{buildroot}/usr/share/icons
mkdir -p %{buildroot}/usr/share/pixmaps

# Copy application files
cp -r ../../../GitExtensions/* %{buildroot}/opt/gitextensions/

# Create symlink
ln -sf /opt/gitextensions/gitextensions %{buildroot}/usr/bin/gitextensions

# Copy desktop file and icons
cp ../../../resources/_common/applications/gitextensions.desktop %{buildroot}/usr/share/applications/
cp -r ../../../resources/_common/icons/* %{buildroot}/usr/share/icons/
cp ../../../resources/_common/icons/hicolor/48x48/apps/gitextensions.png %{buildroot}/usr/share/pixmaps/

%files
/opt/gitextensions/*
/usr/bin/gitextensions
/usr/share/applications/gitextensions.desktop
/usr/share/icons/hicolor/*/apps/gitextensions.png
/usr/share/pixmaps/gitextensions.png

%changelog
* Thu Aug 02 2024 Git Extensions Team <gitextensions@gmail.com> - 4.0.0-1
- Initial RPM package for Git Extensions

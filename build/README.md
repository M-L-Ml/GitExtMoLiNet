# Git Extensions Linux Packaging

This directory contains scripts and resources for building Linux packages (DEB, RPM, and AppImage) for Git Extensions.

## Prerequisites

### System Requirements
- Linux distribution with bash shell
- .NET SDK 6.0 or later
- Git
- Standard build tools

### Package Building Dependencies
- `dpkg-deb` (for DEB packages)
- `rpmbuild` (for RPM packages) 
- `desktop-file-utils` (for desktop file validation)
- `curl` (for downloading AppImage tools)

Install on Ubuntu/Debian:
```bash
sudo apt-get update
sudo apt-get install build-essential dpkg-dev desktop-file-utils curl rpm
```

Install on RHEL/CentOS/Fedora:
```bash
sudo dnf install rpm-build desktop-file-utils curl dpkg
# or on older systems:
sudo yum install rpm-build desktop-file-utils curl dpkg
```

## Building Packages

### Quick Start
```bash
# Build all package types with default settings
./build.sh
```

### Custom Build
```bash
# Set environment variables for custom build
export RUNTIME=linux-x64        # or linux-arm64
export VERSION=4.0.0           # version number
export CONFIGURATION=Release   # or Debug

./build.sh
```

### Individual Package Types
```bash
# Build only specific package types by running the packaging script directly
cd build
export RUNTIME=linux-x64
export VERSION=4.0.0
bash scripts/package.linux.sh
```

## Package Structure

### DEB Package
- **Location**: `build/*.deb`
- **Installation path**: `/opt/gitextensions/`
- **Binary symlink**: `/usr/bin/gitextensions`
- **Desktop file**: `/usr/share/applications/gitextensions.desktop`
- **Icons**: `/usr/share/icons/hicolor/*/apps/gitextensions.png`

### RPM Package
- **Location**: `build/*.rpm`
- **Installation path**: `/opt/gitextensions/`
- **Binary symlink**: `/usr/bin/gitextensions`
- **Desktop file**: `/usr/share/applications/gitextensions.desktop`
- **Icons**: `/usr/share/icons/hicolor/*/apps/gitextensions.png`

### AppImage
- **Location**: `build/*.AppImage`
- **Portable**: Can be run from any location
- **Self-contained**: Includes all dependencies

## Directory Structure

```
build/
├── build.sh                           # Main build script
├── scripts/
│   └── package.linux.sh              # Packaging script
├── resources/
│   ├── _common/
│   │   ├── applications/
│   │   │   └── gitextensions.desktop  # Desktop entry file
│   │   └── icons/
│   │       └── hicolor/
│   │           └── 48x48/apps/        # Icon files (48x48 PNG)
│   ├── deb/
│   │   └── DEBIAN/
│   │       ├── control                # DEB package metadata
│   │       ├── preinst               # Pre-installation script
│   │       └── prerm                 # Pre-removal script
│   ├── rpm/
│   │   └── SPECS/
│   │       └── build.spec            # RPM build specification
│   └── appimage/
│       ├── gitextensions.png         # AppImage icon
│       └── gitextensions.appdata.xml # AppImage metadata
└── README.md                         # This file
```

## Setup Requirements

### Icon Files
Before building packages, you need to copy the actual Git Extensions icon files:

1. Copy the main icon from `setup/assets/Logo/git-extensions-logo-48.png` to:
   - `build/resources/_common/icons/hicolor/48x48/apps/gitextensions.png`
   - `build/resources/appimage/gitextensions.png`

2. If you have icons in other sizes, create the appropriate directories:
   ```bash
   mkdir -p build/resources/_common/icons/hicolor/{16x16,32x32,64x64,128x128}/apps/
   ```

### Version Information
The build script automatically detects the version from the project, but you can override it:
```bash
export VERSION=4.0.0
./build.sh
```

## Installation

### DEB Package
```bash
sudo dpkg -i gitextensions_4.0.0-1_amd64.deb
sudo apt-get install -f  # Fix any dependency issues
```

### RPM Package
```bash
sudo rpm -i gitextensions-4.0.0-1.x86_64.rpm
# or
sudo dnf install gitextensions-4.0.0-1.x86_64.rpm
```

### AppImage
```bash
chmod +x gitextensions-4.0.0.linux.amd64.AppImage
./gitextensions-4.0.0.linux.amd64.AppImage
```

## Troubleshooting

### Common Issues

1. **Missing dependencies**: Ensure all build dependencies are installed
2. **Permission errors**: Make sure scripts are executable (`chmod +x build.sh`)
3. **Icon missing**: Copy the actual icon files as described in Setup Requirements
4. **Version mismatch**: Check that VERSION environment variable is set correctly

### Build Logs
Build output and errors are displayed in the terminal. For debugging:
```bash
# Enable verbose output
set -x
./build.sh
```

### Package Testing
Test the generated packages in a clean environment:
```bash
# For DEB packages
docker run --rm -v $(pwd):/workspace ubuntu:20.04 bash -c "
  apt-get update && 
  apt-get install -y /workspace/*.deb &&
  gitextensions --version
"

# For RPM packages  
docker run --rm -v $(pwd):/workspace fedora:latest bash -c "
  dnf install -y /workspace/*.rpm &&
  gitextensions --version
"
```

## Contributing

When modifying the packaging scripts:

1. Test on multiple Linux distributions
2. Verify all package types build successfully
3. Check that installed packages work correctly
4. Update this README if adding new features or requirements

## License

These packaging scripts are part of the Git Extensions project and follow the same license terms.

#!/usr/bin/env bash

set -e
set -o pipefail
set -u
echo This scripts not fully tested yet
#set -x  # Start printing commands

# Initialize build variables and display configuration
initialize_build_vars() {
    # do in build subfolder - imported scripts
    cd "$(dirname "$0")"
    # Source common configuration
    source "$PWD/common.sh"
    initialize_common
    
    # Generate packaging templates with current configuration
    source "$PWD/generate-templates.sh"
}
# Function to collect all .pdb files and archive them into a zip
collect_and_zip_pdb() {
    local pdb_dir="build/GitExtensions"
    local zip_name="pdb_files_$(date +%Y%m%d_%H%M%S).zip"
    echo "Collecting .pdb files from $pdb_dir and archiving into $zip_name..."
    find "$pdb_dir" -type f -name '*.pdb' -print | zip -@ "build/$zip_name"
    if [[ $? -eq 0 ]]; then
        echo "PDB files successfully archived into $zip_name."
    else
        echo "Failed to archive PDB files." >&2
        return 1
    fi
}
# Main execution (only run if script is executed directly, not sourced)
main() {
    # Initialize build configuration
    initialize_build_vars
    # Ensure we're in the project root
cd "$(dirname "$0")/.."
collect_and_zip_pdb

# Clean previous builds
echo "Cleaning previous builds..."
# rm -rf build/GitExtensions
# rm -rf build/*.deb
# rm -rf build/*.rpm
rm -v -rf build/*.AppImage

#   --self-contained true 
# Build the application
echo "Building Git Extensions..."
echo  Build the solution yourself
echo dotnet publish can fail in the end . 
echo Or use src/app/GitExtensions/Properties/PublishProfiles/FolderProfile.pubxml
# dotnet publish src/app/GitExtensions/GitExtensions.csproj \
#     --configuration $CONFIGURATION \
#     --runtime $RUNTIME \
#     --output build/GitExtensions \
#     -p:PublishSingleFile=false \
#     -p:PublishTrimmed=false

# Make the main executable... executable
chmod +x build/GitExtensions/GitExtensions

# Create a symlink with lowercase name for consistency
#ln -sf GitExtensions build/GitExtensions/gitextensions

# Export environment variables for the packaging script
export RUNTIME
export VERSION

cd build

# Run the packaging script
echo "Running packaging script..."
setup_icons
. "$PWD/scripts/package.linux.sh"

    echo "Build completed successfully!"
    echo "Generated packages:"
    ls -la *.deb *.rpm *.AppImage 2>/dev/null || echo "Some package types may not have been built"
}

# Only run main function if script is executed directly (not sourced)
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi

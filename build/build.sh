#!/usr/bin/env bash

set -e
set -o pipefail
set -u

#set -x  # Start printing commands

# Initialize build variables and display configuration
initialize_build_vars() {
    # Source common configuration
    source "$(dirname "$0")/common.conf"
    initialize_common
    
    # Generate packaging templates with current configuration
    source "$(dirname "$0")/generate-templates.sh"
}

# Main execution (only run if script is executed directly, not sourced)
main() {
    # Initialize build configuration
    initialize_build_vars

    # Ensure we're in the project root
cd "$(dirname "$0")/.."

# Clean previous builds
# echo "Cleaning previous builds..."
# rm -rf build/GitExtensions
# rm -rf build/*.deb
# rm -rf build/*.rpm
# rm -rf build/*.AppImage

#   --self-contained true 
# Build the application
echo "Building Git Extensions..."
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

# Run the packaging script
echo "Running packaging script..."
cd build
. "$PWD/scripts/package.linux.sh"

    echo "Build completed successfully!"
    echo "Generated packages:"
    ls -la *.deb *.rpm *.AppImage 2>/dev/null || echo "Some package types may not have been built"
}

# Only run main function if script is executed directly (not sourced)
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi

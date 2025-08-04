#!/bin/bash

# Variables
REPO="M-L-Ml/GitExtMoLiNet" # <-- Change this to your GitHub repo (e.g. user/project)
TAG="v0.1.0-alpha"
TITLE="Alpha Release"
BODY="Alpha release of GitExtensions. Includes Debug build artifacts.
- GitExtensions_Debug_1.zip: Contains GitExtensions.exe
- GitExtensions_Debug_2.zip: Contains GitExtensions.dll
"
ASSET1="GitExtensions_Debug_1.zip"
ASSET2="GitExtensions_Debug_2.zip"

# Create the release (will fail if tag already exists)
gh release create "$TAG" \
  --repo "$REPO" \
  --title "$TITLE" \
  --notes "$BODY" \
  --prerelease \
  "$ASSET1" "$ASSET2"
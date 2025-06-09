# Registry Access Refactoring Plan

## Current Usage Analysis

The codebase currently uses `Registry.CurrentUser` in a few locations:

1. `AppSettings.cs`: Direct access to HKEY_CURRENT_USER for storing GitExtensions settings
2. Demo/sample code in Windows API CodePack examples (not part of core functionality)

## Problems with Current Approach

1. **Violation of Single Responsibility Principle (SRP)**:
   - `AppSettings` class handles both application settings logic and registry access
   - Direct registry access is scattered across the codebase

2. **Violation of Interface Segregation Principle (ISP)**:
   - No clear interface defining registry operations
   - Consumers are forced to work with concrete registry implementation

3. **Violation of Dependency Inversion Principle (DIP)**:
   - Direct dependency on `Registry.CurrentUser`
   - Hard to test and mock registry operations
   - Platform-specific code not properly abstracted

## Proposed Solution
methods that use VersionIndependentRegKey and the VersionIndependentRegKey itself
should be moved to a new class
### 1. Create Abstraction
 

split SettingsSource, extract part wich ressambles the methods to a new base 
### 2. Implementation Class

 create derived class for that base , by moving implemetions to it

## Success Criteria

1. No direct usage of Registry.CurrentUser in application code
2. All settings operations going through abstraction layer
3. No regression in existing functionality

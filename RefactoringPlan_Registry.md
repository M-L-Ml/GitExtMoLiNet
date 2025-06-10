# Registry Access Refactoring Plan

## Current Usage Analysis

The codebase currently uses `Registry.CurrentUser` in a few locations:

1. `AppSettings.cs`: Direct access to HKEY_CURRENT_USER for storing GitExtensions settings
2. Demo/sample code in Windows API CodePack examples (not part of core functionality)


## Proposed Solution
methods that use VersionIndependentRegKey and the VersionIndependentRegKey itself
should be moved to a new class
#### 2. Implementation Class

 create derived class for that base , by moving implemetions to it
## Proposed Solution

The core idea is to encapsulate all direct Windows Registry interactions currently within `AppSettings.cs` into a new, dedicated nonstatic class. This will improve separation of concerns and make the `AppSettings` class less coupled to direct registry APIs.

### 1. Create a Dedicated Registry Access Class
---generate class GitExtensionsRegistry derived from SettingsSourceBase. make 
*   **Define a new  class:** For example, `GitExtensionsRegistry`. This class will be responsible for all direct interactions with `HKEY_CURRENT_USER\Software\GitExtensions`. Derived from existing "SettingsSourceBase"
*   **Move `VersionIndependentRegKey`:**
    *   The `private  RegistryKey _versionIndependentRegKey;` field from `AppSettings`.
    *   The `private  RegistryKey VersionIndependentRegKey` property, including its initialization logic (`Registry.CurrentUser.CreateSubKey("Software\\GitExtensions", ...)`) from `AppSettings`.
    *   These will become private  members of the new `GitExtensionsRegistry` class.
*   **Move Registry-Interacting Methods:**
    *   The following static methods (and any other similar ones directly using `VersionIndependentRegKey`) should do proxiing to GitExtensionsRegistry object .      
 
 And their implemention  should moved from `AppSettings` to `GitExtensionsRegistry` :
        *   `ReadStringRegValue(string key, string? defaultValue)`
        *   `WriteStringRegValue(string key, string value)`
        *   `ReadBoolRegKey(string key, bool defaultValue)`
        *   `WriteBoolRegKey(string key, bool value)`
 implementions to become overrides of SettingsSourceBase.
    *   rewrite it caller code with calls to SettingsSourceBase methods, which should be implemented accordingly.

### 2. Update Callers in `AppSettings`

*   Modify `AppSettings` to call the moved methods via the new class via new Property for it. For example:
    *   `ImportFromRegistry()` in `AppSettings` will call `GitExtensionsRegistry.GetSettingsFromRegistry()`.
    *   Any internal calls within `AppSettings` that previously used `ReadStringRegValue` directly will now call `GitExtensionsRegistry.ReadStringRegValue`.
*   This change isolates the direct registry dependency to the `GitExtensionsRegistry` class.

### 3. Handling Platform Specifics (`EnvUtils.IsMonoRuntime()`)

*   The logics of existing `EnvUtils.IsMonoRuntime()` checks within the moved methods (e.g., in `WriteStringRegValue`, and the `VersionIndependentRegKey` getter) should be preserved but outside  the new `GitExtensionsRegistry` class to ensure registry operations are only attempted on Windows. For this it to create another derived class of SettingsSourceBase. 

### Benefits of this Approach:

*   **Improved Cohesion:** Registry-specific logic is grouped in one place (`GitExtensionsRegistry`).
*   **Reduced Coupling:** `AppSettings` no longer directly depends on `Microsoft.Win32.Registry` APIs for these operations.
*   **Clearer Responsibilities:** `AppSettings` focuses more on managing application settings (primarily via its XML file), while `GitExtensionsRegistry` handles the specific legacy/direct registry interactions.

## Success Criteria

1. No direct usage of Registry.CurrentUser in application code
2. All settings operations going through abstraction layer
3. No regression in existing functionality

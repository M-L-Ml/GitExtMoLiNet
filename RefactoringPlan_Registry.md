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

### 1. Define and Implement `GitExtensionsRegistry` (derived from `SettingsSourceBase`)

The primary goal is to encapsulate direct Windows Registry operations for the `HKEY_CURRENT_USER\Software\GitExtensions` path into a new, **non-static** class named `GitExtensionsRegistry`. This class will implement the `SettingsSourceBase` interface (or derive from an abstract base class `SettingsSourceBase`, assuming one exists or will be created/identified).

*   **Define `GitExtensionsRegistry` Class:**
    *   Create a new **non-static** class `GitExtensionsRegistry` that inherits from `SettingsSourceBase`.
    *   This class will be specifically responsible for reading from and writing to the `Software\GitExtensions` subkey within `HKEY_CURRENT_USER`.

*   **Manage Registry Key Internally:**
    *   The concept of `VersionIndependentRegKey` (previously a static member in `AppSettings`) will be managed internally by instances of `GitExtensionsRegistry`.
    *   Each `GitExtensionsRegistry` instance will hold its own `Microsoft.Win32.RegistryKey` configured for `HKEY_CURRENT_USER\Software\GitExtensions`. This key will be opened on instantiation or first use and should be properly disposed of when the `GitExtensionsRegistry` instance is disposed (if `SettingsSourceBase` supports `IDisposable`).

*   **Implement `SettingsSourceBase` Methods:**
    *   `GitExtensionsRegistry` will **override** abstract methods (or implement interface methods) from `SettingsSourceBase` to perform registry operations. For example:
        *   `override string GetString(string key, string defaultValue)`: Will use its internal registry key to read the specified string value.
        *   `override void SetString(string key, string value)`: Will use its internal registry key to write the specified string value.
        *   `override bool GetBool(string key, bool defaultValue)`: Will read the boolean value (e.g., stored as "true"/"false") from the registry.
        *   `override void SetBool(string key, bool value)`: Will write the boolean value (e.g., as "true"/"false") to the registry.
    *   The implementation logic for these overrides will be based on the current static methods in `AppSettings` (e.g., `ReadStringRegValue`, `WriteStringRegValue`).

*   **Handle Legacy Settings Import (`GetSettingsFromRegistry()`):**
    *   The functionality of the current static `AppSettings.GetSettingsFromRegistry()` method (which enumerates all settings under `Software\GitExtensions\GitExtensions`) needs to be available through `GitExtensionsRegistry`.
    *   This could be a specific public method on `GitExtensionsRegistry`, such as `IEnumerable<(string name, string value)> GetAllSettings(string subKeyName = "GitExtensions")`. This method would be called by the refactored `AppSettings.ImportFromRegistry()`.

*   **Refactor Static Methods in `AppSettings` as Proxies:**
    *   The existing static methods in `AppSettings` (e.g., `ReadStringRegValue`, `WriteStringRegValue`, `ReadBoolRegKey`, `WriteBoolRegKey`, and `ImportFromRegistry`) will be modified to delegate their calls.
    *   They will no longer contain direct registry access logic. Instead, they will use an instance of `SettingsSourceBase`.
    *   This instance will be provided via a new static property in `AppSettings` (e.g., `private static SettingsSourceBase LegacyRegistrySettings { get; }`).
    *   This property will be responsible for instantiating and returning `GitExtensionsRegistry` on Windows, and a `NullSettingsSource` (a no-op implementation of `SettingsSourceBase`) on non-Windows platforms (checking `EnvUtils.IsMonoRuntime()`).
    *   Example of a refactored `AppSettings` method:
        `public static string ReadStringRegValue(string key, string defaultValue) => LegacyRegistrySettings.GetString(key, defaultValue);`
    *   The `AppSettings.ImportFromRegistry()` method would similarly use the `LegacyRegistrySettings` instance to call the method responsible for `GetAllSettings`.

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

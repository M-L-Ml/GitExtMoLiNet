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

### 1. Create Registry Access Abstraction

```csharp
public interface IRegistryProvider
{
    IRegistryKey OpenKey(string keyPath, bool writable = false);
    IRegistryKey CreateKey(string keyPath);
    void DeleteKey(string keyPath, bool recursive = false);
}

public interface IRegistryKey : IDisposable 
{
    string Name { get; }
    object? GetValue(string name, object? defaultValue = null);
    void SetValue(string name, object value);
    void DeleteValue(string name);
    bool ValueExists(string name);
    void Close();
}
```

### 2. Implementation Class

```csharp
public class WindowsRegistryProvider : IRegistryProvider
{
    private readonly RegistryHive _hive;
    
    public WindowsRegistryProvider(RegistryHive hive = RegistryHive.CurrentUser)
    {
        _hive = hive;
    }

    // Implementation of interface methods
}

public class WindowsRegistryKey : IRegistryKey
{
    private readonly RegistryKey _key;
    
    // Implementation of interface methods
}
```

### 3. Settings Storage Service

```csharp
public interface ISettingsStorage
{
    T? GetValue<T>(string key, T? defaultValue = default);
    void SetValue<T>(string key, T value);
    void DeleteValue(string key);
    bool Exists(string key);
}

public class RegistrySettingsStorage : ISettingsStorage
{
    private readonly IRegistryProvider _registry;
    private readonly string _basePath;

    public RegistrySettingsStorage(IRegistryProvider registry, string basePath)
    {
        _registry = registry;
        _basePath = basePath;
    }

    // Implementation of interface methods
}
```

## Implementation Steps

1. **Create New Classes and Interfaces**:
   - Define the interfaces and classes as shown above
   - Implement the Windows-specific registry provider
   - Add unit tests for the new components

2. **Modify AppSettings**:
   - Inject ISettingsStorage dependency
   - Replace direct registry access with storage service calls
   - Add factory method for creating the appropriate storage implementation

3. **Dependency Injection Setup**:
   - Register interfaces and implementations in DI container
   - Configure lifetime scope appropriately

4. **Transition Plan**:
   - Create new implementation alongside existing code
   - Gradually migrate usage to new abstractions
   - Add feature flag to control rollout
   - Validate changes in test environment

## Benefits

1. **Better Testability**:
   - Easy to mock registry operations
   - Can test settings logic without registry access

2. **Platform Independence**:
   - Abstract storage interface allows different implementations
   - Could support other storage mechanisms in future

3. **Separation of Concerns**:
   - Clear responsibility boundaries
   - Settings logic separated from storage details

4. **Improved Maintainability**:
   - Centralized registry access
   - Consistent error handling
   - Better dependency management

## Migration Strategy

1. **Phase 1 - Infrastructure**:
   - Create new interfaces and base implementations
   - Add unit tests for new components
   - Update dependency injection setup

2. **Phase 2 - Settings Migration**:
   - Create new RegistrySettingsStorage
   - Modify AppSettings to use new storage
   - Add feature flag for gradual rollout

3. **Phase 3 - Legacy Cleanup**:
   - Remove direct Registry.CurrentUser usage
   - Clean up old code paths
   - Update documentation

## Risk Mitigation

1. **Data Migration**:
   - Implement read from both old and new locations during transition
   - Add logging for migration issues
   - Create rollback plan

2. **Testing**:
   - Comprehensive unit tests for new components
   - Integration tests for settings migration
   - Beta testing with feature flags

3. **Monitoring**:
   - Add telemetry for settings access
   - Monitor for errors during migration
   - Track performance metrics

## Success Criteria

1. No direct usage of Registry.CurrentUser in application code
2. All settings operations going through abstraction layer
3. Full test coverage of new components
4. No regression in existing functionality
5. Improved maintainability metrics

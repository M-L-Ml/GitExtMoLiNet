#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using GitExtensions.Extensibility.Settings;
using Microsoft.Win32;

namespace GitCommands.Settings;

/// <summary>
/// Provides access to Git Extensions settings stored in the Windows Registry.
/// This class is intended for use on Windows platforms only.
/// </summary>
public class GitExtensionsRegistry : SettingsSourceBase, IDisposable
{
    private const string BaseRegistryPath = "Software\\GitExtensions";
    private RegistryKey? _baseRegistryKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitExtensionsRegistry"/> class.
    /// Opens the base registry key HKEY_CURRENT_USER\Software\GitExtensions.
    /// </summary>
    public GitExtensionsRegistry()
    {
        // This class should only be instantiated on Windows. Platform checks are done by the caller.
        _baseRegistryKey = Registry.CurrentUser.CreateSubKey(BaseRegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
    }

    /// <inheritdoc />
    public override string? GetValue(string name)
    {
        if (_baseRegistryKey is null)
        {
            // Should not happen if constructor succeeded and not disposed
            return null;
        }
        return _baseRegistryKey.GetValue(name)?.ToString();
    }

    /// <inheritdoc />
    public override void SetValue(string name, string? value)
    {
        if (_baseRegistryKey is null)
        {
            // Should not happen if constructor succeeded and not disposed
            return;
        }
        if (value is null)
        {
            _baseRegistryKey.DeleteValue(name, false); // Do not throw if not found
        }
        else
        {
            _baseRegistryKey.SetValue(name, value);
        }
    }

    /// <summary>
    /// Retrieves all string values from a specified subkey under the base GitExtensions registry path.
    /// </summary>
    /// <param name="subKeyName">The name of the subkey (e.g., "GitExtensions").</param>
    /// <returns>An enumerable of name-value pairs.</returns>
    public IEnumerable<(string name, string value)> GetAllSettings(string subKeyName)
    {
        if (_baseRegistryKey is null)
        {
            return Enumerable.Empty<(string name, string value)>();
        }

        using (RegistryKey? subKey = _baseRegistryKey.OpenSubKey(subKeyName))
        {
            if (subKey is null)
            {
                yield break;
            }

            foreach (string valueName in subKey.GetValueNames())
            {
                object? valueObject = subKey.GetValue(valueName, null);
                if (valueObject is not null)
                {
                    yield return (valueName, valueObject.ToString() ?? string.Empty);
                }
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _baseRegistryKey?.Dispose();
            _baseRegistryKey = null;
        }
    }

    // Finalizer in case Dispose is not called.
    ~GitExtensionsRegistry()
    {
        Dispose(false);
    }
}

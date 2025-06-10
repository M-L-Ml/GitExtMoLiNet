#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using GitCommands.Utils;
using GitExtensions.Extensibility.Settings;
using Microsoft;
using Microsoft.Win32;

namespace GitCommands.Settings;

/// <summary>
/// Provides access to Git Extensions settings stored in the Windows Registry.
/// This class is intended for use on Windows platforms only.
/// </summary>
public class GitExtensionsRegistry : SettingsSourceBase, IDisposable
{
    private const string BaseRegistryPath = "Software\\GitExtensions";

    /// <summary>
    /// Initializes a new instance of the <see cref="GitExtensionsRegistry"/> class.
    /// Opens the base registry key HKEY_CURRENT_USER\Software\GitExtensions.
    /// </summary>
    public GitExtensionsRegistry()
    {
    }


    private RegistryKey? _versionIndependentRegKey;
        // This class should only be instantiated on Windows. Platform checks are done by the caller.

    private RegistryKey VersionIndependentRegKey
    {
        get
        {
            if (_versionIndependentRegKey is null)
            {
                //if (EnvUtils.IsMonoRuntime())
                 //   return _versionIndependentRegKey;
                _versionIndependentRegKey = Registry.CurrentUser.CreateSubKey(BaseRegistryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                Validates.NotNull(_versionIndependentRegKey);
            }

            return _versionIndependentRegKey;
        }
    }
    public override bool? GetBool(string name)
    // private bool ReadBoolRegKey(string key, bool defaultValue)
    {
        object? obj = VersionIndependentRegKey.GetValue(name);
        if (obj is not string)
        {
            obj = null;
        }

        if (obj is null)
        {
            return null;
        }

        return ((string)obj).Equals("true", StringComparison.CurrentCultureIgnoreCase);
    }
    /// <inheritdoc />
    public override string? GetValue(string name)
    {
        if (VersionIndependentRegKey is null)
        {
            // Should not happen if constructor succeeded and not disposed
            return null;
        }
        return VersionIndependentRegKey.GetValue(name)?.ToString();
    }

    /// <inheritdoc />
    public override void SetValue(string name, string? value)
    {
        if (VersionIndependentRegKey is null)
        {
            // Should not happen if constructor succeeded and not disposed
            return;
        }
        if (value is null)
        {
            VersionIndependentRegKey.DeleteValue(name, false); // Do not throw if not found
        }
        else
        {
            VersionIndependentRegKey.SetValue(name, value);
        }
    }

    /// <summary>
    /// Retrieves all string values from a specified subkey under the base GitExtensions registry path.
    /// </summary>
    /// <param name="subKeyName">The name of the subkey (e.g., "GitExtensions").</param>
    /// <returns>An enumerable of name-value pairs.</returns>
    public IEnumerable<(string name, string value)> GetAllSettings(string subKeyName)
    {
        if (VersionIndependentRegKey is null)
        {
            throw new ArgumentNullException(nameof(VersionIndependentRegKey));
            //return Enumerable.Empty<(string name, string value)>();
            yield break;

        }

        using (RegistryKey? subKey = VersionIndependentRegKey.OpenSubKey(subKeyName))
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
            _versionIndependentRegKey?.Dispose();
            _versionIndependentRegKey = null;
        }
    }

    // Finalizer in case Dispose is not called.
    ~GitExtensionsRegistry()
    {
        Dispose(false);
    }
}

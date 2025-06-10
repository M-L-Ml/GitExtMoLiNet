#nullable enable

using GitExtensions.Extensibility.Settings;

namespace GitCommands.Settings;

/// <summary>
/// A settings source that performs no operations. Used on non-Windows platforms
/// where registry settings are not applicable.
/// </summary>
public class NullSettingsSource : SettingsSourceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullSettingsSource"/> class.
    /// </summary>
    public NullSettingsSource()
    {
    }

    /// <inheritdoc />
    public override string? GetValue(string name)
    {
        // Always return null as no settings are stored or retrieved.
        return null;
    }

    /// <inheritdoc />
    public override void SetValue(string name, string? value)
    {
        // No operation, as no settings are stored.
    }
}

namespace GitCommands.Settings
{
    /// <summary>
    /// An adapter to allow the static <see cref="AppSettings"/> to be used as an <see cref="IAppSettings"/> instance.
    /// </summary>
    internal sealed class AppSettingsAdapter : IAppSettings
    {
        public string? LinuxToolsDir => AppSettings.LinuxToolsDir;
        public string? CustomHomeDir => AppSettings.CustomHomeDir;
        public bool UserProfileHomeDir => AppSettings.UserProfileHomeDir;
        public string? GetInstallDir() => AppSettings.GetInstallDir();
    }
}

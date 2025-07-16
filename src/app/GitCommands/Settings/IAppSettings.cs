namespace GitCommands.Settings
{
    public interface IAppSettings
    {
        string? LinuxToolsDir { get; }
        string? CustomHomeDir { get; }
        bool UserProfileHomeDir { get; }
        string? GetInstallDir();
    }
}

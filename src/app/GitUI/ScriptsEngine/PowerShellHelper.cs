using GitCommands;
using GitExtensions.Extensibility;

namespace GitUI.ScriptsEngine
{
    public static class PowerShellHelper
    {
        internal static void RunPowerShell(string command, string? argument, string workingDir, bool runInBackground, EnvironmentConfiguration environmentConfiguration = default)
        {
            const string filename = "powershell.exe";
            string arguments = (runInBackground ? "" : "-NoExit") + " -ExecutionPolicy Unrestricted -Command \"" + command + " " + argument + "\"";
            environmentConfiguration ??= EnvironmentConfiguration.Instance;
            environmentConfiguration.SetEnvironmentVariables();

            IExecutable executable = new Executable(() => filename, workingDir, environmentConfiguration: environmentConfiguration);
            executable.Start(arguments, createWindow: !runInBackground);
        }
    }
}

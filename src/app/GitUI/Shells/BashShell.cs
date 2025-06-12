using GitCommands;
using GitExtUtils;
using GitUI.Properties;

namespace GitUI.Shells
{
    public class BashShell : ShellDescriptor
    {
        private static string GitBashExe => EnvUtils.RunningOnWindows() ? "git-bash.exe" : "git-bash"; // Bash with git in the path, should                           
        private static string BashExe => EnvUtils.RunningOnWindows() ? "bash.exe" : "bash";            // Fallback to generic bash, should                               
        private static string ShExe => EnvUtils.RunningOnWindows() ? "sh.exe" : "sh";                  // Fallback to SH

        public const string ShellName = "bash";

        public BashShell()
        {
            string gitBash = GitBashExe;
            if (EnvUtils.RunningOnUnix())
            {
                gitBash = ShellName;
            }
            Name = ShellName;
            Icon = Images.GitForWindows;

            if (PathUtil.TryFindShellPath(gitBash, out string? exePath))
            {
                ExecutableName = gitBash;
                ExecutablePath = exePath;

                // Try to find bash or sh below to set ExecutableCommandLine, as git-bash.exe cannot be connected to the built-in console.
            }
            else

                foreach (string shellExecutableName in new string[] { BashExe, ShExe })
                {
                    if (PathUtil.TryFindShellPath(shellExecutableName, out exePath))
                    {
                        if (ExecutablePath is null)
                        {
                            ExecutableName = shellExecutableName;
                            ExecutablePath = exePath;
                        }

                        ExecutableCommandLine = $"{exePath.Quote()} --login -i";

                        break;
                    }
                }
        }

        public override string GetChangeDirCommand(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new(path);
                if (directoryInfo.Exists)
                {
                    string posixPath = "/" + directoryInfo.FullName.ToPosixPath().Remove(1, 1);
                    return $"cd {posixPath.QuoteNE()}";
                }
            }
            catch
            {
                // no-op
            }

            return string.Empty;
        }
    }
}

using System.Diagnostics;
using System.Drawing;
using GitCommands.Utils;
using GitExtUtils;
using static System.Environment;

namespace GitCommands
{
    public static class EnvironmentConfiguration
    {
        private static readonly IEnvironmentAbstraction Env = new EnvironmentAbstraction();

        /// <summary>
        /// The <c>USER</c> environment variable's value for the user/machine.
        /// </summary>
        private static readonly string? UserHomeDir
            = Env.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.User)
           ?? Env.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.Machine);

        /// <summary>
        /// Sets <c>PATH</c>, <c>HOME</c>, <c>TERM</c> and <c>SSH_ASKPASS</c> environment variables
        /// for the current process.
        /// </summary>
        public static void SetEnvironmentVariables()
        {
            // PATH variable

            if (!string.IsNullOrEmpty(AppSettings.LinuxToolsDir))
            {
                // Ensure the GNU/Linux tools dir is on the path
                string? path = Env.GetEnvironmentVariable("PATH");

                if (path is null)
                {
                    Env.SetEnvironmentVariable("PATH", AppSettings.LinuxToolsDir);
                }
                else if (!path.Contains(AppSettings.LinuxToolsDir))
                {
                    Env.SetEnvironmentVariable("PATH", $"{path}{Path.PathSeparator}{AppSettings.LinuxToolsDir}");
                }
            }

            // HOME variable
            string userHomeDir = Environment.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable("HOME", EnvironmentVariableTarget.Machine);
            var hom = Env.GetEnvironmentVariable("HOME") ?? userHomeDir;
            Env.SetEnvironmentVariable("HOME", ComputeHomeLocation() ?? hom);
            if (EnvUtils.RunningOnUnix())
            {
                // see         private static string ReadXdgDirectory(string homeDir, string key, string fallback)
                hom = Env.GetEnvironmentVariable("HOME") ?? hom;
                if (string.IsNullOrEmpty(hom))
                {
                    var user = Env.GetEnvironmentVariable("LOGNAME")
                        ?? Env.GetEnvironmentVariable("USER")
                        ?? Env.GetEnvironmentVariable("USERNAME");
                    if (!string.IsNullOrEmpty(user))
                    {
                        hom = Path.Combine("/home", user);
                        if (Directory.Exists(hom))
                        {
                            Env.SetEnvironmentVariable("HOME", hom);
                        }
                        else
                        {
                            Debug.Assert(false, "HOME directory does not exist: " + hom);
                        }
                    }
                    else
                    {
                        Env.SetEnvironmentVariable("HOME", "/home");
                    }
                }

                //return ReadXdgDirectory(home, "XDG_DESKTOP_DIR", "Desktop");
                //case SpecialFolder.ApplicationData:
                //    return GetXdgConfig(home);
                //case SpecialFolder.LocalApplicationData:
                //    // "$XDG_DATA_HOME defines the base directory relative to which user specific data files should be stored."
                //    // "If $XDG_DATA_HOME is either not set or empty, a default equal to $HOME/.local/share should be used."
                //    string? data = GetEnvironmentVariable("XDG_DATA_HOME");
                //    if (data is null || !data.StartsWith('/'))
                //    {
                //        data = Path.Combine(home, ".local", "share");
                //    }
                //    return data;
                //case SpecialFolder.MyDocuments: // same value as Personal
                //    return ReadXdgDirectory(home, "XDG_DOCUMENTS_DIR", "Documents");
                //case SpecialFolder.MyMusic:
                //    return ReadXdgDirectory(home, "XDG_MUSIC_DIR", "Music");
                //case SpecialFolder.MyVideos:
                //    return ReadXdgDirectory(home, "XDG_VIDEOS_DIR", "Videos");
                //case SpecialFolder.MyPictures:
                //    return ReadXdgDirectory(home, "XDG_PICTURES_DIR", "Pictures");
                //case SpecialFolder.Fonts:


                //    Environment.GetFolderPath(Environment.SpecialFolder.Recent);

                //case UIIcon.PlacesDesktop:
                //        return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                //    case UIIcon.PlacesPersonal:
                //        return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //    case UIIcon.PlacesMyComputer:
                //        return Environment.GetFolderPath(Environment.SpecialFolder.MyComputer
                //
                // using Environment.SpecialFolder;

                var tups = new (Environment.SpecialFolder enn, string vname, string desc)[]
            {
                    (SpecialFolder.DesktopDirectory, "XDG_DESKTOP_DIR", "Desktop"),
                    (SpecialFolder.MyDocuments, "XDG_DOCUMENTS_DIR", "Documents"),
                    (SpecialFolder.MyMusic, "XDG_MUSIC_DIR", "Music"),
                    (SpecialFolder.MyVideos, "XDG_VIDEOS_DIR", "Videos"),
                    (SpecialFolder.MyPictures, "XDG_PICTURES_DIR", "Pictures"),
            };

                foreach (var (enn, vname, desc) in tups)
                {
                    var dir = GetFolderPath(enn);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        // If the directory exists, it will be used - no error
                        continue;
                    }

                    string? dir2 = Env.GetEnvironmentVariable(vname);
                    if (!string.IsNullOrEmpty(dir2))
                    {
                        Debug.Assert(false, "check this , wrong env vars? ");
                        if (Directory.Exists(dir2))
                        {

                            // If the directory exists, it will be used - no error
                            continue;
                        }
                    }
                    if (enn == SpecialFolder.DesktopDirectory)
                    {
                        Env.SetEnvironmentVariable(vname, "/");
                    }
                    else
                        // If the directory does not exist
                        //set  default value
                        Env.SetEnvironmentVariable(vname, Env.GetEnvironmentVariable("HOME"));
                    Debug.Assert(!string.IsNullOrEmpty(GetFolderPath(enn)));
                }
            }
            // TERM variable

            // to prevent from leaking processes see issue #1092 for details
            Env.SetEnvironmentVariable("TERM", "msys");

            // Force a non-empty DISPLAY so ssh uses SSH_ASKPASS if it has no terminal
            if (string.IsNullOrEmpty(Env.GetEnvironmentVariable("DISPLAY")))
            {
                Env.SetEnvironmentVariable("DISPLAY", ":");
            }

            // SSH_ASKPASS variable

            if (EnvUtils.RunningOnWindows())
            {
                string sshAskPass = Path.Combine(AppSettings.GetInstallDir(), "GitExtSshAskPass.exe");

                if (File.Exists(sshAskPass))
                {
                    Env.SetEnvironmentVariable("SSH_ASKPASS", sshAskPass);
                }
            }
            else if (string.IsNullOrEmpty(Env.GetEnvironmentVariable("SSH_ASKPASS")))
            {
                Env.SetEnvironmentVariable("SSH_ASKPASS", "ssh-askpass");
            }

            if (!string.IsNullOrEmpty(Env.GetEnvironmentVariable("SSH_ASKPASS")))
            {
                Env.SetEnvironmentVariable("SSH_ASKPASS_REQUIRE", "force");
            }

            return;

            static string? ComputeHomeLocation()
            {
                if (!string.IsNullOrEmpty(AppSettings.CustomHomeDir))
                {
                    return AppSettings.CustomHomeDir;
                }

                if (AppSettings.UserProfileHomeDir)
                {
                    return Env.GetEnvironmentVariable("USERPROFILE");
                }

                return GetDefaultHomeDir();
            }
        }

        /// <summary>
        /// Gets the value of the current process's <c>HOME</c> environment variable.
        /// </summary>
        /// <returns>The variable's value, or an empty string if it is not present.</returns>
        public static string GetHomeDir()
        {
            return Env.GetEnvironmentVariable("HOME") ?? "";
        }

        public static string? GetDefaultHomeDir()
        {
            // Use the HOME property from the user or machine, as captured at startup
            if (!string.IsNullOrEmpty(UserHomeDir))
            {
                return UserHomeDir;
            }

            if (EnvUtils.RunningOnWindows())
            {
                // Use the Windows default home directory
                string homeDrive = Env.GetEnvironmentVariable("HOMEDRIVE");

                if (!string.IsNullOrEmpty(homeDrive))
                {
                    return homeDrive + Env.GetEnvironmentVariable("HOMEPATH");
                }

                return Env.GetEnvironmentVariable("USERPROFILE");
            }

            return Env.GetFolderPath(SpecialFolder.Personal);
        }
    }
}

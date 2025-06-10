using System.Diagnostics;
using System.Runtime.Versioning;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GitCommands.Utils
{
    public static class EnvUtils
    {
        [SupportedOSPlatformGuard("windows")]
        public static bool RunningOnWindows()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
                default:
                    return false;
            }
        }

        public static bool RunningOnWindowsWithMainWindow()
        {
            if (!RunningOnWindows())
            {
                return false;
            }

            Process currentProcess = Process.GetCurrentProcess();
            if (currentProcess is null)
            {
                return false;
            }

            return currentProcess.MainWindowHandle != IntPtr.Zero;
        }

        public static bool IsWindowsVistaOrGreater()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                   && Environment.OSVersion.Version.CompareTo(new Version(6, 0)) >= 0;
        }

        public static bool IsWindows7OrGreater()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                   && Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0;
        }

        public static bool IsWindows8OrGreater()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                   && Environment.OSVersion.Version.CompareTo(new Version(6, 2)) >= 0;
        }

        public static bool IsWindows8Point1OrGreater()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                   && Environment.OSVersion.Version.CompareTo(new Version(6, 3)) >= 0;
        }


        [SupportedOSPlatformGuard("linux")]
        public static bool RunningOnUnix()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        [SupportedOSPlatformGuard("macos")]
        public static bool RunningOnMacOSX()
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }

        [SupportedOSPlatformGuard("linux")]
        [MustUseReturnValue]
        public static bool IsMonoRuntime()
        {
            return RunningOnUnix();
            //or return Type.GetType( "Mono.Posix.Signals") != null;
            //or return GetAssembly( "Mono.Posix.NETStandard.dll" )!= null;
            //return Type.GetType("Mono.Runtime") != null;
        }

        [UnsupportedOSPlatformGuard("windows")]
        [MustUseReturnValue]
        public static bool IsMonoRuntimeOrMForms()
        {
            return IsMonoRuntime() ||
            //or return
            Type.GetType("Mono.Posix.Signals") != null

#if !(!__MonoCS__ && WINDOWS && WINDOWS_OWN)
            || true;
#endif
            //or return GetAssembly( "Mono.Posix.NETStandard.dll" )!= null;
            //return Type.GetType("Mono.Runtime") != null;
        }
        public static bool IsNet4FullOrHigher()
        {
            if (Environment.Version.Major > 4)
            {
                return true;
            }

            if (Environment.Version.Major == 4)
            {
                if (Environment.Version.Minor >= 5)
                {
                    return true;
                }

                try
                {
                    RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full", false);
                    if (registryKey is not null)
                    {
                        using (registryKey)
                        {
                            object v = registryKey.GetValue("Install");
                            return v?.ToString() is "1";
                        }
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    System.Diagnostics.Trace.WriteLine(e);
                }
            }

            return false;
        }

        public static string? ReplaceLinuxNewLinesDependingOnPlatform(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (RunningOnUnix())
            {
                return s;
            }

            return s.Replace("\n", Environment.NewLine);
        }

        public static char EnvVariableSeparator => RunningOnWindows() ? ';' : ':';
    }
}

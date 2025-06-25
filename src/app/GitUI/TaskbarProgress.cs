//#if WINDOWS && WINDOWSAPICODEPACK
using GitCommands.Utils;
using GitExtUtils;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace GitUI
{
    #if !(WINDOWS && WINDOWSAPICODEPACK)
    [Obsolete("This class is not implemented on Linux with the current version of ported WinForms. The Windows API Code Pack.")]
    #endif
    public static class TaskbarProgress
    {
        private static void Try(Action<TaskbarManager> action)
        {
            // This class is not implemented on Linux with the current version of ported WinForms. The Windows API Code Pack.
            if (EnvUtils.IsMonoRuntimeOrMForms())
            {
                return;
            }
            if (EnvUtils.RunningOnWindowsWithMainWindow() && TaskbarManager.IsPlatformSupported)
            {
                try
                {
                    action(TaskbarManager.Instance);
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
#if true

        public static void Clear()
        {
            Try(taskbar => taskbar.SetProgressState(TaskbarProgressBarState.NoProgress));
        }

        public static void SetProgress(TaskbarProgressBarState state, int progressValue, int maximumValue)
        {
            Try(taskbar =>
            {
                taskbar.SetProgressState(state);
                taskbar.SetProgressValue(progressValue, maximumValue);
            });
        }

        public static void SetState(TaskbarProgressBarState state)
        {
            Try(taskbar => taskbar.SetProgressState(state));
        }
#else
        public static void Clear() { }
        public static void SetProgress(object state, int progressValue, int maximumValue) { }
        public static void SetState(object state) { }
#endif
    }
}



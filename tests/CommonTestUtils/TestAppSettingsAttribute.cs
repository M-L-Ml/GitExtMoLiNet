using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using BuildXL.Utilities.Core;
using GitCommands;
using GitExtUtils;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
namespace CommonTestUtils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class TestAppSettingsAttribute : Attribute, ITestAction
    {
        private readonly INamedSemaphore _semaphore = GetSemaphore();

        public TestAppSettingsAttribute()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyLoadContext currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
            currentContext.Unloading += (a) =>
            {
                _semaphore?.Dispose();
            };
        }

        private static INamedSemaphore GetSemaphore()
        {
            return SemaphoreFactory.CreateOrOpen(initialCount: 1, maximumCount: 1, name: (EnvUtils.RunningOnWindows() ? "" : "/") + "GitExtensionsTestAssemblySerializer").ThrowIfFailure().Result;
        }

        public ActionTargets Targets => ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            _semaphore.WaitOne();

            File.Delete(AppSettings.SettingsContainer.SettingsCache.SettingsFilePath);
            AppSettings.SettingsContainer.SettingsCache.Load();

            AppSettings.CheckForUpdates = false;
            AppSettings.ShowAvailableDiffTools = false;

            // Create the settings file so that the SettingsCache does not think it should reload the file again and again
            AppSettings.SettingsContainer.SettingsCache.Save();
        }

        public void AfterTest(ITest test)
        {
            AppSettings.SettingsContainer.SettingsCache.Dispose();

            _semaphore.Release();
        }
    }
}

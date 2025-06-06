using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System
{
    internal static partial class NativeMethods
    {
        [SupportedOSPlatform("windows")]
        [LibraryImport(Libraries.UxTheme)]
        private static unsafe partial int SetWindowTheme(IntPtr hWnd, char* pszSubAppName, char* pszSubIdList);

        [SupportedOSPlatform("windows")]
        public static unsafe int SetWindowTheme(IntPtr hWnd, string subAppName, string? subIdList)
        {
            fixed (char* pszSubAppName = subAppName)
            {
                fixed (char* pszSubIdList = subIdList)
                {
                    return SetWindowTheme(hWnd, pszSubAppName, pszSubIdList);
                }
            }
        }
    }
}

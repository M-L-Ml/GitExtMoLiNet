using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static System.Interop;

namespace System
{
    internal static partial class NativeMethods
    {
        [SupportedOSPlatform("windows")]
        [LibraryImport(Libraries.User32, SetLastError = true)]
        public static partial BOOL DestroyIcon(IntPtr handle);
    }
}

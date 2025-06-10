using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System
{
    internal static partial class NativeMethods
    {
        [SupportedOSPlatform("windows")]
        [LibraryImport(Libraries.UxTheme, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int SetWindowTheme(nint hWnd, string? pszSubAppName, string? pszSubIdList);

    }
}

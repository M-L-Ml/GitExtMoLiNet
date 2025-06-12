using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System;

[SupportedOSPlatform("windows")]
internal static partial class NativeMethods
{
    internal const int ComCtl32CLRNone = unchecked((int)0xFFFFFFFF);

    [LibraryImport("comctl32.dll", EntryPoint = "ImageList_SetBkColor")]
    internal static partial int ImageListSetBkColor(IntPtr himl, int clrBk);
}

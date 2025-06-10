using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;

namespace GitExtUtils.GitUI
{
    using static DpiUtilImpl;

    /// <summary>
    /// Utility class related to DPI settings, primarily used for scaling dimensions on high-DPI displays.
    /// Non-static implementation of DPI utilities.
    /// </summary>
    public partial class DpiUtilImpl
    {
        public int DpiX { get; init; }
        public int DpiY { get; init; }

        public float ScaleX { get; init; }
        public float ScaleY { get; init; }

        /// <summary>
        /// Gets whether the current pixel density is not 96 DPI.
        /// </summary>
        public bool IsNonStandard => DpiX != 96 || DpiY != 96;

        /// <summary>
        /// Returns a scaled copy of <paramref name="size"/> which takes equivalent
        /// screen space at the current DPI as the original would at 96 DPI.
        /// </summary>
        public Size Scale(Size size)
        {
            Scale(ref size);
            return size;
        }

        /// <summary>
        /// Returns a scaled copy of <paramref name="size"/> which takes equivalent
        /// screen space at the current DPI as the original would at <paramref name="originalDpi"/>.
        /// </summary>
        public Size Scale(Size size, int originalDpi)
        {
            float scale = (float)DpiX / originalDpi;

            return new Size(
                (int)(size.Width * scale),
                (int)(size.Height * scale));
        }

        /// <summary>
        /// Modifies <paramref name="size"/> in place so that it takes equivalent screen
        /// space at the current DPI as the original value would at 96 DPI.
        /// </summary>
        public void Scale(ref Size size)
        {
            size.Width = (int)(size.Width * ScaleX);
            size.Height = (int)(size.Height * ScaleY);
        }

        /// <summary>
        /// Returns a scaled copy of measurement <paramref name="i"/> which has
        /// equivalent length on screen at the current DPI at the original would
        /// at 96 DPI.
        /// </summary>
        public int Scale(int i)
        {
            return (int)Math.Round(i * ScaleX);
        }

        /// <summary>
        /// Returns a scaled copy of <paramref name="i"/> which has equivalent
        /// length on screen at the current DPI as the original would at
        /// <paramref name="originalDpi"/>.
        /// </summary>
        public int Scale(int i, int originalDpi)
        {
            float scale = (float)DpiX / originalDpi;

            return (int)(i * scale);
        }

        /// <summary>
        /// Returns a scaled copy of measurement <paramref name="i"/> which has
        /// equivalent length on screen at the current DPI at the original would
        /// at 96 DPI.
        /// </summary>
        public float Scale(float i)
        {
            return (float)Math.Round(i * ScaleX);
        }

        /// <summary>
        /// Returns a scaled copy of <paramref name="f"/> which has equivalent
        /// length on screen at the current DPI as the original would at
        /// <paramref name="originalDpi"/>.
        /// </summary>
        public float Scale(float f, int originalDpi)
        {
            float scale = (float)DpiX / originalDpi;

            return f * scale;
        }

        /// <summary>
        /// Modifies <paramref name="point"/> in place so that it has equivalent physical
        /// screen position at the current DPI as the original value would at 96 DPI.
        /// </summary>
        public Point Scale(Point point)
        {
            return new Point(
                (int)(point.X * ScaleX),
                (int)(point.Y * ScaleY));
        }

        /// <summary>
        /// Modifies <paramref name="point"/> in place so that it has equivalent physical
        /// screen position at the current DPI as the original value would at <paramref name="originalDpi"/>.
        /// </summary>
        public Point Scale(Point point, int originalDpi)
        {
            float scale = (float)DpiX / originalDpi;

            return new Point(
                (int)(point.X * scale),
                (int)(point.Y * scale));
        }

        /// <summary>
        /// Returns a scaled copy of <paramref name="padding"/> which takes equivalent
        /// screen space at the current DPI as the original would at 96 DPI.
        /// </summary>
        public Padding Scale(Padding padding)
        {
            return new Padding((int)(padding.Left * ScaleX),
                               (int)(padding.Top * ScaleX),
                               (int)(padding.Right * ScaleX),
                               (int)(padding.Bottom * ScaleX));
        }

        [NotNull]
        public Image Scale([NotNull] Image image)
        {
            const string dpiScaled = "__DPI_SCALED__";

            if (!IsNonStandard || image.Tag as string == dpiScaled)
            {
                return image;
            }

            Size size = Scale(new Size(image.Width, image.Height));
            Bitmap bitmap = new(size.Width, size.Height);

            using Graphics g = Graphics.FromImage(bitmap);

            // NearestNeighbor is better for 200% and above
            // https://devblogs.microsoft.com/visualstudio/improving-high-dpi-support-for-visual-studio-2013/

            g.InterpolationMode = ScaleX >= 2
                ? InterpolationMode.NearestNeighbor
                : InterpolationMode.HighQualityBicubic;

            g.DrawImage(image, new Rectangle(Point.Empty, size));

            bitmap.Tag = dpiScaled;

            return bitmap;
        }

        [SupportedOSPlatform("windows")]
        [LibraryImport("gdi32.dll")]
        public static partial int GetDeviceCaps(DeviceContextSafeHandle hdc, int index);

        [SupportedOSPlatform("windows")]
        [LibraryImport("user32.dll")]
        public static partial DeviceContextSafeHandle GetDC(IntPtr hwnd);

        [SupportedOSPlatform("windows")]
        [LibraryImport("user32.dll")]
        public static partial int ReleaseDC(IntPtr hwnd, IntPtr deviceContextHandle);

        [SupportedOSPlatform("windows")]
        [UsedImplicitly]
        public sealed class DeviceContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            /// <summary>
            /// Called by P/Invoke.
            /// </summary>
            public DeviceContextSafeHandle()
                : base(ownsHandle: true)
            {
            }

            protected override bool ReleaseHandle()
            {
                ReleaseDC(IntPtr.Zero, handle);
                return true;
            }
        }
    }

    public static class DpiUtil
    {


        private static readonly DpiUtilImpl Instance;

        [SupportedOSPlatform("windows")]
        static DpiUtil()
        {
            //using static Instance;
            if (EnvUtils2.IsMonoRuntime())
            {
                Instance = new()
                {
                    DpiX = 96,
                    DpiY = 96,
                    ScaleX = 1.0f,
                    ScaleY = 1.0f
                };
                return;
            }

            using DeviceContextSafeHandle hdc = GetDC(IntPtr.Zero);
            try
            {
                const int LOGPIXELSX = 88;
                const int LOGPIXELSY = 90;
                int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                int dpiY = GetDeviceCaps(hdc, LOGPIXELSY);
                Instance = new()
                {
                    DpiX = dpiX,
                    DpiY = dpiY,

                    ScaleX = dpiX / 96.0f,
                    ScaleY = dpiY / 96.0f
                };
            }
            catch
            {
                Instance = new()
                {
                    DpiX = 96,
                    DpiY = 96,
                    ScaleX = 1.0f,
                    ScaleY = 1.0f
                };
            }
        }
        public static int DpiX => Instance.DpiX;


        public static int DpiY => Instance.DpiY;

        public static float ScaleX => Instance.ScaleX;
        public static float ScaleY => Instance.ScaleY;

        public static bool IsNonStandard => Instance.IsNonStandard;

        /// <summary>
        /// <inheritdoc cref="DpiUtilImpl.Scale(Size)"/>
        /// </summary>
        public static Size Scale(Size size) => Instance.Scale(size);
        public static Size Scale(Size size, int originalDpi) => Instance.Scale(size, originalDpi);
        public static void Scale(ref Size size) => Instance.Scale(ref size);
        public static int Scale(int i) => Instance.Scale(i);
        public static int Scale(int i, int originalDpi) => Instance.Scale(i, originalDpi);
        public static float Scale(float i) => Instance.Scale(i);
        public static float Scale(float f, int originalDpi) => Instance.Scale(f, originalDpi);
        public static Point Scale(Point point) => Instance.Scale(point);
        public static Point Scale(Point point, int originalDpi) => Instance.Scale(point, originalDpi);
        public static Padding Scale(Padding padding) => Instance.Scale(padding);
        public static Image Scale(Image image) => Instance.Scale(image);
    }

    internal class EnvUtils2
    {
        public static bool RunningOnUnix()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        public static bool RunningOnMacOSX()
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }

        public static bool IsMonoRuntime()
        {
            return RunningOnUnix();
            //or return Type.GetType( "Mono.Posix.Signals") != null;
            //or return GetAssembly( "Mono.Posix.NETStandard.dll" )!= null;
            //return Type.GetType("Mono.Runtime") != null;
        }

    }
}

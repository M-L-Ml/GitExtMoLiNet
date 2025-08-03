using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GitExtUtils
{
    public static class FileUtility
    {
        /// <summary>
        /// Writes all text to a file. Works around issues with hidden files encountered by File.WriteAllText.
        /// </summary>
        /// <param name="fileName">Destination file.</param>
        /// <param name="contents">Text to write as file contents.</param>
        /// <param name="encoding">Encoding used for StreamWriter to convert text to bytes.</param>
        /// <param name="filePreamble">File preamble (such as BOM) to put at the beginning of a file.</param>
        public static void SafeWriteAllText(string fileName, string contents, Encoding encoding, byte[] filePreamble)
        {
            using FileStream fs = new(fileName, FileMode.Open);
            using (TextWriter tw = new StreamWriter(fs, encoding, bufferSize: 4096, leaveOpen: true))
            {
                if (filePreamble.Length > 0)
                {
                    fs.Write(filePreamble, 0, filePreamble.Length);
                }

                tw.Write(contents);
            }

            // after flushing, set the stream length to the current position in order to truncate leftover text
            fs.SetLength(fs.Position);
        }


        /// <summary>
        /// Register the import resolver before calling the imported function.
        /// Only one import resolver can be set for a given assembly.
        /// <code> NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);  </code>
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="assembly"></param>
        /// <param name="searchPath"></param>
        /// <returns></returns>
        public static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {

            // or fix using
            // sudo ln -s /usr/lib/x86_64-linux-gnu/libXinerama.so.1 /usr/lib/x86_64-linux-gnu/libXinerama.so
            switch (libraryName.ToLowerInvariant())
            {
                case "System.Windows.Forms":

                    break;
                case "xinerama":
                case "libxinerama":
                case "libxinerama.so":
                case "libxinerama.so.1":
                    {
                        string[] paths = ["/usr/lib/x86_64-linux-gnu/libXinerama.so", "/usr/lib/x86_64-linux-gnu/libXinerama.so.1"];
                                              foreach (string path in paths)
                        {
                            // Attempt to load the library from the custom path
                            if (NativeLibrary.TryLoad(path, out IntPtr handle))
                            {
                                return handle;
                            }
                        }
                    }
                    break;

            }

            // Otherwise, fallback to default import resolver.
            return IntPtr.Zero;
        }
    }
}

# GitExtensions Linux Runtime Dependencies

This document lists the runtime dependencies for GitExtensions on Linux, discovered through `lsof` analysis of the running application.

## Runtime Analysis Results

The following shared object files are loaded by GitExtensions at runtime:

### .NET Core Runtime Dependencies
- `/usr/lib/dotnet/shared/Microsoft.NETCore.App/9.0.7/libcoreclr.so`
- `/usr/lib/dotnet/shared/Microsoft.NETCore.App/9.0.7/libclrjit.so`
- `/usr/lib/dotnet/shared/Microsoft.NETCore.App/9.0.7/libhostpolicy.so`
- `/usr/lib/dotnet/shared/Microsoft.NETCore.App/9.0.7/libSystem.Native.so`
- `/usr/lib/dotnet/shared/Microsoft.NETCore.App/9.0.7/libSystem.Security.Cryptography.Native.OpenSsl.so`
- `/usr/lib/dotnet/shared/Microsoft.NETCore.App/9.0.7/libcoreclrtraceptprovider.so`
- `/usr/lib/dotnet/host/fxr/9.0.7/libhostfxr.so`

### Graphics and UI Libraries
- `/usr/lib/libgdiplus.so.0.0.0` - **Critical: GDI+ for .NET graphics**
- `/usr/lib/x86_64-linux-gnu/libgtk-x11-2.0.so.0.2400.33` - **GTK2 (not GTK3)**
- `/usr/lib/x86_64-linux-gnu/libgdk-x11-2.0.so.0.2400.33`
- `/usr/lib/x86_64-linux-gnu/libgdk_pixbuf-2.0.so.0.4200.10`
- `/usr/lib/x86_64-linux-gnu/libatk-1.0.so.0.25209.1`
- `/usr/lib/x86_64-linux-gnu/libcairo.so.2.11800.0`
- `/usr/lib/x86_64-linux-gnu/libpango-1.0.so.0.5200.1`
- `/usr/lib/x86_64-linux-gnu/libpangocairo-1.0.so.0.5200.1`
- `/usr/lib/x86_64-linux-gnu/libpangoft2-1.0.so.0.5200.1`

### X11 and Display Libraries
- `/usr/lib/x86_64-linux-gnu/libX11.so.6.4.0`
- `/usr/lib/x86_64-linux-gnu/libXext.so.6.4.0`
- `/usr/lib/x86_64-linux-gnu/libXrender.so.1.3.0`
- `/usr/lib/x86_64-linux-gnu/libXrandr.so.2.2.0`
- `/usr/lib/x86_64-linux-gnu/libXi.so.6.1.0`
- `/usr/lib/x86_64-linux-gnu/libXinerama.so.1.0.0`
- `/usr/lib/x86_64-linux-gnu/libXfixes.so.3.1.0`
- `/usr/lib/x86_64-linux-gnu/libXdamage.so.1.1.0`
- `/usr/lib/x86_64-linux-gnu/libXcomposite.so.1.0.0`
- `/usr/lib/x86_64-linux-gnu/libXcursor.so.1.0.2`

### Font and Text Rendering
- `/usr/lib/x86_64-linux-gnu/libfreetype.so.6.20.1`
- `/usr/lib/x86_64-linux-gnu/libfontconfig.so.1.12.1`
- `/usr/lib/x86_64-linux-gnu/libharfbuzz.so.0.60830.0`
- `/usr/lib/x86_64-linux-gnu/libfribidi.so.0.4.0`
- `/usr/lib/x86_64-linux-gnu/libgraphite2.so.3.2.1`

### Image Format Support
- `/usr/lib/x86_64-linux-gnu/libjpeg.so.8.2.2`
- `/usr/lib/x86_64-linux-gnu/libpng16.so.16.43.0`
- `/usr/lib/x86_64-linux-gnu/libtiff.so.6.0.1`
- `/usr/lib/x86_64-linux-gnu/libwebp.so.7.1.8`
- `/usr/lib/x86_64-linux-gnu/libgif.so.7.2.0`
- `/usr/lib/x86_64-linux-gnu/libexif.so.12.3.4`

### Core System Libraries
- `/usr/lib/x86_64-linux-gnu/libglib-2.0.so.0.8000.0`
- `/usr/lib/x86_64-linux-gnu/libgobject-2.0.so.0.8000.0`
- `/usr/lib/x86_64-linux-gnu/libgio-2.0.so.0.8000.0`
- `/usr/lib/x86_64-linux-gnu/libc.so.6`
- `/usr/lib/x86_64-linux-gnu/libgcc_s.so.1`
- `/usr/lib/x86_64-linux-gnu/libstdc++.so.6.0.33`
- `/usr/lib/x86_64-linux-gnu/libm.so.6`

### Internationalization
- `/usr/lib/x86_64-linux-gnu/libicudata.so.74.2`
- `/usr/lib/x86_64-linux-gnu/libicui18n.so.74.2`
- `/usr/lib/x86_64-linux-gnu/libicuuc.so.74.2`

## Package Dependencies

### Debian/Ubuntu Packages
```
libgtk2.0-0, libglib2.0-0, libcairo2, libpango-1.0-0, libatk1.0-0, 
libgdk-pixbuf2.0-0, libgdiplus, libfreetype6, libfontconfig1, 
libx11-6, libxext6, libxrender1, libxrandr2, libxi6, libxinerama1, 
libxfixes3, libxdamage1, libxcomposite1, libxcursor1, libicu74, 
libjpeg8, libpng16-16, libtiff6, libwebp7, libgif7, libexif12, 
libc6, libgcc-s1, libstdc++6, git
```

### RHEL/CentOS/Fedora Packages
```
git, gtk2-devel, glib2, cairo, pango, atk, gdk-pixbuf2, libgdiplus, 
freetype, fontconfig, libX11, libXext, libXrender, libXrandr2, libXi, 
libXinerama, libXfixes, libXdamage, libXcomposite, libXcursor, libicu, 
libjpeg-turbo, libpng, libtiff, libwebp, giflib, libexif, glibc, 
libgcc, libstdc++
```

## Critical Notes

1. **GTK2 vs GTK3**: GitExtensions uses GTK2, not GTK3. This is important for packaging.

2. **libgdiplus**: This is a critical dependency for .NET graphics on Linux. It provides GDI+ compatibility.

3. **Mono Dependencies**: The application loads `libMonoPosixHelper.so` from the runtime directory, indicating some Mono compatibility layer usage.

4. **Version Sensitivity**: Some libraries like ICU (libicu74) are version-sensitive and may need adjustment for different distributions.

## Testing Dependencies

To verify all dependencies are available on a target system:

```bash
# Check if all required .so files can be found
ldd /path/to/GitExtensions.exe

# Or for the .NET application:
ldd $(which dotnet)
```

## Installation Commands

### Ubuntu/Debian
```bash
sudo apt-get update
sudo apt-get install libgtk2.0-0 libglib2.0-0 libcairo2 libpango-1.0-0 \
    libatk1.0-0 libgdk-pixbuf2.0-0 libgdiplus libfreetype6 libfontconfig1 \
    libx11-6 libxext6 libxrender1 libxrandr2 libxi6 libxinerama1 \
    libxfixes3 libxdamage1 libxcomposite1 libxcursor1 libicu74 \
    libjpeg8 libpng16-16 libtiff6 libwebp7 libgif7 libexif12 git
```

### RHEL/CentOS/Fedora
```bash
sudo dnf install git gtk2-devel glib2 cairo pango atk gdk-pixbuf2 \
    libgdiplus freetype fontconfig libX11 libXext libXrender libXrandr2 \
    libXi libXinerama libXfixes libXdamage libXcomposite libXcursor \
    libicu libjpeg-turbo libpng libtiff libwebp giflib libexif
```

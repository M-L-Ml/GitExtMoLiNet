# AI prompt task

Fix errors of compilation the GitUI project by implementing conditional compilation and/or stubs classes, or functions, or properties.

I need to fix compilation errors , caused by some Windows Forms library replacement - a new incomplate implementation of it. They are in  the GitUI and in the exrternals/WindowsAPICodePack/ folder. It sometimes caused by missing WPF dependencies. 
Please use this method whatever better to make simplier and preserve the much of the original text of the code:

1. Wrap the broken code with conditional compilation blocks
1.1 Use `#if WINDOWS_OWN  ... #endif` if it's related to missed Windows WPF or WinForms code
1.2 Use `#if FULLAPI ... #endif` if it missed WindowsAPICodePack code or you are not sure.
2. Create stub classes or methods for the non-FULLAPI path that:
   - Replace WPF-specific types (UIElement, Window, BitmapSource, Vector) with generic types (object)
   - Maintain the same method signatures and property names
   - Provide empty implementations 
   - throw  NotImplemented exceptions with an explanation.
  

The goal is to ensure the project compiles successfully while maintaining the API structure and leave   what is disabled as is , while it should be obvious it disable. No matter if some functionality is limited or broken when FULLAPI is not defined. No matter if it causes Null reference or NotImplemented exceptions.



# Conditional Compilation in GitExt

## Overview

This document explains the conditional compilation approach used in the GitExt project to handle missing dependencies and ensure successful compilation across different environments. By using conditional compilation and stub implementations, we can build the project even when certain dependencies (like WPF) are not available.

## The FULLAPI and WINDOWS_OWN Compilation Symbols

### Purpose

Two main compilation symbols are used in this project:

1. `FULLAPI` - Used to conditionally include or exclude code that depends on Windows API Code Pack functionality that may not be available in all build environments.

2. `WINDOWS_OWN` - Used specifically for Windows WPF or WinForms dependent code.

These symbols allow the codebase to compile successfully even when certain dependencies are missing, while preserving the original code structure.

### Usage

Code blocks wrapped with `#if FULLAPI` and `#endif` directives contain functionality that depends on external libraries (primarily WPF-related components) that might not be available in all build environments. When the `FULLAPI` symbol is not defined, these code blocks are excluded from compilation, and alternative implementations or empty stubs are used instead.

```csharp
#if FULLAPI
// Code that depends on external libraries (e.g., WPF)
public void SetImage(BitmapSource bitmapSource)
{
    // Implementation that uses WPF types
}
#else
// Alternative implementation or empty stub
public void SetImage(object bitmapSource)
{
    // Empty implementation or simplified version
}
#endif
```

## Stub Classes and Methods

For types that are referenced throughout the codebase but might not be available in all environments, we provide stub implementations when `FULLAPI` is not defined. These stubs ensure the code compiles but may not provide full functionality.

### Example: Stub Classes

```csharp
#if FULLAPI
// Real implementation using WPF types
public class TabbedThumbnailEventArgs : EventArgs
{
    public IntPtr WindowHandle { get; private set; }
    public UIElement WindowsControl { get; private set; }
    
    internal TabbedThumbnailEventArgs(UIElement windowsControl)
    {
        WindowsControl = windowsControl;
    }
}
#else
// Stub implementation without WPF dependencies
public class TabbedThumbnailEventArgs : EventArgs
{
    public IntPtr WindowHandle { get; private set; }
    public object WindowsControl { get; private set; }
    
    internal TabbedThumbnailEventArgs(object windowsControl)
    {
        WindowsControl = windowsControl;
    }
}
#endif
```

### Example: Method Implementation with Conditional Compilation

```csharp
private static void thumbnailPreview_TitleChanged(object sender, EventArgs e)
{
    var preview = sender as TabbedThumbnail;

    TaskbarWindow taskbarWindow = null;

    if (preview.WindowHandle == IntPtr.Zero)
    {
#if FULLAPI
        taskbarWindow = GetTaskbarWindow(preview.WindowsControl, TaskbarProxyWindowType.TabbedThumbnail);
#endif
    }
    else
    {
        taskbarWindow = GetTaskbarWindow(preview.WindowHandle, TaskbarProxyWindowType.TabbedThumbnail);
    }

    // Update the proxy window for the tabbed thumbnail
    if (taskbarWindow != null)
    {
#if FULLAPI
        taskbarWindow.SetTitle(preview.Title);
#endif
    }
}
```

### Example: Property Stubs

```csharp
#if FULLAPI
// Real property with WPF type
public Vector? PeekOffset { get; set; }
#else
// Stub property with generic object type
public object PeekOffset { get; set; }
#endif
```

### When to Use

Use the conditional compilation directives and stub implementations when:

1. Fixing code that depends on unavailable code
2. Creating empty method stubs to ensure successful compilation when certain dependencies are missing
3. Replacing specific types (like WPF's `UIElement` or `Vector`) with generic types (like `object`) in non-FULLAPI builds

Use `#if FULLAPI` for Windows API Code Pack dependencies and `#if WINDOWS_OWN` for Windows WPF or WinForms code.

### Recomendations 

1. **Preserve Original Code**: Wrap existing code in conditional compilation blocks rather than deleting or significantly modifying it
2. **Selective Enabling**: When possible, enable parts of disabled code by using nested conditional compilation directives
3. **Minimal Stubs**: For non-FULLAPI builds, provide minimal stub implementations that throw `NotImplementedException` with explanatory messages
4. **Type Replacement**: Replace WPF-specific types with generic types (object) in non-FULLAPI builds
5. **Maintain API Structure**: Keep method signatures and property names consistent between FULLAPI and non-FULLAPI implementations




## Other Recomendations

1. Provide appropriate empty stubs or alternative implementations for the non-FULLAPI path


2. Do not delete or significantly change the original code - just wrap it in conditional compilation blocks
3. Don't use nested  `#if ` directives
4. reEnable and fix parts of disabled code by using  `#if FULLAPI` or `#if WINDOWS_OWN` directives rather then create new stubs. 

## Runtime Considerations

This approach prioritizes successful compilation over runtime functionality. When running code built without the compilation symbols FULLAPI or WINDOWS_OWN:

- Features that depend on excluded code may throw exceptions if invoked (typically `NotImplementedException`)
- UI elements that depend on WPF will not function
- It's acceptable if the application throws exceptions at runtime when attempting to use disabled functionality

The primary goal is to ensure the project compiles successfully, even if some functionality is limited or broken when the compilation symbols are not defined.

# AI prompt task

Fix errors of compilation the GitUI project by implementing conditional compilation and/or stubs classes, or functions, or properties.

I need to fix compilation errors , caused by some Windows Forms library replacement - a new incomplate implementation of it. They are in  the GitUI and in the exrternals/WindowsAPICodePack/ folder. It sometimes caused by missing WPF dependencies. 
Please use this method whatever better to make simplier and preserve the much of the original text of the code:

1. Wrap the broken code with conditional compilation blocks
1.1 Use `#if WINDOWS_OWN` if it's related to missed Windows WPF or WinForms code
1.2 Use `#if FULLAPI ... #endif` if it missed WindowsAPICodePack code or you are not sure.
2. Create stub classes or methods for the non-FULLAPI path that:
   - Replace WPF-specific types (UIElement, Window, BitmapSource, Vector) with generic types (object)
   - Maintain the same method signatures and property names
   - Provide empty implementations 
    -  throw  NotImplemented exceptions with an explanation.
  

The goal is to ensure the project compiles successfully while maintaining the API structure and leave   what is disabled as is , while it should be obvious it disable. No matter if some functionality is limited or broken when FULLAPI is not defined. No matter if it causes Null reference or NotImplemented exceptions.


# One proposed commit message

Fix compilation errors in TabbedThumbnail.cs with conditional compilation

- Wrapped WPF-dependent code with #if FULLAPI ... #endif blocks
- Added empty stubs for WPF-specific types when FULLAPI is not defined
- Fixed XML documentation to match conditional compilation structure
- Ensured GitUI project compiles successfully without WPF dependencies

This change allows the project to be built in environments where
WPF dependencies are not available by conditionally excluding
WPF-specific functionality.

# Conditional Compilation in GitExt

## Overview

This document explains the conditional compilation approach used in the GitExt project to handle missing dependencies and ensure successful compilation across different environments. By using conditional compilation and stub implementations, we can build the project even when certain dependencies (like WPF) are not available.

## The FULLAPI Compilation Symbol

### Purpose

The `FULLAPI` compilation symbol is used to conditionally include or exclude code that depends on specific libraries or frameworks that may not be available in all build environments. This approach allows the codebase to compile successfully even when certain dependencies are missing.

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

Use the `FULLAPI` conditional compilation directive and stub implementations when:

1. Implementing code that depends on libraries that might not be available in all build environments
2. Providing alternative implementations for different target frameworks
3. Creating empty method stubs to ensure successful compilation when certain dependencies are missing
4. Replacing specific types (like WPF's `UIElement` or `Vector`) with generic types (like `object`) in non-FULLAPI builds

## Building Options

### Building with FULLAPI

To build the project with all features enabled, define the `FULLAPI` symbol in your build configuration:
- In Visual Studio: Project Properties > Build > Conditional compilation symbols > Add "FULLAPI"
- With MSBuild: `/p:DefineConstants=FULLAPI`
- With dotnet CLI: `dotnet build -p:DefineConstants=FULLAPI`

### Building without FULLAPI

When building without the `FULLAPI` symbol defined, the code will compile with reduced functionality, excluding features that depend on potentially missing libraries. This is useful for:
- Environments where WPF dependencies are not available
- Simplified builds that don't require full UI functionality
- Testing core functionality without UI dependencies

## Maintenance Notes

When modifying code that uses conditional compilation:

1. Ensure that both code paths (with and without `FULLAPI`) compile successfully
2. Provide appropriate empty stubs or alternative implementations for the non-FULLAPI path
3. Document any significant functionality differences between the two paths
4. Consider the impact on runtime behavior when the full implementation is not available
5. When adding new WPF-dependent code, always wrap it with `#if FULLAPI` blocks
6. For public APIs, provide stub implementations that maintain the same signature but with generic types

## Runtime Considerations

This approach prioritizes successful compilation over runtime functionality. When running code built without the `FULLAPI` symbol:

- Features that depend on excluded code may throw exceptions if invoked
- UI elements that depend on WPF will not function
- Applications should check for feature availability before using conditionally compiled features

Consider adding runtime checks to prevent exceptions when attempting to use features that require the full API implementation.

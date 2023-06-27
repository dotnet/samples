`ComWrappers` API Demo
================

The [`ComWrappers`](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.comwrappers) API was introduced in .NET 5.0 to help users build custom COM interop scenarios.

Key Features
------------

Demonstrates how to implement and utilize the `ComWrappers` API by manually implementing a subset of [`IDispatch`](https://docs.microsoft.com/windows/win32/api/oaidl/nn-oaidl-idispatch) for .NET objects. The consumer of the `IDispatch` instance is the [Windows Forms `WebBrowser`](https://docs.microsoft.com/dotnet/framework/winforms/controls/webbrowser-control-windows-forms) API for [exposing an object to the JavaScript engine](https://docs.microsoft.com/dotnet/api/system.windows.forms.webbrowser.objectforscripting).

This is already fully supported by the built-in COM interop system, but this demonstrates a way for users to provide their own implementation.

**Note** There are two sections of code commented as `WORKAROUND`. These sections are needed to trick the `WebBrowser.ObjectForScripting` API into accepting a .NET object but receiving a `ComWrappers` generated `IDispatch` implementation instead.

Build and Run
-------------

1) Install .NET 5.0 or later.

1) Load `ComWrappersIDispatch.csproj` in Visual Studio 2019 or build from the command line.
    - Double click on `ComWrappersIDispatch.csproj` in File Explorer.

    or

    - Open a Command prompt with `dotnet` on the path and build `dotnet build ComWrappersIDispatch.csproj`.

1) Press F5 to build and debug the project or `dotnet run ComWrappersIDispatch.csproj`

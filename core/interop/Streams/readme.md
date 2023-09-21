# Interop with .NET Stream sample

The [`Stream`](https://learn.microsoft.com/dotnet/api/system.io.stream) data type is one of the most common non-trivial data types for interop. This sample provides two distinct mechanisms for working with the `Stream` type in interop scenarios.

1) The projection of the `Stream` type using C-style exports.

1) The projection of the `Stream` type using the COM [`IStream`](https://learn.microsoft.com/windows/win32/api/objidl/nn-objidl-istream) interface. This approach utilizes [`ComWrappers`](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.comwrappers) and the [COM source generator](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.marshalling.generatedcominterfaceattribute).

The [`ManagedLib`](./ManagedLib/) represents a .NET class library that is exposing a `Stream` type to an unmanaged process. This project demonstrates both the latest .NET approach and how it could be done using .NET Framework.

The [`Runner`](./Runner/) is a placeholder for an unmanaged process. The code within [`Program.cs`](./Runner/Program.cs) is written in relatively low-level C#, but represents basic concepts that translate directly into C/C++.

## Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or a later version

## Build and Run

1. Navigate to this directory.

1. Build the sample by executing `dotnet build`.

1. Run the sample by executing `dotnet run`.

The expected output:

```
--- Stream via C export style interop
1 2 3 4
5 6 7 8
9

--- Stream via IStream style interop
9 8 7 6
5 4 3 2
1
```

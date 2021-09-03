---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "ComWrappers API Tutorial Sample"
urlFragment: "comwrappers-api-tutorial"
description: "A .NET 5 sample that demonstrates using the ComWrappers API to define native and managed object wrappers."
---

# `ComWrappers` API Tutorial Sample

The [`ComWrappers`](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.comwrappers) API was introduced in .NET 5.0 to help users build custom COM interop scenarios.

## Key Features

- Statically and dynamically defined Native Object Wrappers.
- Complete Managed Object Wrapper example.
- Accompanying [walkthrough](https://docs.microsoft.com/dotnet/standard/native-interop/tutorial-comwrappers) article.

## Build and Run-------------

1) Install .NET 5.0 or later.

1) Load `Tutorial.csproj` in Visual Studio 2019 or build from the command line.
    - Double click on `Tutorial.csproj` in File Explorer.

    or

    - Open a Command prompt with `dotnet` on the path and build `dotnet build Tutorial.csproj`.

1) Press F5 to build and debug the project or `dotnet run Tutorial.csproj`

---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Assembly Loading Extension Points"
urlFragment: "assembly-loading-extension-points"
description: "A .NET application that demonstrates extension points available for affecting managed assembly loading."
---

# Assembly Loading Extension Points Demo

This demo uses the extensions points available for affecting managed assembly loading in .NET and shows when they are called by the runtime. The application (`AssemblyLoading`) loads:

- an assembly that is part of the `Microsoft.NETCore.App` framework and thus one of the known application assemblies
- an assembly (`MyLibrary`) that is not referenced by the application and thus requires handling in one of the available extension points to be successfully loaded

The following hooks into the [managed assembly loading algorithm](https://docs.microsoft.com/dotnet/core/dependency-loading/loading-managed#algorithm) are included in this demo:

- [`AssemblyLoadContext.Load`](https://docs.microsoft.com/dotnet/api/system.runtime.loader.assemblyloadcontext.load)
- [`AssemblyLoadContext.Resolving`](https://docs.microsoft.com/dotnet/api/system.runtime.loader.assemblyloadcontext.resolving)
- [`AppDomain.AssemblyResolve`](https://docs.microsoft.com/dotnet/api/system.appdomain.assemblyresolve)

## Build and Run

1) Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later

1) Build the solution with `dotnet build`

1) Run the generated binary - for example, `AssemblyLoading/bin/Debug/net5.0/AssemblyLoading`
   - The program will print information about its usage syntax
      - `AssemblyLoading <context> [<extension-point>]`
   - Example: `AssemblyLoading default`
      - The default `AssemblyLoadContext` is used. The extension points are implemented, but none of them load the `MyLibrary` assembly. The expected output will show each extension point being called:

         ```
         === Loading 'System.Xml' ===

         Successfully loaded assembly:
         - Assembly: System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
         - Load context: "Default" System.Runtime.Loader.DefaultAssemblyLoadContext #0

         === Loading 'MyLibrary' ===

         AssemblyLoadContext.Resolving event handler
         - Name: MyLibrary, Culture=neutral, PublicKeyToken=null;
         - Load context: "Default" System.Runtime.Loader.DefaultAssemblyLoadContext #0

         AppDomain.AssemblyResolve event handler
         - Name: MyLibrary, Culture=neutral, PublicKeyToken=null

         System.IO.FileNotFoundException: Could not load file or assembly 'MyLibrary, Culture=neutral, PublicKeyToken=null'. The system cannot find the file specified.
         File name: 'MyLibrary, Culture=neutral, PublicKeyToken=null'
         ```

   - Example: `AssemblyLoading custom alc-resolving`
      - A new custom `AssemblyLoadContext` is used. The extension points are implemented and the handler for the `Resolving` event on the custom `AssemblyLoadContext` loads the `MyLibrary` assembly. The expected output will show each extension point being called:

         ```
         === Loading 'System.Xml' ===

         AssemblyLoadContext.Load
         - Name: System.Xml, Culture=neutral, PublicKeyToken=null
         - Load context: "CustomALC" AssemblyLoading.Program+CustomALC #0

         Successfully loaded assembly:
         - Assembly: System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
         - Load context: "Default" System.Runtime.Loader.DefaultAssemblyLoadContext #1

         === Loading 'MyLibrary' ===

         AssemblyLoadContext.Load
         - Name: MyLibrary, Culture=neutral, PublicKeyToken=null
         - Load context: "CustomALC" AssemblyLoading.Program+CustomALC #0

         AssemblyLoadContext.Resolving event handler
         - Name: MyLibrary, Culture=neutral, PublicKeyToken=null;
         - Load context: "Default" System.Runtime.Loader.DefaultAssemblyLoadContext #1

         AssemblyLoadContext.Resolving event handler
         - Name: MyLibrary, Culture=neutral, PublicKeyToken=null;
         - Load context: "CustomALC" AssemblyLoading.Program+CustomALC #0

         Successfully loaded assembly:
         - Assembly: MyLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
         - Load context: "CustomALC" AssemblyLoading.Program+CustomALC #0
         ```

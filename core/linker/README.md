# IL Linker Samples

These samples demonstrate how to use the new ILLink functionality in the .NET Core 3.0 SDK to remove unused IL from your published builds and shrink your application size. This folder includes two samples to demonstrate various scenarios.

## HelloWorldTrimmed Sample

The `HelloWorldTrimmed` sample demonstrates the default behavior of the .NET Core 3.0 SDK with IL Link trimming enabled. For most applications, the HelloWorldTrimmed should work as expected.

The default settings for trimming automatically "root" all application assemblies. "Rooting" an assembly means that none of the IL in that assembly or any IL code that the assembly uses directly will be trimmed. Framework assemblies are not rooted, however, so it is possible to have issues in a trimmed application when using reflection to call types built into the .NET Core framework. Any assembly in the Microsoft.NETCore.App framework is considered a "framework" assembly, but Windows Forms, WPF, and ASP.NET frameworks are considered application assemblies for the purposes of linking.

## TrimmedWithAdditionalRoots Sample

Sometimes a project needs to use reflection to call a method in .NET Core. If that method is in the Microsoft.NETCore.App framework and it is only used via reflection, then the linker will trim out the method because it assumes that the method is unused. The `TrimmedWithAdditionalRoots` sample shows a simple example of rooting a framework assembly so that reflection-based usage of it will succeed.

## Build and Run

To build each sample, run `dotnet publish -r <rid>` where `<rid>` is a Runtime Identifier such as `win-x64`. To run each sample, go to the `bin/Debug/netcoreapp3.0/<rid>/publish/` folder under the project folder after building and run the executable file that has the name of the sample.

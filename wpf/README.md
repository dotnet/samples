# .NET Core 3.0 WPF Samples

Beginning with [.NET Core 3.0](https://github.com/dotnet/core-sdk#installers-and-binaries), you can build WPF applications.

## Why build WPF applications on top of .NET Core

If you're new to .NET Core, here are a few resources to help you understand the advantages of .NET Core for building Windows applications:

* [Blog: .NET Core 3 and Support for Windows Desktop Applications](https://devblogs.microsoft.com/dotnet/net-core-3-and-support-for-windows-desktop-applications/)
* [Video: Modernizing Desktop Apps on Windows 10 with .NET Core 3.0 and much more](https://channel9.msdn.com/events/Build/2018/BRK3501?term=scott%20hunter&pubDate=year&lang-en=true)

## Quality disclaimer

.NET Core 3 support for desktop development is still in preview. You will likely encounter missing tools, bugs, and unexpected behavior. We do not recommend using this SDK and tools for building applications for production scenarios. We do recommend using this SDK and tools to evaluate how easy it will be to migrate your existing applications, or if you're just interested in trying out the latest upcoming Windows development technology.

## Samples in this repo

Coming soon!

## Additional Samples

See [WPF Samples](https://www.github.com/Microsoft/wpf-samples) repo for additional WPF samples that have been updated to target .NET Core 3.0.

## Getting Started

### Prerequisites and getting the tools

Install Visual Studio 2019 preview from [https://visualstudio.microsoft.com/vs/preview](https://visualstudio.microsoft.com/vs/preview), selecting the **.NET desktop development** workload with the options: **.NET Framework 4.7.2 development tools** and **.NET Core 2.2 development tools**.

Install either the [preview build](https://dotnet.microsoft.com/download/dotnet-core/3.0) of the .NET Core 3.0 SDK or the latest daily build available in the [dotnet/code-sdk repo](https://github.com/dotnet/core-sdk).

### Analyzing your applications for .NET Core 3.0 readiness

If you want to first understand your existing applications readiness for targeting .NET Core 3.0, you can run the .NET Portability Analyzer using the instructions [here](https://devblogs.microsoft.com/dotnet/are-your-windows-forms-and-wpf-applications-ready-for-net-core-3-0/). This will produce a report that will show you API compatibility for each assembly that your application depends on.

### Creating new .NET Core 3.0 WPF applications

To create a new application you can use the `dotnet new` command, using the new templates for WPF.

Using the developer command prompt installed with Visual Studio, run:

```cmd
dotnet new wpf -o MyWPFApp
cd MyWPFApp
dotnet run
```

## Porting existing applications

>We recommend running the [APIPort tool](https://github.com/Microsoft/dotnet-apiport-ui/releases) first to determine if there are any APIs your application depends on that will be missing with .NET Core.

There is no tooling available to help with project migration. In order to migrate your WPF application, you will create a new project and manually port all of the elements defined in your original project. You will notice the new project is based on the simplified project format, and not everything will be migrated.

### Migrate the head project

Ideally you should migrate all projects in your solution to target .NET Core 3.0 and/or .NET Standard 2.0. The first step to migrate will be to retarget the application's entry point (i.e. 'head' project) and maintain your existing references.

1. Start from a working Solution. You must be able to open the solution in Visual Studio and double check that you can build and run without any issues.
1. If your solution also has server side projects, such as ASP.NET, we recommend splitting your solution into different server and client solutions. For this effort, work with the client solution only.
1. Add a new .NET Core 3.0 WPF project to the solution. Adding this project to a sibling folder to your existing 'head' project will make it easier to port references later (using relative paths to other projects or assemblies in the solution)
1. If your 'head' project uses NuGet packages, you must add the same NuGet packages to the new project. The new SDK-Style projects only support the PackageReference format for adding NuGet package references. If your existing project is using `packages.config`, you must migrate to the new format. You can use the Migrator Tool described [here](https://docs.microsoft.com/nuget/reference/migrate-packages-config-to-package-reference) to automate this process.
1. Copy the `PackageReference` elements generated in the previous step from the original project into the new project's .csproj file.
1. Copy the `ProjectReference` elements from the original project. Note: The new project format does not use the `Name` and `ProjectGuid` elements so you can safely delete those.
1. At this point it's a good idea to try and restore/build to make sure all dependencies are properly configured.
1. [Link the files](#link-files-from-the-old-project) from your existing .NET Framework WPF project to the .NET Core 3.0 WPF project.
1. **Optional** If you have difficulties with compiler linking, you can copy the project files from the .NET Framework WPF project to the new .NET Core 3.0 WPF project.

    * C# files (files with the `.cs.` extension) are included by default in the .csproj.
    * Other project elements like `EmbeddedResources` can also use globbing.
    * XAML files need to be included using the `<Page />` element. Remember that globbing is allowed, so you can add all XAML files from a given folder with a single `<Page Include=Views\*.xaml />` element.

### Migration tips

**Configure Assembly File generation**

Most existing projects include an `AssemblyInfo.cs` file in the Properties folder. The new project style uses a different approach and generate the same assembly attributes as part of the build process. To disable that behavior you can add the property:

```xml
<GenerateAssemblyInfo>false</GenerateAssemblyInfo>
```

**Include the Windows.Compatibility Pack**

Not every framework assembly is available in the .NET Core base class library. Windows applications like Windows Forms and WPF could have dependencies that are not available in .NET Core or .NET Standard. Adding a reference to the [Windows Compatibility Pack](https://docs.microsoft.com/dotnet/core/porting/windows-compat-pack) will help reduce missing assembly dependencies as it includes several types that might be needed by your application.

```cmd
dotnet add package Microsoft.Windows.Compatibility
```

**Link Files from the old Project**

Visual Studio does not yet support designers and custom tools for .NET desktop development. You can keep your files in the original project and link the generated files to the new project by using the link attribute in the project elements, e.g. `<Compile Link="" />`. See the [sample](HelloWorld-WithLinkedFiles) in this repo for an example of this.

**Migrating WCF Clients**

.NET Core has its own implementation of `System.ServiceModel` with some differences:

* It's available as NuGet packages (also included in the Windows Compatibility Pack).
* Review if your application uses some of the [unsupported features](https://github.com/dotnet/wcf/blob/main/release-notes/SupportedFeatures-v2.1.0.md).
* If you want to reuse the ServiceReference created by Visual Studio you might get the error `System.PlatformNotSupportedException: 'Configuration files are not supported.'`. This error requires a code change to specify the binding and endpoint address in the service client constructor.

## Filing Issues and Getting Help

You can file Windows Forms and WPF related issues in the [dotnet/core repo](https://github.com/dotnet/core/issues). If you are trying out WPF or Windows Forms development on top of .NET Core 3.0 and get stuck or have questions, please reach out to <netcore3modernize@microsoft.com>.

### Known Issues

Take a look at the issues filed with the [WPF area tag](https://github.com/dotnet/core/labels/area-wpf).

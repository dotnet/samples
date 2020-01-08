# WPF-WinRT

This sample shows how to consume the Windows 10 API from .NET Core and .Net Framework projects.

## Pre-requisites

To access Win10 APIs you need to have the Windows SDK installed. You can add it from the Visual Studio setup or download from [here](https://developer.microsoft.com/windows/downloads/windows-10-sdk)

## Configure the reference in your project

You can reference the winmd by using the `Add Reference` dialog or adding the following to your `.csproj` file:

```xml
<ItemGroup>
  <Reference Include="Windows">
    <HintPath>$(MSBuildProgramFiles32)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd</HintPath>
    <Private>False</Private>
  </Reference>
</ItemGroup>
```

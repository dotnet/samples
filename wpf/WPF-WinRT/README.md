# WPF-WinRT

This sample shows how to consume the Windows 10 API from .NET Core and .Net Framework projects.

Windows 10 SDK is required.

```xml
  <ItemGroup>
    <Reference Include="Windows">
      <HintPath>$(MSBuildProgramFiles32)\Windows Kits\10\UnionMetadata\10.0.16299.0\Windows.winmd</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>
```

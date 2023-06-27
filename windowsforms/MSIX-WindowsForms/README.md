# MSIX Windows Forms Core Application

This sample shows how to use the Windows Packaging Project to package a Windows Forms application running on .NET Core 3.1.

## Pre-requisites

To use the Windows Packaging Project (wapproj) you need to install the Universal Windows workload in Visual Studio.

To produce `MSIX` packages, you must have the **Windows 10 October 2018 SDK** (aka 10.0.17763): otherwise, the generated package will have the `APPX` extension.

To install MSIX packages you need the **Windows 10 October 2018 Update**

## Customize the Packaging Project

Currently, the packaging project does not support .NET Core applications. As a workaround, we updated the project files to call the `publish` target to produce a self-contained app, and to fix the manifest entry point.

In the .NET Core csproj file add:

```xml
<Target Name="__GetPublishItems" DependsOnTargets="ComputeFilesToPublish" Returns="@(_PublishItem)">
  <ItemGroup>
    <_PublishItem Include="@(ResolvedFileToPublish->'%(FullPath)')" TargetPath="%(ResolvedFileToPublish.RelativePath)" OutputGroup="__GetPublishItems" />
    <_PublishItem Include="$(ProjectDepsFilePath)" TargetPath="$(ProjectDepsFileName)" />
    <_PublishItem Include="$(ProjectRuntimeConfigFilePath)" TargetPath="$(ProjectRuntimeConfigFileName)" />
  </ItemGroup>
</Target>
```

In the .wapproj file modify the `ProjectReference` element:

```xml
<ItemGroup>
  <ProjectReference Include="..\CoreWinFormsApp1\CoreWinFormsApp1.csproj" SkipGetTargetFrameworkProperties="true" Properties="RuntimeIdentifier=win-x64;SelfContained=true" />
</ItemGroup>
```

And add the targets:

```xml
<PropertyGroup>
  <PackageOutputGroups>@(PackageOutputGroups);__GetPublishItems</PackageOutputGroups>
</PropertyGroup>

<Target Name="_ValidateAppReferenceItems" />
<Target Name="_FixEntryPoint" AfterTargets="_ConvertItems">
  <PropertyGroup>
    <EntryPointExe>CoreWinFormsApp1\CoreWinFormsApp1.exe</EntryPointExe>
  </PropertyGroup>
</Target>
<Target Name="PublishReferences" BeforeTargets="ExpandProjectReferences">
  <MSBuild Projects="@(ProjectReference->'%(FullPath)')" BuildInParallel="$(BuildInParallel)" Targets="Publish" />
</Target>
```

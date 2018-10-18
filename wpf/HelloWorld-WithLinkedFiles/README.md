# WPF Hello World sample with linked files

This sample shows how to use linked files to share the source code of a WPF application based on .NET 
Framework with a .NET Core 3.0 project.

To share source code you can use the `Link` attribute in most of the project elements:

```xml
<ItemGroup>
  <ApplicationDefinition Include="..\HelloWorldNetFx\App.xaml" Link="App.xaml" />
  <Compile Include="..\HelloWorldNetFx\App.xaml.cs" Link="App.xaml.cs" />
</ItemGroup>

<ItemGroup>
  <Compile Include="..\HelloWorldNetFx\**\*.cs" />
  <Page Include="..\HelloWorldNetFx\**\*.xaml" Exclude="@(ApplicationDefinition)" />      
</ItemGroup>
```

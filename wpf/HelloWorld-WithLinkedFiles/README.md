# WPF Hello World sample with linked files

This sample shows how to use linked files to share the source code of a WPF application based on .NET
Framework with a .NET 6.0 project.

To share source code, you can use the `Link` attribute in most of the project elements:

```xml
  <ItemGroup>
    <ApplicationDefinition Include="..\HelloWorldNetFx\App.xaml" Link="App.xaml" />
    <Compile Include="..\HelloWorldNetFx\App.xaml.cs" Link="App.xaml.cs" />
  </ItemGroup>

  <ItemGroup>
    <Page Include="..\HelloWorldNetFx\MainWindow.xaml" Link="MainWindow.xaml" />
    <Compile Include="..\HelloWorldNetFx\MainWindow.xaml.cs" Link="MainWindow.xaml.cs" />
  </ItemGroup>
```

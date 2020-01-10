# Hello-World with Links

This sample shows how to share code between a .NET Core 3.0 and .NET Framework project. This approach allows you to reuse the same code files. This is important because it allows you to use the designers from the .NET Framework project and reuse them in  .NET Core.

To link files between two projects, this sample uses the the `Link` attribute.

```xml
<ItemGroup>
  <Compile Include="..\FullFxApp\*.cs" />
</ItemGroup>
```

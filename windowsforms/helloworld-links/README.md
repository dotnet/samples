# Hello-World with Links

This sample shows how to share code between a .NET 6.0 and .NET Framework 4.8 project. This approach allows you to reuse the same code files. This is important because it allows you to use the designers from the .NET Framework project and reuse them in .NET.

To link files between two projects, this sample uses the the `Link` attribute.

```xml
<ItemGroup>
  <Compile Include="..\FullFxApp\*.cs" />
</ItemGroup>
```

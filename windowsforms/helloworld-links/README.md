# Hello-World with Links

This sample shows how to share code between a NetCore3 and NetFramework projects. This approach allows to reuse the same code files, this is important because allows to use the designers from the NetFramework project and reuse it from NetCore.

To link files between two project this sample uses the the `Link` attribute.

```xml
<ItemGroup>
    <Compile Include="..\FullFxApp\*.cs" />
  </ItemGroup>
```


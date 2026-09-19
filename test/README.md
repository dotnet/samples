# .NET testing samples

This directory contains minimal examples that demonstrate how to use common .NET testing frameworks with different test platforms.

## MSTest

The MSTest samples demonstrate the same test using two different test platforms:

- [VSTest](mstest/vstest)
- [Microsoft.Testing.Platform](mstest/microsoft-testing-platform)

### VSTest

The VSTest sample uses MSTest with the VSTest platform.

To run the sample from this directory:

```bash
dotnet test mstest/vstest/MSTestVSTest.csproj
```

### Microsoft.Testing.Platform

The Microsoft.Testing.Platform sample uses `MSTest.Sdk` and the MSTest runner based on Microsoft.Testing.Platform.

To run the sample from this directory:

```bash
dotnet test mstest/microsoft-testing-platform/MSTestMTP.csproj
```

Both samples contain the same minimal test so that the differences between the test platform configurations are easy to compare.

Additional testing framework samples can be added in separate directories.

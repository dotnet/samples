# MSBuild examples

This folder includes MSBuild examples:

1. The first example is the creation of a _MSBuild Custom Task_ for code generation. The idea is to consume a txt file and generate code from it during the build process. It is simple in order to show the mechanism, then you will be able to create a more complex piece of code. Part of this effort includes how to ship and consume the MSBuild Custom Task as a _NuGet package_.

   [Please see the Custom Task-Code Generation Readme](./custom-task-code-generation/)

1. Generate a Rest Client API during the build process. The example uses [NSwag](https://docs.microsoft.com/aspnet/core/tutorials/getting-started-with-nswag) as a client generator (It is also a code generation example). It is a very common scenario. We are going to create two examples

   1. Use the pre-defined [MSBuild Exec Task](https://docs.microsoft.com/en-us/dotnet/api/microsoft.build.tasks.exec) to do that.
   1. Use the _MSBuild Custom Task_ derived from [MSBuild Tool Task](https://docs.microsoft.com/dotnet/api/microsoft.build.utilities.tooltask) to do that.

   [Please see the Rest-Api client Generation Readme](./rest-api-client-generation/)

1. [How to test a MSBuild Custom Task](./Test.README.md)

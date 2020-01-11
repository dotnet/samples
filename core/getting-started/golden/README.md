# Getting Started using Visual Studio Code Sample

This sample is part of the [Tutorial: Create a .NET Core solution in macOS using Visual Studio Code](https://docs.microsoft.com/dotnet/core/tutorials/using-on-macos)
walkthrough. Please see that topic for detailed steps on the code
for this sample.

## Key Features

This sample builds a program and an associated unit test assembly. You'll learn how to structure
projects as part of a larger solution, and incorporate unit tests into your projects.

## Build and Run

To build and run the sample, change to the `src/library` directory and
type the following two commands:

```
dotnet restore
dotnet build
```

`dotnet restore` ([see note](#dotnet-restore-note)) installs all the dependencies for this sample into the current directory.
`dotnet build` creates the output assembly (or assemblies).

Next, change to the `app` directory and run those same
two commands again.

After that, type this command:

`dotnet run`

`dotnet run` runs the output executable.

To run the tests, change to the `test-library` directory and
type the following three commands:

```
dotnet restore
dotnet build
dotnet test
```

`dotnet test` runs all the configure tests.

Note that you must run `dotnet restore` ([see note](#dotnet-restore-note)) in the `src/library` directory before you can run
the tests. `dotnet build` will follow the dependency and build both the library and unit
tests projects, but it will not restore NuGet packages.

This sample focuses on setting up and using [Visual Studio Code](https://code.visualstudio.com)
as your development editor. The topic walks through all the setup and configuration steps for
that environment.

<a name="dotnet-restore-note"></a>
**Note:** Starting with .NET Core 2.0 SDK, you don't have to run [`dotnet restore`](https://docs.microsoft.com/dotnet/core/tools/dotnet-restore) because it's run implicitly by all commands that require a restore to occur, such as `dotnet new`, `dotnet build` and `dotnet run`. It's still a valid command in certain scenarios where doing an explicit restore makes sense, such as [continuous integration builds in Azure DevOps Services](https://docs.microsoft.com/azure/devops/build-release/apps/aspnet/build-aspnet-core) or in build systems that need to explicitly control the time at which the restore occurs.

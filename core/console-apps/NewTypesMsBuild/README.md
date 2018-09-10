# NewTypes Pets Sample

This sample is part of the [Organizing and testing projects with the .NET Core command line tutorial](https://docs.microsoft.com/dotnet/core/tutorials/testing-with-cli) for creating .NET Core console applications. See the tutorial for details on the code for this sample.

## Key Features

This sample builds a program and an associated unit test assembly. Using this sample, you learn how to structure projects as part of a larger solution and incorporate unit tests into your projects.

## Build and run

To build and run the sample, change to the *src/NewTypes* directory and execute the following command:

```console
dotnet run
```

`dotnet restore` ([see note](#dotnet-restore-note)) restores the dependencies of the sample. `dotnet run` builds the sample and runs the output executable. It implicitly runs `dotnet restore` to restore the dependencies of the sample. If you're using .NET Core 1.0 or .NET Core 1.1 instead of .NET Core 2.0 or a later version, you have to run `dotnet restore` yourself.

To run the tests, change to the *test/NewTypesTests* directory and execute the following two commands:

```console
dotnet build
dotnet test
```

`dotnet test` runs the configured tests.

`dotnet build` will follow the dependency on the `NewTypesMsBuild` project and build both the app and unit tests projects. It implicitly runs `dotnet restore` on .NET Core 2.0 and later versions. If you're using .NET Core 1.0 or .NET Core 1.1, you first have to run `dotnet restore` yourself. 


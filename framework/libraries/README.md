# .NET Core Library Samples

These samples are buildable projects whose source is used for code snippets in [the guide for writing cross-platform libraries](https://docs.microsoft.com/dotnet/core/tutorials/libraries).  They can be built and run using the .NET Core toolchain, and are intended to simply demonstrate how to target and build NuGet packages for different targets.  They aren't examples of how you'd build a real, feature-complete library.

To build/use any of these (using `frameworks-library` as an example):

1. Open your favorite Command Line Interface (for example, Cmd.exe or Terminal).

2. Navigate to the top-level directory:

    `cd frameworks-library`

3. Restore packages by typing the following:

    `dotnet restore`

    **Note:** Starting with .NET Core 2.0 SDK, you don't have to run [`dotnet restore`](https://docs.microsoft.com/dotnet/core/tools/dotnet-restore) because it's run implicitly by all commands that require a restore to occur, such as `dotnet new`, `dotnet build` and `dotnet run`. It's still a valid command in certain scenarios where doing an explicit restore makes sense, such as [continuous integration builds in Azure DevOps Services](https://docs.microsoft.com/azure/devops/build-release/apps/aspnet/build-aspnet-core) or in build systems that need to explicitly control the time at which the restore occurs.

4. To build and package the library as a NuGet package, type the following:

    ```
    cd src/Library
    dotnet build
    dotnet pack
    ```

    Check out the `/bin/Debug` directory to see the generated artifacts and `.nupkg`.

5. To run unit tests (if applicable):

    ```
    cd ../../test/LibraryTests
    dotnet test
    ```

And that's it!

## frameworks-library

**IMPORTANT:** This project requires Windows and the .NET Framework installed on your machine.

The project under `/frameworks-library` demonstrates how to use the CLI tools to build a library that targets the .NET Framework.  It does so with a simple project targeting the .NET Framework 4.0.  You could extend this to target additional versions of the .NET Framework by adding new build targets in the `library.csproj` project file.  Check out the [section on cross-compiling](https://docs.microsoft.com/dotnet/core/tutorials/libraries#how-to-multitarget) in the CLI libraries article for more information.

## net45-compat-library

**IMPORTANT:** This project requires Windows and the .NET Framework installed on your machine.

The project under `/net45-compat-library` targets any of the following:

* .NET Framework 4.5.1 and above
* Windows Phone 8.1
* Universal Windows Platform
* Xamarin
* Mono

It uses the `netstandard1.2` Target Framework Moniker introduced with the [.NET Standard](https://docs.microsoft.com/dotnet/standard/library).

## net40-library

**IMPORTANT:** This project requires Windows and the .NET Framework installed on your machine.

The project under `/net40-library` targets the .NET Framework 4.0 and above.  It also demonstrates how to use [#if](https://docs.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-if) directives to multi-target for a .NET 4.0 target.

## pcl-library

**IMPORTANT:** This project requires Windows and the .NET Framework installed on your machine.

The project under `/pcl-library` shows how to target a supported PCL Profile (for example, 344).  It shows how to structure the `Library.csproj` file to allow for targeting a PCL.  It also demonstrates how to use [#if](https://docs.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-if) directives and how to define a preprocessor constant, `PORTABLE259` in the `Library.csproj` file.

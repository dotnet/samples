# Age Calculator Sample

This is a beginner-friendly .NET 8 console app that calculates a user's age and days until their next birthday based on their date of birth input.

## Key Features

- Reads user input for date of birth
- Calculates current age
- Displays days until next birthday
- Includes basic error handling for invalid input

## Build and Run

To build and run the sample, type the following command:

`dotnet run`

`dotnet run` builds the sample and runs the output assembly. It implicitly calls `dotnet restore` on .NET Core 2.0 and later versions. If you're using a .NET Core 1.x SDK, you first have to call `dotnet restore` yourself.

**Note:** Starting with .NET Core 2.0 SDK, you don't have to run [`dotnet restore`](https://docs.microsoft.com/dotnet/core/tools/dotnet-restore) because it's run implicitly by all commands that require a restore to occur, such as `dotnet new`, `dotnet build` and `dotnet run`. It's still a valid command in certain scenarios where doing an explicit restore makes sense, such as [continuous integration builds in Azure DevOps Services](https://docs.microsoft.com/azure/devops/build-release/apps/aspnet/build-aspnet-core) or in build systems that need to explicitly control the time at which the restore occurs.

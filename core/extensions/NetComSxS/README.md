# .NET Framework and .NET Core COM interoperability

This sample shows how to interoperate between .NET Framework and .NET Core with COM interop.

## To Build

To test calling into a .NET Core COM server, `dotnet build NetSxS.sln`. If you want to test calling into a .NET Framework COM server, then run `dotnet build /p:NetFXServer=true`.

## To Run

Copy the files in the output directory of the Client and Server projects for the respective runtime combination you want to run into a single folder. Run `Client.exe` in that folder.


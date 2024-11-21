# Windows Forms for .NET Samples

| Sample Name | Description |
| ----------- | ----------- |
| [Hello World - shared source](helloworld-sharedsource) | This sample shows you how to share source between a .NET Framework Windows Forms application and a .NET Core Windows Forms application. Use this to get the full .NET Framework tooling experience while still building for .NET Core. |
| [Matching Game](matching-game) | This sample demonstrates simple event handling and timers in a .NET Windows Forms application |
| [DataGridView Sample](datagridview) | This sample demonstrates DataGridView usage in .NET |
| [Graphics Sample](graphics) | This sample demonstrates using GDI+ APIs via the Graphics type in .NET |
| [Sudoku Sample](Sudoku) | This sample demonstrates creating a game using event handling and the Graphics type in .NET |
| [Conway's Game of Life Sample](Conway's-Game-of-Life) | This sample demonstrates creating a DataGridView extension to handle OnRowPrePaint and trapping Windows Messages to prevent a left mouse click in .NET |

## Prerequisites

- Windows Operating System.
- [Visual Studio 2022 version 17.12 or later to target .NET 9.](https://visualstudio.microsoft.com/downloads/?utm_medium=microsoft&utm_source=learn.microsoft.com&utm_campaign=inline+link&utm_content=download+vs2022+desktopguide+winforms+migration)
- [Visual Studio 2022 version 17.8 or later to target .NET 8.](https://visualstudio.microsoft.com/downloads/?utm_medium=microsoft&utm_source=learn.microsoft.com&utm_campaign=inline+link&utm_content=download+vs2022+desktopguide+winforms+migration)

## Porting apps from .NET Framework

If you want to first understand your existing application's readiness for migrating from .NET Framework to .NET, use the [.NET Upgrade Assistant](https://learn.microsoft.com/dotnet/core/porting/upgrade-assistant-how-to-analyze). This tool analyzes projects and generates a report that you can browse to get more information about the upgrade.

To learn how to migrate a Windows Forms app, see [Upgrade a .NET Framework Windows Forms desktop app to .NET](https://learn.microsoft.com/dotnet/desktop/winforms/migration/).

## Create Windows Forms app in the command line

To create a new application you can use the `dotnet new` command, using the new templates for Windows Forms. Open a terminal use use the following commands:

```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet run
```

## Create a Windows Forms app in Visual Studio

To create a new app in Visual Studio, see [Tutorial: Create a Windows Forms app with .NET](https://learn.microsoft.com/dotnet/desktop/winforms/get-started/create-app-visual-studio?view=netdesktop-9.0).

## Filing issues and getting help

You can file Windows Forms related issues in the [dotnet/winforms repo](https://github.com/dotnet/winforms/issues).

---
languages:
- csharp
products:
- dotnet
- windows-forms
page_type: sample
name: ".NET Core Windows Forms Formatting Utility (C#)"
urlFragment: "windowsforms-formatting-utility-cs"
description: "A .NET Core Windows Forms application written in C# that allows you to apply standard or custom format strings."
---

# .NET Formatting Utility

The .NET Formatting Utility (Formatter.exe) is a .NET Core Windows Forms application written in C# that allows you to apply standard or custom format strings to either numeric values or date and time values and to determine how they affect the result string.

## Sample prerequisites

This sample is written in C# and targets .NET 5.0 running on Windows. It requires the [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/5.0).

## Building the sample

The source code includes an MSBuild project file for C# (a .csproj file) that targets .NET 5.0. After you download the .zip file containing the example code, create a directory and Download the sample code files to your computer. To build the example:

1. Download the .zip file containing.

2. Create the directory to which you want to copy the files.

3. Copy the files from the .zip file to the directory you just created.

4. If you are using Visual Studio 2019:

   1. In Visual Studio, select **Open a project or solution** (or **File** > **Open** > **Project/Solution** from the Visual Studio menu.

   2. Select **Debug** > **Start Debugging** from the Visual Studio menu to build and launch the application.

5. If you are working from the command line:

   1. Navigate to the directory that contains the sample.

   2. Type in the command `dotnet run` to build and launch the application.

## Format strings in .NET

*Formatting* involves converting a value to its string representation. `ToString` methods that include a string parameter, [interpolated strings](https://docs.microsoft.com/dotnet/csharp/language-reference/tokens/interpolated), as well as the [composite formatting feature](https://docs.microsoft.com/dotnet/standard/base-types/composite-formatting) supported by such methods as [String.Format](https://docs.microsoft.com/dotnet/api/system.string.format), [StringBuilder.AppendFormat](https://docs.microsoft.com/dotnet/api/system.text.stringbuilder.appendformat), the [Console](https://docs.microsoft.com/dotnet/api/system.console) output methods, and the stream output methods give developers control over the string representations of numbers and dates. They allow such code as:

```cs
Console.WriteLine("{0:YYYY-MM-dd}", thisDate);
```

Frequently, though, developers forget what standard and custom format specifiers are available, and they forget how a particular format string is reflected in the result string. The Format Utility (Formatter.exe) allows you to enter a source value, indicate whether it is a number or a date and time value, and select a standard format string from a drop-down list or enter a custom format string in a text box. By default, numbers and dates use the formatting conventions of the current culture, though you can also select a culture whose formatting conventions you'd like to use. A text box then displays the result string

Note that the standard numeric format strings do not include precision specifiers. If you want to include a precision specifier in a format string, you must enter it yourself.

For information on formatting in .NET, see:

- [Formatting types in .NET](https://docs.microsoft.com/dotnet/standard/base-types/formatting-types).

- [Standard numeric format strings](https://docs.microsoft.com/dotnet/standard/base-types/standard-numeric-format-strings).

- [Custom numeric format strings](https://docs.microsoft.com/dotnet/standard/base-types/custom-numeric-format-strings).

- [Standard date and time format strings](https://docs.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings).

- [Custom date and time format strings](https://docs.microsoft.com/dotnet/standard/base-types/custom-date-and-time-format-strings).

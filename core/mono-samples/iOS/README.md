---
languages:
- c#
products:
- dotnet
- iOS
- mono runtime
page_type: sample
name: "iOS Sample: Simple greeting and counter (C#)"
description: "An iOS application that contains an example of embedding the mono runtime to invoke unmanaged code with C#."
---

# iOS Sample: Simple greeting and counter (C#)

In this sample, the mono runtime is used to invoke objective-c unmanaged code (main.m) from the C# managed side (Program.cs) and vice versa. With the sample running, you can enter your name and click the corresponding button to modify the greeting message as well as clicking a button to increment a counter.

NOTE: The purpose of this sample is to demonstrate the concept of building an iOS application on top of the mono runtime.

## Requirements

To run the sample for iOS you will need a recent version of XCode installed (e.g. 11.3 or higher).

## Building the sample

To build the sample from the command line, run `dotnet publish`.

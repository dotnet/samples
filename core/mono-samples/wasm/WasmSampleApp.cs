// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

public class WasmSampleApp
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Runtime.InteropServices.JavaScript.Runtime", "System.Private.Runtime.InteropServices.JavaScript")]
    public static void Main(string[] args)
    {
        Console.WriteLine ("Hello, World!");
    }

    // Called by javascript, see index.html
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int TestMeaning()
    {
        return 42;
    }

    private static int counter = 1;
    public static string IncrementCounter()
    {
        return $"Clicked {counter++} times!";
    }

    public static string GreetName(string name)
    {
        return name;
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public static class AndroidSampleApp
{
    [DllImport("native-lib")]
    private extern static int myNum();

    [DllImport("native-lib")]
    private extern static void androidRegisterCounterIncrement(Func<string> del);
    private static int counter = 1;

    // Called by native code, see native-lib.c
    private static string IncrementCounter()
    {
        return $"Clicked {counter++} times!";
    }

    public static int Main(string[] args)
    {
        androidRegisterCounterIncrement(IncrementCounter);
        Console.WriteLine("Hello, Android!"); // logcat
        return myNum();
    }
}

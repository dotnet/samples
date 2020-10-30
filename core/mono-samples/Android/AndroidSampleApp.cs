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

    [DllImport("native-lib")]
    private extern static void androidRegisterNameGreet(Func<string,string> del);

    private static int counter = 1;

    // Called by native code, see native-lib.c
    private static string IncrementCounter()
    {
        return $"Clicked {counter++} times!";
    }

    private static string GreetName(string name)
    {
        return $"Hello {name}!\nRunning on mono runtime\nUsing C#";
    }

    public static int Main(string[] args)
    {
        androidRegisterCounterIncrement(IncrementCounter);
        androidRegisterNameGreet(GreetName);
        Console.WriteLine("Hello, Android!"); // logcat
        return myNum();
    }
}

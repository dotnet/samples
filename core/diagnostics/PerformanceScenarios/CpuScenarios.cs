// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static class CpuScenarios
{
    public static Task HotspotAsync(CancellationToken token)
    {
        long result = 0;
        while (!token.IsCancellationRequested)
        {
            result += Fibonacci(36);
        }

        GC.KeepAlive(result);
        return Task.CompletedTask;
    }

    public static Task InliningAsync(CancellationToken token)
    {
        long result = 0;
        while (!token.IsCancellationRequested)
        {
            result += CallerOne();
            result += CallerTwo();
            result += CallerThree();
        }

        GC.KeepAlive(result);
        return Task.CompletedTask;
    }

    public static Task NativeAsync(CancellationToken token)
    {
        const int BufferSize = 1024 * 1024;
        IntPtr source = Marshal.AllocHGlobal(BufferSize);
        IntPtr destination = Marshal.AllocHGlobal(BufferSize);
        try
        {
            byte[] initial = new byte[BufferSize];
            Random.Shared.NextBytes(initial);
            Marshal.Copy(initial, 0, source, initial.Length);

            while (!token.IsCancellationRequested)
            {
                Memcpy(destination, source, BufferSize);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(destination);
            Marshal.FreeHGlobal(source);
        }

        return Task.CompletedTask;
    }

    private static long Fibonacci(int value)
    {
        return value <= 1 ? value : Fibonacci(value - 1) + Fibonacci(value - 2);
    }

    private static long CallerOne() => SharedCompute(4_000);

    private static long CallerTwo() => SharedCompute(5_000);

    private static long CallerThree() => SharedCompute(6_000);

    private static long SharedCompute(int iterations)
    {
        long value = 17;
        for (int index = 0; index < iterations; index++)
        {
            value = unchecked((value * 31) ^ index);
        }

        return value;
    }

    [DllImport("libc", EntryPoint = "memcpy")]
    private static extern IntPtr Memcpy(IntPtr destination, IntPtr source, nuint count);
}

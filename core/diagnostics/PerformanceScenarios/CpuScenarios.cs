// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

    private static long Fibonacci(int value)
    {
        return value <= 1 ? value : Fibonacci(value - 1) + Fibonacci(value - 2);
    }
}

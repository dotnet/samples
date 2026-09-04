// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static class MemoryScenarios
{
    public static Task AllocationGcAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            string value = string.Empty;
            for (int index = 0; index < 2_000; index++)
            {
                value += index.ToString();
            }

            GC.KeepAlive(value.Split('7'));
        }

        return Task.CompletedTask;
    }

    public static Task LohGcAsync(CancellationToken token)
    {
        List<byte[]> retained = [];
        while (!token.IsCancellationRequested)
        {
            retained.Add(new byte[200_000]);
            if (retained.Count > 200)
            {
                retained.RemoveRange(0, 100);
            }
        }

        GC.KeepAlive(retained);
        return Task.CompletedTask;
    }

    public static async Task ManagedGrowthAsync(CancellationToken token)
    {
        List<byte[]> retained = [];
        while (!token.IsCancellationRequested)
        {
            retained.Add(new byte[512 * 1024]);
            await Task.Delay(100, token);
        }

        GC.KeepAlive(retained);
    }

    public static Task InducedGcAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            byte[] data = new byte[1_000_000];
            GC.KeepAlive(data);
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        }

        return Task.CompletedTask;
    }

    public static async Task NativeGrowthAsync(CancellationToken token)
    {
        List<IntPtr> allocations = [];
        try
        {
            while (!token.IsCancellationRequested)
            {
                IntPtr memory = Marshal.AllocHGlobal(256 * 1024);
                for (int offset = 0; offset < 256 * 1024; offset += 4096)
                {
                    Marshal.WriteByte(memory, offset, 1);
                }

                allocations.Add(memory);
                await Task.Delay(100, token);
            }
        }
        finally
        {
            foreach (IntPtr allocation in allocations)
            {
                Marshal.FreeHGlobal(allocation);
            }
        }
    }
}

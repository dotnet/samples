// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class MemoryScenarios
{
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
}

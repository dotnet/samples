// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Emit;

internal static class RuntimeScenarios
{
    public static async Task JitStartupAsync(CancellationToken token)
    {
        for (int index = 0; index < 10_000 && !token.IsCancellationRequested; index++)
        {
            DynamicMethod method = new($"Generated{index}", typeof(int), [typeof(int)]);
            ILGenerator generator = method.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldc_I4_1);
            generator.Emit(OpCodes.Add);
            generator.Emit(OpCodes.Ret);
            Func<int, int> callback = method.CreateDelegate<Func<int, int>>();
            GC.KeepAlive(callback(index));
        }

        await Task.Delay(Timeout.Infinite, token);
    }

    public static Task SwallowedExceptionsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                throw new InvalidOperationException("Expected and ignored.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        return Task.CompletedTask;
    }
}

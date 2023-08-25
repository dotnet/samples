using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

[assembly: DisableRuntimeMarshalling]
public class Program
{
    public static void Main()
    {
        nint ptrToCalc = PInvokes.GetNativeCalculator();
        ComWrappers cw = new StrategyBasedComWrappers();
        object obj = cw.GetOrCreateObjectForComInstance(ptrToCalc, CreateObjectFlags.None);
        ISimpleCalculator calculator = (ISimpleCalculator)obj;
        int a = 5;
        int b = 3;
        int c = calculator.Add(a, b);
        Debug.Assert(c == 8);
        c = calculator.Subtract(a, b);
        Debug.Assert(c == 2);
    }
}

[GeneratedComInterface]
[Guid("c67121c6-cf26-431f-adc7-d12fe2448841")]
internal partial interface ISimpleCalculator
{
    int Add(int a, int b);
    int Subtract(int a, int b);
}

[GeneratedComClass]
internal partial class Calculator : ISimpleCalculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
}

public static partial class PInvokes
{
    [LibraryImport("BuiltInCOM", EntryPoint = "GetNativeCalculator")]
    internal static partial nint GetNativeCalculator();
}

public static class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "GetNativeCalculator")]
    public static nint GetNativeCalculator()
    {
        ComWrappers cw = new StrategyBasedComWrappers();
        return cw.GetOrCreateComInterfaceForObject(new Calculator(), CreateComInterfaceFlags.None);
    }
}

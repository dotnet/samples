using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

public class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new InvalidOperationException("This program can only run on Windows");
        }
        nint ptrToCalc = PInvokes.GetNativeCalculator();
        object obj = Marshal.GetObjectForIUnknown(ptrToCalc);
        ISimpleCalculator calculator = (ISimpleCalculator)obj;
        int a = 5;
        int b = 3;
        int c = calculator.Add(a, b);
        Debug.Assert(c == 8);
        c = calculator.Subtract(a, b);
        Debug.Assert(c == 2);
    }
}

[ComImport]
[ComVisible(true)]
[Guid("c67121c6-cf26-431f-adc7-d12fe2448841")]
internal interface ISimpleCalculator
{
    int Add(int a, int b);
    int Subtract(int a, int b);
}

[ComVisible(true)]
[Guid("c67121c6-cf27-431f-adc7-d12fe2448841")]
internal class Calculator : ISimpleCalculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
}

public static class PInvokes
{
    [DllImport("ComInterfaceGeneratorDemo", EntryPoint = "GetNativeCalculator")]
    internal static extern nint GetNativeCalculator();

    [DllImport("ComInterfaceGeneratorDemo", EntryPoint = "GetNativeCalculator")]
    internal static extern ISimpleCalculator GetCalculator();
}

public static class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "GetNativeCalculator")]
    public static nint GetNativeCalculator()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new InvalidOperationException("This program can only run on Windows");
        }
        return Marshal.GetComInterfaceForObject(new Calculator(), typeof(ISimpleCalculator));
    }
}

internal class Ole32
{
    // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
    [DllImport(nameof(Ole32))]
    public static extern int CoCreateInstance(
        [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
        IntPtr pUnkOuter,
        uint dwClsContext,
        [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
        [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
}

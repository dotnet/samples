using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static Tutorial.ComInterfaces;

[assembly: DisableRuntimeMarshalling]

namespace Tutorial;

public class Program
{
    public static void Main()
    {
        Ole32.CoCreateInstance(new Guid(ClsIds.ClassFactory), 0, 0, new Guid(IClassFactory.IID), out object objClassFactory);
        IClassFactory factory = (IClassFactory) objClassFactory;

        int a = 5;
        int b = 3;
        int c;
        Guid calc_iid = new Guid(ISimpleCalculator.IID);
        object obj = factory.CreateInstance(null, in calc_iid);
        ISimpleCalculator calc = (ISimpleCalculator)obj;

        c = calc.Add(a, b);
        Debug.Assert(c == 8);
        c = calc.Subtract(a, b);
        Debug.Assert(c == 2);
    }
}

public static partial class PInvokes
{
    [LibraryImport("ComServer", EntryPoint = "GetNativeCalculator")]
    [return: MarshalAs(UnmanagedType.Interface)]
    internal static partial ISimpleCalculator GetCalculator();
    [LibraryImport("ComServer", EntryPoint = nameof(GetClassFactory))]
    internal static partial IClassFactory GetClassFactory();
}


internal static partial class Ole32
{
    // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
    [LibraryImport(nameof(Ole32))]
    public static partial int CoCreateInstance(
        Guid rclsid,
        IntPtr pUnkOuter,
        uint dwClsContext,
        Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out object ppv);
}

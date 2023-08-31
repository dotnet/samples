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
    public static unsafe void Main()
    {
        int hr;
        int a = 5;
        int b = 3;
        int c;

        // string tempClsid = "10144713-1526-46C9-88DA-1FB52807A9FF";
        // string tempIID = "E357FCCD-A995-4576-B01F-234630154E96";
        // var rclsid = new Guid(tempClsid);
        // var riid = new Guid(tempIID);
        var rclsid =  new Guid(ClsIds.Calculator);
        var riid = new Guid(ISimpleCalculator.IID);
        nint iunk = 0;
        hr = Ole32.CoCreateInstance(in rclsid, 0, (uint)Ole32.CLSCTX.CLSCTX_INPROC_SERVER, in riid, &iunk);
        Console.WriteLine("Returned from CoCreate: " + hr);
        Console.WriteLine($"Returned from CoCreate: ptr = {iunk:x}");

        var vtable = *(void***)iunk;
        // nint outval;
        Guid iid = new(ISimpleCalculator.IID);
        nint icalc;
        hr = Marshal.QueryInterface(iunk, ref iid, out icalc);
        // hr = ((delegate* unmanaged[MemberFunction]<void*, Guid*, nint*, int>)vtable[0])((void*)iunk, &iid, &icalc);
        if (hr != 0)
        {
            Console.WriteLine($"Failed");
        }
        Console.WriteLine($"ISimpleCalc: {icalc:x}");

        vtable = *(void***)icalc;
        Console.WriteLine($"methodptr: {(nint)vtable[3]:x}");
        hr = ((delegate* unmanaged<void*, int, int, int*, int>)vtable[3])((void*)icalc, 2, 3, &c);
        if (hr != 0)
        {
            Console.WriteLine($"failed");
        }
        Console.WriteLine($"Added: {c}");
        // var vtable = *(void***)iunk;
        // var cw = new StrategyBasedComWrappers();
        // var ivokeval = ((delegate* unmanaged[MemberFunction]<void*, int, int, int*, int>)vtable[3])((void*)iunk, 2, 3, &c);
        // Console.WriteLine(c);
        // var obj = cw.GetOrCreateObjectForComInstance(iunk, CreateObjectFlags.None);
        // Console.WriteLine(obj);

        // ISimpleCalculator calc = (ISimpleCalculator)obj;
        // Console.WriteLine($"asdfasddf");
        // Console.WriteLine(calc);
        // Console.WriteLine(obj as ComObject);

        // c = calc.Add(a, b);
        // Console.WriteLine($"{a} + {b} = {c}");
        // Debug.Assert(c == 8);
        // c = calc.Subtract(a, b);
        // Console.WriteLine($"{a} - {b} = {c}");
        // Debug.Assert(c == 2);
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


internal static unsafe partial class Ole32
{
    // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
    [LibraryImport(nameof(Ole32))]
    public static partial int CoCreateInstance(
        in Guid rclsid,
        IntPtr pUnkOuter,
        uint dwClsContext,
        in Guid riid,
        nint* ppv);

    public enum CLSCTX : uint
    {
        CLSCTX_INPROC_SERVER = 0x1,
        CLSCTX_INPROC_HANDLER = 0x2,
        CLSCTX_LOCAL_SERVER = 0x4,
        CLSCTX_INPROC_SERVER16 = 0x8,
        CLSCTX_REMOTE_SERVER = 0x10,
        CLSCTX_INPROC_HANDLER16 = 0x20,
        CLSCTX_RESERVED1 = 0x40,
        CLSCTX_RESERVED2 = 0x80,
        CLSCTX_RESERVED3 = 0x100,
        CLSCTX_RESERVED4 = 0x200,
        CLSCTX_NO_CODE_DOWNLOAD = 0x400,
        CLSCTX_RESERVED5 = 0x800,
        CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
        CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
        CLSCTX_NO_FAILURE_LOG = 0x4000,
        CLSCTX_DISABLE_AAA = 0x8000,
        CLSCTX_ENABLE_AAA = 0x10000,
        CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
        CLSCTX_ACTIVATE_X86_SERVER = 0x40000,
        CLSCTX_ACTIVATE_32_BIT_SERVER,
        CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
        CLSCTX_ENABLE_CLOAKING = 0x100000,
        CLSCTX_APPCONTAINER = 0x400000,
        CLSCTX_ACTIVATE_AAA_AS_IU = 0x800000,
        CLSCTX_RESERVED6 = 0x1000000,
        CLSCTX_ACTIVATE_ARM32_SERVER = 0x2000000,
        CLSCTX_ALLOW_LOWER_TRUST_REGISTRATION,
        CLSCTX_PS_DLL = 0x80000000,
    }
}

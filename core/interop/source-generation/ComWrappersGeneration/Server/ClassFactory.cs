
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;
using static Tutorial.ComInterfaces;

namespace Tutorial;

[GeneratedComClass]
[Guid(ClsId)]
public unsafe partial class ClassFactory : IClassFactory
{
    public void CreateInstance(nint outer, ref Guid id, nint* iface)
    {
        Console.WriteLine($"CreateFactory requested IID: {id}");
        var cw = new StrategyBasedComWrappers();
        var iunk = cw.GetOrCreateComInterfaceForObject(new Calculator(), CreateComInterfaceFlags.None);
        Console.WriteLine($"Iunk: {iunk:x}");
        var vtable = *(void***)iunk;
        // nint outval;
        var hr = Marshal.QueryInterface(iunk, ref id, out *iface);
        // var invokeval = ((delegate* unmanaged[MemberFunction]<void*, Guid*, nint*, int>)vtable[0])((void*)iunk, &iid, iface);
        Console.WriteLine($"ISimpleCalc: {*iface:x}");

        int c;
        vtable = *(void***)(*iface);
        Console.WriteLine($"MethodPtr: {(nint)vtable[3]:x}");
        var ivokeval = ((delegate* unmanaged<void*, int, int, int*, int>)vtable[3])((void*)(*iface), 2, 3, &c);
        Console.WriteLine(ivokeval);
        Console.WriteLine(c);
        // Console.WriteLine(invokeval);
        // Console.WriteLine($"ISimpleCalc: {outval}");
        // *iface = outval;
        // Console.WriteLine($"iface: {*iface}");

        // int c;
        // vtable = *(void***)outval;
        // Console.WriteLine((nint)vtable);
        // var ivokeval = ((delegate* unmanaged[MemberFunction]<void*, int, int, int*, int>)vtable[3])((void*)outval, 2, 3, &c);
        // Console.WriteLine(ivokeval);
        // Console.WriteLine(c);
    }

    public void LockServer(bool fLock) { }

    internal const string ClsId = ClsIds.ClassFactory;
}

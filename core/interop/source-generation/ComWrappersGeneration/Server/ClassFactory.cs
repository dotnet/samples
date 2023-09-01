
using System;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;
using static Tutorial.ComInterfaces;

namespace Tutorial;

[GeneratedComClass]
[Guid(ClsId)]
public unsafe partial class ClassFactory : IClassFactory
{
    public void CreateInstance(nint pOuter, ref Guid iid, nint* ppInterface)
    {
        Console.WriteLine($"IID requested from ClassFactory.CreateInstance: {iid}");
        if (pOuter != 0)
        {
            *ppInterface = 0;
            throw new COMException("Class does not support aggregation", -2147221232 /* CLASS_E_NOAGGREGATION */);
        }
        Calculator calculator = new();
        nint pIUnknown = (nint)ComInterfaceMarshaller<Calculator>.ConvertToUnmanaged(calculator);
        var hr = Marshal.QueryInterface(pIUnknown, ref iid, out *ppInterface);
        Marshal.Release(pIUnknown);
        Marshal.ThrowExceptionForHR(hr);
    }

    public void LockServer(bool fLock) { }

    internal const string ClsId = ClsIds.ClassFactory;
}

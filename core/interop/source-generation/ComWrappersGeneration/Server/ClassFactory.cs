using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Tutorial;

[GeneratedComClass]
public unsafe partial class ClassFactory : IClassFactory
{
    public void CreateInstance(nint pOuter, ref Guid iid, nint* ppInterface)
    {
        Console.WriteLine($"Server: IID requested from ClassFactory.CreateInstance: {iid}");
        if (pOuter != 0)
        {
            *ppInterface = 0;
            const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
            throw new COMException("Class does not support aggregation", CLASS_E_NOAGGREGATION);
        }
        Calculator calculator = new();
        nint pIUnknown = (nint)ComInterfaceMarshaller<Calculator>.ConvertToUnmanaged(calculator);
        var hr = Marshal.QueryInterface(pIUnknown, ref iid, out *ppInterface);
        Marshal.Release(pIUnknown);
        Marshal.ThrowExceptionForHR(hr);
    }

    public void LockServer(bool fLock) { }
}

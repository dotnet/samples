using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Tutorial;

[GeneratedComClass]
public unsafe partial class ClassFactory : IClassFactory
{
    public static ClassFactory Instance { get; } = new ClassFactory();
    public void CreateInstance(nint pOuter, in Guid iid, out object? ppInterface)
    {
        Console.WriteLine($"Server: IID requested from ClassFactory.CreateInstance: {iid}");
        if (pOuter != 0)
        {
            ppInterface = null;
            const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
            throw new COMException("Class does not support aggregation", CLASS_E_NOAGGREGATION);
        }
        Calculator calculator = new();
        ppInterface = calculator;
    }

    public void LockServer(bool fLock) { }
}

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Tutorial;

/// <summary>
/// The interface that the COM system uses to create new objects when clients use CoCreateInstance.
/// <see href="https://learn.microsoft.com/windows/win32/api/unknwn/nn-unknwn-iclassfactory" />
/// </summary>
[GeneratedComInterface]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(IID)]
internal unsafe partial interface IClassFactory
{
    void CreateInstance(nint outer, in Guid id, [MarshalAs(UnmanagedType.Interface)] out object? iface);
    void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
    public const string IID = "00000001-0000-0000-C000-000000000046";
}

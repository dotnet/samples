using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System;

namespace Tutorial;
public static partial class ComInterfaces
{
    [GeneratedComInterface]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid(IID)]
    internal partial interface IClassFactory
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object CreateInstance([MarshalAs(UnmanagedType.Interface)] object? outer, in Guid id);
        void LockServer([MarshalAs(UnmanagedType.U4)] bool fLock);
        public const string IID = "e5acc998-8195-47ba-9fae-854da721e324";
    }

    [GeneratedComInterface]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid(IID)]
    internal partial interface ISimpleCalculator
    {
        int Add(int a, int b);
        int Subtract(int a, int b);
        public const string IID = "c67121c6-cf26-431f-adc7-d12fe2448841";
    }
}

using System;
using System.Runtime.InteropServices;

namespace ManagedClient
{
    class Program
    {
        static void Main(string[] _)
        {
            // If the COM server is registered as an in-proc server (as is the case when using
            // a DLL surrogate), activation through the Activator will only use the out-of-proc
            // server if the client and the registered COM server are not the same bitness.
            //
            // Type t = Type.GetTypeFromCLSID(Contract.Constants.ServerClassGuid);
            // var server = (IServer)Activator.CreateInstance(t);
            //
            // This demo explicitly calls CoCreateInstance with CLSCTX_LOCAL_SERVER to force
            // usage of the out-of-proc server.
            object obj;
            int hr = Ole32.CoCreateInstance(Contract.Constants.ServerClassGuid, IntPtr.Zero, Ole32.CLSCTX_LOCAL_SERVER, typeof(IServer).GUID, out obj);
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            var server = (IServer)obj;
            double pi = server.ComputePi();
            Console.WriteLine($"\u03C0 = {pi}");
        }

        private class Ole32
        {
            // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
            public const int CLSCTX_LOCAL_SERVER = 0x4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
            [DllImport(nameof(Ole32))]
            public static extern int CoCreateInstance(
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
                IntPtr pUnkOuter,
                uint dwClsContext,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

// The Assembly GUID and TLB version must match what
// is defined in Contract.idl when defining the library.
[assembly: Guid("46F3FEB2-121D-4830-AA22-0CDA9EA90DC3")]
[assembly: TypeLibVersion(1, 0)]

namespace OutOfProcCOM
{
    [ComVisible(true)]
    [Guid(Contract.Constants.ServerClass)]
    [ComDefaultInterface(typeof(IServer))]
    public sealed class DllServer : IServer
    {
        double IServer.ComputePi()
        {
            Trace.WriteLine($"Running {nameof(DllServer)}.{nameof(IServer.ComputePi)}");
            double sum = 0.0;
            int sign = 1;
            for (int i = 0; i < 1024; ++i)
            {
                sum += sign / (2.0 * i + 1.0);
                sign *= -1;
            }

            return 4.0 * sum;
        }

#if EMBEDDED_TYPE_LIBRARY
        private static readonly string tlbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{nameof(DllServer)}.comhost.dll");
#else
        private static readonly string tlbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Contract.Constants.TypeLibraryName);
#endif

        [ComRegisterFunction]
        internal static void RegisterFunction(Type t)
        {
            if (t != typeof(DllServer))
                return;

            // Register DLL surrogate and type library
            COMRegistration.DllSurrogate.Register(Contract.Constants.ServerClassGuid, tlbPath);
        }

        [ComUnregisterFunction]
        internal static void UnregisterFunction(Type t)
        {
            if (t != typeof(DllServer))
                return;

            // Unregister DLL surrogate and type library
            COMRegistration.DllSurrogate.Unregister(Contract.Constants.ServerClassGuid, tlbPath);
        }
    }
}

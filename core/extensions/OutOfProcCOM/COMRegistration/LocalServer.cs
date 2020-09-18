using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace COMRegistration
{
    public sealed class LocalServer : IDisposable
    {
        public static void Register(Guid clsid, string exePath, string tlbPath)
        {
            // Register local server
            Trace.WriteLine($"Registering server:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.WriteLine($"Executable: {exePath}");
            Trace.Unindent();

            string serverKey = string.Format(KeyFormat.LocalServer32, clsid);
            using RegistryKey regKey = Registry.LocalMachine.CreateSubKey(serverKey);
            regKey.SetValue(null, exePath);

            // Register type library
            TypeLib.Register(tlbPath);
        }

        public static void Unregister(Guid clsid, string tlbPath)
        {
            Trace.WriteLine($"Unregistering server:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.Unindent();

            // Unregister local server
            string serverKey = string.Format(KeyFormat.LocalServer32, clsid);
            Registry.LocalMachine.DeleteSubKey(serverKey, throwOnMissingSubKey: false);

            // Unregister type library
            TypeLib.Unregister(tlbPath);
        }

        private readonly List<int> registrationCookies = new List<int>();

        public void RegisterClass<T>(Guid clsid) where T : new()
        {
            Trace.WriteLine($"Registering class object:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.WriteLine($"Type: {typeof(T)}");

            int cookie;
            int hr = Ole32.CoRegisterClassObject(ref clsid, new BasicClassFactory<T>(), Ole32.CLSCTX_LOCAL_SERVER, Ole32.REGCLS_MULTIPLEUSE | Ole32.REGCLS_SUSPENDED, out cookie);
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            registrationCookies.Add(cookie);
            Trace.WriteLine($"Cookie: {cookie}");
            Trace.Unindent();

            hr = Ole32.CoResumeClassObjects();
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        public void Run()
        {
            // This sample does not handle lifetime management of the server.
            // For details around ref counting and locking of out-of-proc COM servers, see
            // https://docs.microsoft.com/windows/win32/com/out-of-process-server-implementation-helpers
            Console.ReadLine();
        }

        public void Dispose()
        {
            Trace.WriteLine($"Revoking class object registrations:");
            Trace.Indent();
            foreach (int cookie in registrationCookies)
            {
                Trace.WriteLine($"Cookie: {cookie}");
                int hr = Ole32.CoRevokeClassObject(cookie);
                Debug.Assert(hr >= 0, $"CoRevokeClassObject failed ({hr:x}). Cookie: {cookie}");
            }

            Trace.Unindent();
        }

        private class Ole32
        {
            // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
            public const int CLSCTX_LOCAL_SERVER = 0x4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/ne-combaseapi-regcls
            public const int REGCLS_MULTIPLEUSE = 1;
            public const int REGCLS_SUSPENDED = 4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-coregisterclassobject
            [DllImport(nameof(Ole32))]
            public static extern int CoRegisterClassObject(ref Guid guid, [MarshalAs(UnmanagedType.IUnknown)] object obj, int context, int flags, out int register);

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-coresumeclassobjects
            [DllImport(nameof(Ole32))]
            public static extern int CoResumeClassObjects();

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-corevokeclassobject
            [DllImport(nameof(Ole32))]
            public static extern int CoRevokeClassObject(int register);
        }
    }
}

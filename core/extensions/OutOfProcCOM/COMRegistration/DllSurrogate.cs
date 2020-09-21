using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace COMRegistration
{
    public static class DllSurrogate
    {
        public static void Register(Guid clsid, string tlbPath)
        {
            Trace.WriteLine($"Registering server with system-supplied DLL surrogate:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.Unindent();

            string serverKey = string.Format(KeyFormat.CLSID, clsid);

            // Register App ID - use the CLSID as the App ID
            using RegistryKey regKey = Registry.LocalMachine.CreateSubKey(serverKey);
            regKey.SetValue("AppID", clsid.ToString("B"));

            // Register DLL surrogate - empty string for system-supplied surrogate
            string appIdKey = string.Format(KeyFormat.AppID, clsid);
            using RegistryKey appIdRegKey = Registry.LocalMachine.CreateSubKey(appIdKey);
            appIdRegKey.SetValue("DllSurrogate", string.Empty);

            // Register type library
            TypeLib.Register(tlbPath);
        }

        public static void Unregister(Guid clsid, string tlbPath)
        {
            Trace.WriteLine($"Unregistering server:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.Unindent();

            // Remove the App ID value
            string serverKey = string.Format(KeyFormat.CLSID, clsid);
            using RegistryKey regKey = Registry.LocalMachine.OpenSubKey(serverKey, writable: true);
            if (regKey != null)
                regKey.DeleteValue("AppID");

            // Remove the App ID key
            string appIdKey = string.Format(KeyFormat.AppID, clsid);
            Registry.LocalMachine.DeleteSubKey(appIdKey, throwOnMissingSubKey: false);

            // Unregister type library
            TypeLib.Unregister(tlbPath);
        }
    }
}

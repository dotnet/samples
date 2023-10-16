using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;

namespace Tutorial;

public static unsafe class Exports
{
    /// <summary>
    /// Returns a pointer to an IClassFactory instance that corresponds to the requested <paramref name="classId"/>.
    /// <paramref name="interfaceId"/> is expected to be the IID of IClassFactory.
    /// This method is called by the COM system when a client requests an object that this server has registered.
    /// <see href="https://learn.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-dllgetclassobject"/>
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = nameof(DllGetClassObject))]
    public static int DllGetClassObject([DNNE.C99Type("void*")] Guid* classId, [DNNE.C99Type("void*")] Guid* interfaceId, nint* ppIClassFactory)
    {
        Console.WriteLine($"Server: Class ID requested from DllGetClassObject: {*classId}");
        Console.WriteLine($"Server: Interface ID requested from DllGetClassObject: {*interfaceId}");
        if (*classId != new Guid(Clsids.Calculator)
            || *interfaceId != new Guid(IClassFactory.IID))
        {
            *ppIClassFactory = 0;
            const int CLASS_E_CLASSNOTAVAILABLE = unchecked((int)0x80040111);
            return CLASS_E_CLASSNOTAVAILABLE;
        }
        ClassFactory factory = ClassFactory.Instance;
        nint pIUnknown = (nint)ComInterfaceMarshaller<ClassFactory>.ConvertToUnmanaged(factory);
        // Call QI on the COM ptr from COM wrappers to get the requested interface pointer
        // This is IClassFactory for CoCreateInstance
        int hr = Marshal.QueryInterface(pIUnknown, in *interfaceId, out *ppIClassFactory);
        Marshal.Release(pIUnknown);
        if (hr != 0)
        {
            Console.WriteLine($"Server: QueryInterface in DllGetClassObject failed: {hr:x}");
            return hr;
        }
        return 0;
    }

    /// <summary>
    /// Registers the server with the COM system.
    /// Called by <c>regsvr32.exe</c> when run with this .dll as the argument.
    /// <see href="https://learn.microsoft.com/windows/win32/api/olectl/nf-olectl-dllregisterserver"/>
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = nameof(DllRegisterServer))]
    public static int DllRegisterServer()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return -1;

        if (!FileUtils.TryGetDllPath(out string? dllPath))
        {
            const int SELFREG_E_CLASS = unchecked((int)0x80040201);
            return SELFREG_E_CLASS;
        }
        CreateComRegistryEntryForClass(Calculator.Clsid, nameof(Calculator), dllPath!);
        return 0;
    }

    static void CreateComRegistryEntryForClass(string clsid, string className, string dllPath)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new InvalidOperationException();

        string progId = GetProgId(className);

        using (RegistryKey key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}"""))
        {
            key.SetValue(null, className, RegistryValueKind.String);
            key.SetValue("ProgId", progId, RegistryValueKind.String);
        }
        using (RegistryKey key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}\InprocServer32"""))
        {
            key.SetValue(null, dllPath, RegistryValueKind.String);
            key.SetValue("ThreadingModel", "Both", RegistryValueKind.String);
        }
        using (RegistryKey key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\{{{progId}}}"""))
        {
            key.SetValue(null, className, RegistryValueKind.String);
            key.SetValue("CLSID", clsid, RegistryValueKind.String);
        }
    }

    /// <summary>
    /// Unregisters the server from the COM system.
    /// Called by <c>regsvr32.exe</c> when run with the -u flag and this .dll as the argument
    /// <see href="https://learn.microsoft.com/windows/win32/api/olectl/nf-olectl-dllunregisterserver"/>
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = nameof(DllUnregisterServer))]
    public static int DllUnregisterServer()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new InvalidOperationException();

        string clsid = Calculator.Clsid;
        string progId = GetProgId(nameof(Calculator));

        Registry.CurrentUser.DeleteSubKeyTree($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}""");
        Registry.CurrentUser.DeleteSubKeyTree($$"""SOFTWARE\Classes\{{{progId}}}""");
        return 0;
    }

    public static string GetProgId(string className) => $"Tutorial.{className}.0";
}

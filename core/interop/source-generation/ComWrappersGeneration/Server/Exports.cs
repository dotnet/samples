using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;
using static Tutorial.ComInterfaces;

namespace Tutorial;

public static unsafe class Exports
{
    private const string SamplesRoot = @"<Path to dotnet/samples repository base>";
    private const string PathToDll = SamplesRoot + @"\core\interop\source-generation\ComWrappersGeneration\OutputFiles\Server.dll";

    [UnmanagedCallersOnly(EntryPoint = nameof(DllGetClassObject))]
    public static int DllGetClassObject(Guid* classId, Guid* interfaceId, nint* ppIClassFactory)
    {
        Console.WriteLine($"Class ID requested from DllGetClassObject: {*classId}");
        Console.WriteLine($"Interface ID requested from DllGetClassObject: {*interfaceId}");
        if (*classId != new Guid(ClsIds.Calculator)
            || *interfaceId != new Guid(IClassFactory.IID))
        {
            *ppIClassFactory = 0;
            return -2147221231; // 0x80040111 CLASS_E_CLASSNOTAVAILABLE
        }
        var factory = new ClassFactory();
        nint pIUnknown = (nint)ComInterfaceMarshaller<ClassFactory>.ConvertToUnmanaged(factory);

        // Call QI on the COM ptr from COM wrappers to get the requested interface pointer
        // This is IClassFactory for CoCreateInstance
        var hr = Marshal.QueryInterface(pIUnknown, ref *interfaceId, out *ppIClassFactory);
        Marshal.Release(pIUnknown);
        if (hr != 0)
        {
            Console.WriteLine($"QueryInterface in DllGetClassObject failed: {hr:x}");
            return hr;
        }
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(DllRegisterServer))]
    public static int DllRegisterServer()
    {
        const string InprocServer32 = nameof(InprocServer32);
        const string ProgId = nameof(ProgId);
        const string CLSID = nameof(CLSID);
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return -1;

        CreateKeyForClass(Calculator.ClsId, nameof(Calculator));

        return 0;

        static void CreateKeyForClass(string clsid, string className)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new InvalidOperationException();

            string progId = $"Tutorial.{className}.0";

            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}"""))
            {
                key.SetValue(null, className, RegistryValueKind.String);
                key.SetValue("ProgId", progId, RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}\{{InprocServer32}}"""))
            {
                key.SetValue(null, PathToDll, RegistryValueKind.String);
                key.SetValue("ThreadingModel", "Both", RegistryValueKind.String);
                // key.SetValue(null, typeof(ClassFactory).Assembly.GetName().Name + ".dll", RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\{{{progId}}}"""))
            {
                key.SetValue(null, className, RegistryValueKind.String);
                key.SetValue("CLSID", clsid, RegistryValueKind.String);
            }
        }
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(DllUnregisterServer))]
    public static int DllUnregisterServer()
    {

        DeleteKey(Calculator.ClsId, nameof(Calculator));

        return 0;

        static void DeleteKey(string clsid, string className)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new InvalidOperationException();

            string progId = $"Tutorial.{className}.0";

            Registry.CurrentUser.DeleteSubKeyTree($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}""");
            Registry.CurrentUser.DeleteSubKeyTree($$"""SOFTWARE\Classes\{{{progId}}}""");
        }
    }
}

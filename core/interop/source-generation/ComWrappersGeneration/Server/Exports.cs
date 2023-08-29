using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32;
using static Tutorial.ComInterfaces;

namespace Tutorial;

public static unsafe class Exports
{
    [UnmanagedCallersOnly(EntryPoint = nameof(GetClassFactory))]
    public static nint GetClassFactory()
    {
        ClassFactory cf = new();
        var cw = new StrategyBasedComWrappers();
        return cw.GetOrCreateComInterfaceForObject(cf, CreateComInterfaceFlags.None);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(GetNativeCalculator))]
    public static nint GetNativeCalculator()
    {
        ComWrappers cw = new StrategyBasedComWrappers();
        return cw.GetOrCreateComInterfaceForObject(new Calculator(), CreateComInterfaceFlags.None);
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(DllGetClassObject))]
    public static int DllGetClassObject(Guid classId, Guid interfaceId, void** value)
    {
        var factory = new ClassFactory();
        var cw = new StrategyBasedComWrappers();
        *value = (void*)cw.GetOrCreateComInterfaceForObject(factory, CreateComInterfaceFlags.None);
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

        CreateKeyForAssembly(ISimpleCalculator.IID, Calculator.ClsId, nameof(ISimpleCalculator), nameof(Calculator), 5);

        return 0;

        static void CreateKeyForAssembly(string iid, string clsid, string interfaceName, string className, int numMethods)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new InvalidOperationException();

            string progId = $"Tutorial.{className}.0";

            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}"""))
            {
                key.SetValue(null, className, RegistryValueKind.String);
                key.SetValue("ProgId", progId, RegistryValueKind.String);
            }

            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}\Interface\{{{iid}}}"""))
            {
                key.SetValue(null, interfaceName, RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}\Interface\{{{iid}}}"""))
            {
                key.SetValue(null, interfaceName, RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}\{{InprocServer32}}"""))
            {
                key.SetValue(null, @"C:\src\samples\core\interop\source-generation\ComWrappersGeneration\OutputFiles\Server.dll", RegistryValueKind.String);
                // key.SetValue(null, typeof(ClassFactory).Assembly.GetName().Name + ".dll", RegistryValueKind.String);
            }

            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\Interface\{{{iid}}}"""))
            {
                key.SetValue(null, interfaceName, RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\Interface\{{{iid}}}\ProxyStubClsid32\{{{clsid}}}"""))
            {
                key.SetValue(null, className, RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\Interface\{{{iid}}}\NumMethods"""))
            {
                key.SetValue(null, numMethods, RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\{{{progId}}}"""))
            {
                key.SetValue(null, className, RegistryValueKind.String);
                key.SetValue("CLSID", clsid, RegistryValueKind.String);
            }

            // static void CreateComRegKey(string value, params string[] path)
            // {
            //     if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //         throw new InvalidOperationException();

            //     var keyPath = @"SOFTWARE\Classes\" + string.Join('\\', path);
            //     using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            //     {
            //         key.SetValue(null, value, RegistryValueKind.String);
            //     }
            // }
        }
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(DllUnregisterServer))]
    public static int DllUnregisterServer()
    {
        return 0;
    }
}

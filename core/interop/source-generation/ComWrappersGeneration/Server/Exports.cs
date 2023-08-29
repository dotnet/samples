using System;
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

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return -1;

        CreateKeyForAssembly(Calculator.ClsId, ISimpleCalculator.IID);
        CreateKeyForAssembly(ClassFactory.ClsId, IClassFactory.IID);

        return 0;

        static void CreateKeyForAssembly(string iid, string clsid)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new InvalidOperationException();
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{Calculator.ClsId}}}"""))
            {
                key.SetValue(null, "Calculator", RegistryValueKind.String);
            }
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{Calculator.ClsId}}}\{{InprocServer32}}"""))
            {
                key.SetValue(null, AppContext.BaseDirectory + typeof(ClassFactory).Assembly.GetName().Name + ".exe", RegistryValueKind.String);
            }

            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\Interface\{{{ISimpleCalculator.IID}}}"""))
            {
                key.SetValue(null, "ISimpleCalculator", RegistryValueKind.String);
            }

            // using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\ProgId\{{{ISimpleCalculator.IID}}}"""))
            // {
            //     key.SetValue(null, "Description", RegistryValueKind.String);
            // }
        }
    }

    [UnmanagedCallersOnly(EntryPoint = nameof(DllUnregisterServer))]
    public static int DllUnregisterServer()
    {
        return 0;
    }
}

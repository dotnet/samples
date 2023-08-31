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
    public static int DllGetClassObject(Guid* classId, Guid* interfaceId, void** value)
    {
        Console.WriteLine($"Class ID requested from DllGetClassObject: {*classId}");
        Console.WriteLine($"Interface ID requested from DllGetClassObject: {*interfaceId}");
        var factory = new ClassFactory();
        var cw = new StrategyBasedComWrappers();
        var val = cw.GetOrCreateComInterfaceForObject(factory, CreateComInterfaceFlags.None);

        // Call QI on the COM ptr from COM wrappers
        var vtable = *(void***)val;
        nint outval = 0;
        Guid iid = new (IClassFactory.IID);
        var hr = Marshal.QueryInterface(val, ref iid, out nint icf);
        Marshal.Release(val);
        // var hr = ((delegate* unmanaged[MemberFunction]<void*, Guid*, nint*, int> )vtable[0])((void*)val, &iid, &outval);
        if (hr != 0)
        {
            Console.WriteLine($"QI in DllGetClassObject failed: {hr}");
            return hr;
        }

        // Release val
        *value = (void*)icf;
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
            using (var key = Registry.CurrentUser.CreateSubKey($$"""SOFTWARE\Classes\CLSID\{{{clsid}}}\{{InprocServer32}}"""))
            {
                string dllPath =@"C:\src\samples\core\interop\source-generation\ComWrappersGeneration\Server\bin\Debug\net8.0\win-x64\publish\Server.dll";
                key.SetValue(null, dllPath, RegistryValueKind.String);
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
        return 0;
    }
}

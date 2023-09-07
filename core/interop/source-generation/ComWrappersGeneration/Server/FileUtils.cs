
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Tutorial;

public static class FileUtils
{
    public static unsafe bool TryGetDllPath([NotNullWhen(true)]out string? dllPath)
    {
        if (TryGetDllPathCoreCLR(out dllPath))
            return true;

        return TryGetDllPathNativeAOT(out dllPath);
    }

    public static unsafe bool TryGetDllPathCoreCLR([NotNullWhen(true)]out string? dllPath)
    {
        string assemblyPath = typeof(FileUtils).Assembly.Location;
        if (assemblyPath == "")
        {
            dllPath = null;
            return false;
        }
        const string dnneSuffix = "NE.dll";
        var fileName = Path.GetFileNameWithoutExtension(assemblyPath);
        var directory = Path.GetDirectoryName(assemblyPath) ?? "";
        dllPath = Path.Combine(directory, fileName + dnneSuffix);
        return true;
    }

    public static unsafe bool TryGetDllPathNativeAOT([NotNullWhen(true)]out string? dllPath)
    {
        dllPath = null;
        bool receivedHandle = Kernel32.GetModuleHandleExW(
            Kernel32.GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | Kernel32.GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
            (nint)(delegate* unmanaged<int>)&Exports.DllRegisterServer,
            out nint moduleHandle);
        if (!receivedHandle)
            return false;

        char[] filePath = new char[256];
        int pathSize = Kernel32.GetModuleFileNameW(moduleHandle, filePath, filePath.Length);
        if (pathSize == 0)
            return false;

        dllPath = new string(filePath);
        return true;
    }
}

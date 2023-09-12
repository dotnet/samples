
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tutorial;

public static class FileUtils
{
    /// <summary>
    /// Gets the path to the NativeDll that the COM system should use to call the native exports
    /// </summary>
    public static unsafe bool TryGetDllPath([NotNullWhen(true)]out string? dllPath)
    {
        if (IsNativeAOT())
            return TryGetDllPathNativeAOT(out dllPath);

        return TryGetDllPathCoreCLR(out dllPath);
    }

    /// <summary>
    /// Returns true if the application is running as NativeAOT, or false otherwise
    /// </summary <summary>
    private static bool IsNativeAOT()
        => typeof(FileUtils).Assembly.Location == ""
            && !RuntimeFeature.IsDynamicCodeSupported;

    /// <summary>
    /// Gets the path to the NativeDll that the COM system should use to call the native exports when deploying to CoreCLR.
    /// Returns false if the application is deployed as a NativeAOT application.
    /// </summary>
    public static unsafe bool TryGetDllPathCoreCLR(out string? dllPath)
    {
        string assemblyPath = typeof(FileUtils).Assembly.Location;
        const string dnneSuffix = "NE.dll";
        var fileName = Path.GetFileNameWithoutExtension(assemblyPath);
        var directory = Path.GetDirectoryName(assemblyPath) ?? "";
        dllPath = Path.Combine(directory, fileName + dnneSuffix);
        return true;
    }

    /// <summary>
    /// Gets the path to the NativeDll that the COM system should use to call the native exports
    /// </summary>
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

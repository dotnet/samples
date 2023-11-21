using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tutorial;

public static class FileUtils
{
    /// <summary>
    /// Gets the path to the NativeDll that the COM system should use to call the native exports
    /// </summary>
    [UnconditionalSuppressMessage(category: "SingleFile", checkId: "IL3000" /* Avoid accessing Assembly file path when publishing as a single file */, Justification = "The code is aware the `Assembly.Location` may be an empty string and falls back to Win32 APIs")]
    public static unsafe bool TryGetDllPath([NotNullWhen(true)] out string? dllPath)
    {
        // Try using reflection to get the path to the native COM entry points
        dllPath = typeof(FileUtils).Assembly.Location;
        // Assembly.Location is an empty string in single file and NativeAOT
        // Fall back to Windows APIs if Location is empty string
        if (dllPath == "" && !TryGetDllPathWin32(out dllPath))
            return false;
        // Check if DNNE binary exists and return path to DNNE binary if it exists
        const string dnneSuffix = "NE.dll";
        string fileName = Path.GetFileNameWithoutExtension(dllPath);
        string directory = Path.GetDirectoryName(dllPath) ?? "";
        string dnnePath = Path.Combine(directory, fileName + dnneSuffix);
        if (File.Exists(dnnePath))
            dllPath = dnnePath;
        return true;
    }

    /// <summary>
    /// Gets the path to the dll that has the DllRegisterServer address using the Windows APIs
    /// </summary>
    public static unsafe bool TryGetDllPathWin32([NotNullWhen(true)] out string? dllPath)
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

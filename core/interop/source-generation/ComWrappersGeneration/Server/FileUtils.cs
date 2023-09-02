
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tutorial;

public static class FileUtils
{
    public static unsafe bool TryGetDllPath([NotNullWhen(true)]out string? dllPath)
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

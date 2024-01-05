using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

internal static partial class Kernel32
{
    /// <summary>
    /// Gets a module handle given an address from that module.
    /// <see href="https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-getmodulehandleexw" />
    /// </summary>
    [LibraryImport(nameof(Kernel32))]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetModuleHandleExW(int flags, nint pAddressInModule, out nint moduleHandle);

    /// <summary>
    /// Makes the second argument of <see cref="GetModuleHandleExW"/> interpreted as an address in the module rather than a string.
    /// </summary>
    public static int GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 0x00000004;

    /// <summary>
    /// Makes GetModuleHandleExW not increment the reference count, so we don't need to decrement it ourselves.
    /// </summary>
    public const int GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002;

    /// <summary>
    /// Gets the path to a module given the module handle.
    /// <see href="https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-getmodulefilenamew" />
    /// </summary>
    [LibraryImport(nameof(Kernel32), StringMarshalling = StringMarshalling.Utf16)]
    public static partial int GetModuleFileNameW(
        nint moduleHandle,
        [MarshalUsing(CountElementName = nameof(nSize)), Out] char[] FileName,
        int nSize);
}

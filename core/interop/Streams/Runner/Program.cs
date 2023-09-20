using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using IStream = InteropWithStream.IStream;

int hr;
byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

//
// Example using the C export style.
//

Console.WriteLine("--- Stream via C export style interop");
unsafe
{
    // The returned void* is a handle that is passed to/from the managed library.
    void* streamHandle = ManagedLibNE.C_CreateStream();
    Debug.Assert(streamHandle != null);

    fixed (byte* pb = bytes)
    {
        hr = ManagedLibNE.C_Stream_Write(streamHandle, bytes.Length, pb);
        Debug.Assert(hr == 0);
    }

    hr = ManagedLibNE.C_PrintStream(streamHandle);
    Debug.Assert(hr == 0);

    ManagedLibNE.C_DeleteStream(streamHandle);
}
Console.WriteLine();

// Transform test input for alternative output.
bytes = bytes.Reverse().ToArray();

//
// Example using the IStream export style.
//

Console.WriteLine("--- Stream via IStream style interop");
unsafe
{
    // In C/C++ this would be typed as IStream*.
    void* istreamRaw;
    hr = ManagedLibNE.IStream_CreateStream(&istreamRaw);
    Debug.Assert(hr == 0 && istreamRaw != null);

    // Managed type system work, unnecessary in C/C++.
    var istream = UniqueComInterfaceMarshaller<IStream>.ConvertToManaged(istreamRaw);
    Debug.Assert(istream != null);

    fixed (byte* pb = bytes)
    {
        // In C/C++ this would check the HRESULT and out value.
        istream.Write(pb, (uint)bytes.Length, out uint written);
        Debug.Assert(written == bytes.Length);
    }

    // Pass the IStream* back to the library.
    hr = ManagedLibNE.IStream_PrintStream(istreamRaw);
    Debug.Assert(hr == 0);

    // In C/C++ this would be handled by calling IUnknown::Release() or implicitly
    // through a smart pointer like CComPtr<T>.
    Marshal.Release((IntPtr)istreamRaw);
}

/// <summary>
/// In C/C++ calling these functions would normally be done via
/// OS APIs such as LoadLibrary/dlopen and GetProcAddress/dlsym.
/// </summary>
static unsafe partial class ManagedLibNE
{
    //
    // C export pattern
    //

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial void* C_CreateStream();

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial void C_DeleteStream(void* stream);

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial int C_PrintStream(void* stream);

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial int C_Stream_Read(void* streamMaybe, nint length, byte* data, nint* dataRead);

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial int C_Stream_Write(void* streamMaybe, nint length, byte* data);

    //
    // IStream pattern
    //

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial int IStream_CreateStream(void** istream);

    [LibraryImport(nameof(ManagedLibNE))]
    public static partial int IStream_PrintStream(void* istream);

    // See the COM generated IStream type - IStream.cs.
    // The IStream definition for C/C++ can be found in objidl.h
    // on Windows, but must be manually defined on non-Windows platforms.
}

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InteropWithStream;

/// <summary>
/// Stream data type projected as an opaque handle and interacted via C exports.
/// </summary>
public static unsafe partial class StreamExports
{
#if NETFRAMEWORK
    [DNNE.Export]
#else
    [UnmanagedCallersOnly]
#endif // !NETFRAMEWORK
    public static IntPtr C_CreateStream()
    {
        MemoryStream stream = new();
        return GCHandle.ToIntPtr(GCHandle.Alloc(stream));
    }

#if NETFRAMEWORK
    [DNNE.Export]
#else
    [UnmanagedCallersOnly]
#endif // !NETFRAMEWORK
    public static void C_DeleteStream(IntPtr streamRaw)
    {
        if (streamRaw != IntPtr.Zero)
        {
            GCHandle.FromIntPtr(streamRaw).Free();
        }
    }

#if NETFRAMEWORK
    [DNNE.Export]
#else
    [UnmanagedCallersOnly]
#endif // !NETFRAMEWORK
    public static int C_PrintStream(IntPtr streamRaw)
    {
        try
        {
            var stream = (MemoryStream)GCHandle.FromIntPtr(streamRaw).Target!;
            var data = stream.ToArray();
            const int row = 4;
            for (int i = 0; i < data.Length; ++i)
            {
                char delim = (i % row == row - 1) ? '\n' : ' ';
                Console.Write($"{data[i]}{delim}");
            }
            Console.WriteLine();
        }
        catch (Exception e)
        {
            return e.HResult;
        }
        return 0;
    }

    //
    // APIs for interacting with Streams via C export
    //

#if NETFRAMEWORK
    [DNNE.Export]
#else
    [UnmanagedCallersOnly]
#endif // !NETFRAMEWORK
    public static int C_Stream_Read(IntPtr streamRaw, nint length, byte* dataRaw, nint* dataRead)
    {
        Debug.Assert(length <= int.MaxValue);
        try
        {
            var stream = (Stream)GCHandle.FromIntPtr(streamRaw).Target!;
            Span<byte> data = new(dataRaw, (int)length);
#if NETFRAMEWORK
            var buffer = new byte[data.Length];
            *dataRead = (nint)stream.Read(buffer, 0, buffer.Length);
            buffer.CopyTo(data);
#else
            *dataRead = (nint)stream.Read(data);
#endif // !NETFRAMEWORK
        }
        catch (Exception e)
        {
            return e.HResult;
        }
        return 0;
    }

#if NETFRAMEWORK
    [DNNE.Export]
#else
    [UnmanagedCallersOnly]
#endif // !NETFRAMEWORK
    public static int C_Stream_Write(IntPtr streamRaw, nint length, byte* dataRaw)
    {
        Debug.Assert(length <= int.MaxValue);
        try
        {
            var stream = (Stream)GCHandle.FromIntPtr(streamRaw).Target!;
            ReadOnlySpan<byte> data = new(dataRaw, (int)length);
#if NETFRAMEWORK
            stream.Write(data.ToArray(), 0, data.Length);
#else
            stream.Write(data);
#endif // !NETFRAMEWORK
        }
        catch (Exception e)
        {
            return e.HResult;
        }
        return 0;
    }
}

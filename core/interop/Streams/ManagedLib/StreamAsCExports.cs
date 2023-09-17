﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InteropWithStream;

/// <summary>
/// Stream data type projected as an opaque handle and interacted via C exports.
/// </summary>
public static unsafe partial class StreamExports
{
    [UnmanagedCallersOnly]
    public static IntPtr C_CreateStream()
    {
        MemoryStream stream = new();
        return GCHandle.ToIntPtr(GCHandle.Alloc(stream));
    }

    [UnmanagedCallersOnly]
    public static void C_DeleteStream(IntPtr streamMaybe)
    {
        if (streamMaybe != IntPtr.Zero)
        {
            GCHandle.FromIntPtr(streamMaybe).Free();
        }
    }

    [UnmanagedCallersOnly]
    public static int C_PrintStream(IntPtr streamMaybe)
    {
        try
        {
            var stream = (MemoryStream)GCHandle.FromIntPtr(streamMaybe).Target!;
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

    [UnmanagedCallersOnly]
    public static int C_Stream_Read(IntPtr streamMaybe, nint length, byte* dataRaw)
    {
        Debug.Assert(length < int.MaxValue);
        try
        {
            var stream = (Stream)GCHandle.FromIntPtr(streamMaybe).Target!;
            Span<byte> data = new(dataRaw, (int)length);
            stream.Read(data);
        }
        catch (Exception e)
        {
            return e.HResult;
        }
        return 0;
    }

    [UnmanagedCallersOnly]
    public static int C_Stream_Write(IntPtr streamMaybe, nint length, byte* dataRaw)
    {
        Debug.Assert(length < int.MaxValue);
        try
        {
            var stream = (Stream)GCHandle.FromIntPtr(streamMaybe).Target!;
            ReadOnlySpan<byte> data = new(dataRaw, (int)length);
            stream.Write(data);
        }
        catch (Exception e)
        {
            return e.HResult;
        }
        return 0;
    }
}
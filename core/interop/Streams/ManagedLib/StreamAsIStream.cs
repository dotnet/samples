using System.Runtime.InteropServices;

#if NETFRAMEWORK
using IStream = System.Runtime.InteropServices.ComTypes.IStream;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;
#else
using System.Runtime.InteropServices.Marshalling;
using IStream = InteropWithStream.IStream;
using STATSTG = InteropWithStream.STATSTG;
#endif // !NETFRAMEWORK

namespace InteropWithStream;

/// <summary>
/// Stream data type projected as IStream instance.
/// </summary>
public static unsafe partial class StreamExports
{
#if NETFRAMEWORK
    [DNNE.Export]
#else
    [UnmanagedCallersOnly]
#endif // !NETFRAMEWORK
    public static int IStream_CreateStream(void** istream)
    {
        try
        {
            MemoryStream stream = new();
#if NETFRAMEWORK
            *istream = (void*)Marshal.GetIUnknownForObject(new MemoryStreamWrapper(stream));
#else
            *istream = ComInterfaceMarshaller<MemoryStreamWrapper>.ConvertToUnmanaged(new MemoryStreamWrapper(stream));
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
    public static int IStream_PrintStream(void* streamMaybe)
    {
        try
        {
#if NETFRAMEWORK
            var streamWrapper = (MemoryStreamWrapper)Marshal.GetObjectForIUnknown((IntPtr)streamMaybe);
#else
            var streamWrapper = ComInterfaceMarshaller<MemoryStreamWrapper>.ConvertToManaged(streamMaybe);
#endif // !NETFRAMEWORK
            if (streamWrapper is null)
            {
                throw new NotSupportedException();
            }

            var stream = streamWrapper.Stream;
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

#if NETFRAMEWORK
    [ComVisible(true)]
    sealed partial class MemoryStreamWrapper : IStream
    {
        public MemoryStreamWrapper(MemoryStream s)
        {
            this.Stream = s;
        }

        public MemoryStream Stream { get; private set; }

        // ISequentialStream
        public void Read([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out] byte[] pv, int cb, IntPtr pcbRead)
        {
            Marshal.WriteInt32(pcbRead, Stream.Read(pv, 0, pv.Length));
        }

        public void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten)
        {
            Stream.Write(pv, 0, pv.Length);
            Marshal.WriteInt32(pcbWritten, pv.Length);
        }

        // IStream
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
            => throw new NotImplementedException();
        public void SetSize(long libNewSize)
            => throw new NotImplementedException();
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
            => throw new NotImplementedException();
        public void Commit(int grfCommitFlags)
            => throw new NotImplementedException();
        public void Revert()
            => throw new NotImplementedException();
        public void LockRegion(long libOffset, long cb, int dwLockType)
            => throw new NotImplementedException();
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
            => throw new NotImplementedException();
        public void Stat(out STATSTG pstatstg, int grfStatFlag)
            => throw new NotImplementedException();
        public void Clone(out IStream ppstm)
            => throw new NotImplementedException();
    }
#else
    [GeneratedComClass]
    sealed partial class MemoryStreamWrapper : IStream
    {
        public MemoryStreamWrapper(MemoryStream s)
        {
            this.Stream = s;
        }

        public MemoryStream Stream { get; private set; }

        // ISequentialStream
        public void Read([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out] byte[] pv, uint cb, out uint pcbRead)
        {
            Span<byte> data = new(pv);
            pcbRead = (uint)Stream.Read(data);
        }

        public void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, uint cb, out uint pcbWritten)
        {
            ReadOnlySpan<byte> data = new(pv);
            Stream.Write(data);
            pcbWritten = (uint)pv.Length;
        }

        // IStream
        public void Seek(long dlibMove, uint dwOrigin, ulong plibNewPosition)
            => throw new NotImplementedException();
        public void SetSize(ulong libNewSize)
            => throw new NotImplementedException();
        public void CopyTo(IStream pstm, ulong cb, out ulong pcbRead, out ulong pcbWritten)
            => throw new NotImplementedException();
        public void Commit(uint grfCommitFlags)
            => throw new NotImplementedException();
        public void Revert()
            => throw new NotImplementedException();
        public void LockRegion(ulong libOffset, ulong cb, uint dwLockType)
            => throw new NotImplementedException();
        public void UnlockRegion(ulong libOffset, ulong cb, uint dwLockType)
            => throw new NotImplementedException();
        public void Stat(out STATSTG pstatstg, uint grfStatFlag)
            => throw new NotImplementedException();
        public void Clone(out IStream ppstm)
            => throw new NotImplementedException();
    }
#endif // !NETFRAMEWORK
}
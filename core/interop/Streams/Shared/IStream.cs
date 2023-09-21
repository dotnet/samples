// Represents the IStream API surface area in .NET 8+.

#if !NETFRAMEWORK

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace InteropWithStream;

[GeneratedComInterface]
[Guid("0C733A30-2A1C-11CE-ADE5-00AA0044773D")]
internal unsafe partial interface ISequentialStream
{
    void Read(byte* pv, uint cb, out uint pcbRead);
    void Write(byte* pv, uint cb, out uint pcbWritten);
}

[GeneratedComInterface]
[Guid("0000000c-0000-0000-C000-000000000046")]
internal unsafe partial interface IStream : ISequentialStream
{
    void Seek(long dlibMove, uint dwOrigin, ulong plibNewPosition);
    void SetSize(ulong libNewSize);
    void CopyTo(IStream pstm, ulong cb, out ulong pcbRead, out ulong pcbWritten);
    void Commit(uint grfCommitFlags);
    void Revert();
    void LockRegion(ulong libOffset, ulong cb, uint dwLockType);
    void UnlockRegion(ulong libOffset, ulong cb, uint dwLockType);
    void Stat(out STATSTG pstatstg, uint grfStatFlag);
    void Clone(out IStream ppstm);
}

[NativeMarshalling(typeof(STATSTGMarshaller))]
internal struct STATSTG
{
    public string? pwcsName;
    public uint type;
    public ulong cbSize;
    public FILETIME mtime;
    public FILETIME ctime;
    public FILETIME atime;
    public uint grfMode;
    public uint grfLocksSupported;
    public Guid clsid;
    public uint grfStateBits;
    public uint reserved;
}

[CustomMarshaller(typeof(STATSTG), MarshalMode.Default, typeof(STATSTGMarshaller))]
internal static class STATSTGMarshaller
{
    public static STATSTGUnmanaged ConvertToUnmanaged(STATSTG managed)
    {
        return new STATSTGUnmanaged()
        {
            pwcsName = Marshal.StringToCoTaskMemUni(managed.pwcsName),
            type = managed.type,
            cbSize = managed.cbSize,
            mtime = managed.mtime,
            ctime = managed.ctime,
            atime = managed.atime,
            grfMode = managed.grfMode,
            grfLocksSupported = managed.grfLocksSupported,
            clsid = managed.clsid,
            grfStateBits = managed.grfStateBits,
            reserved = managed.reserved,
        };
    }

    public static STATSTG ConvertToManaged(STATSTGUnmanaged unmanaged)
    {
        return new STATSTG()
        {
            pwcsName = Marshal.PtrToStringUni(unmanaged.pwcsName),
            type = unmanaged.type,
            cbSize = unmanaged.cbSize,
            mtime = unmanaged.mtime,
            ctime = unmanaged.ctime,
            atime = unmanaged.atime,
            grfMode = unmanaged.grfMode,
            grfLocksSupported = unmanaged.grfLocksSupported,
            clsid = unmanaged.clsid,
            grfStateBits = unmanaged.grfStateBits,
            reserved = unmanaged.reserved,
        };
    }

    public static void Free(STATSTGUnmanaged unmanaged)
        => Marshal.FreeCoTaskMem(unmanaged.pwcsName);

    public struct STATSTGUnmanaged
    {
        public IntPtr pwcsName;
        public uint type;
        public ulong cbSize;
        public FILETIME mtime;
        public FILETIME ctime;
        public FILETIME atime;
        public uint grfMode;
        public uint grfLocksSupported;
        public Guid clsid;
        public uint grfStateBits;
        public uint reserved;
    }
}

#endif // !NETFRAMEWORK
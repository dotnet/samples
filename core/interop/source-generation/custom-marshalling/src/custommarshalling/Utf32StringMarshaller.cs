using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomMarshalling
{
    /// <summary>
    /// Marshaller for strings using UTF-32.
    /// </summary>
    /// <remarks>
    /// Attributes are specified such that <see cref="ManagedToUnmanagedIn"/> is used when marshalling by-value and <c>in</c>
    /// parameters in managed-to-unmanaged scenarios (like P/Invoke) and <see cref="Utf32StringMarshaller"/> is used in all
    /// other marshalling scenarios.
    /// </remarks>
    [CustomMarshaller(typeof(string), MarshalMode.Default, typeof(Utf32StringMarshaller))]
    [CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
    internal static unsafe class Utf32StringMarshaller
    {
        public static uint* ConvertToUnmanaged(string? managed)
        {
            if (managed is null)
                return null;

            int exactByteCount = checked(Encoding.UTF32.GetByteCount(managed) + 4); // + 4 for null terminator
            uint* mem = (uint*)NativeMemory.Alloc((nuint)exactByteCount);
            Span<byte> buffer = new(mem, exactByteCount);

            int byteCount = Encoding.UTF32.GetBytes(managed, buffer);
            buffer[byteCount..].Fill(0); // null-terminate
            return mem;
        }

        public static string? ConvertToManaged(uint* unmanaged)
        {
            if (unmanaged == null)
                return null;

            var toSearch = new Span<uint>(unmanaged, int.MaxValue);
            int len = toSearch.IndexOf((uint)0);

            return Encoding.UTF32.GetString((byte*)unmanaged, len * sizeof(uint));
        }

        public static void Free(uint* unmanaged)
            => NativeMemory.Free(unmanaged);

        /// <summary>
        /// Stateful marshaller for managed to unmanaged that enables the caller-allocated buffer optimization.
        /// </summary>
        public ref struct ManagedToUnmanagedIn
        {
            public static int BufferSize => 0x100;

            private uint* _unmanagedValue;
            private bool _allocated;

            public void FromManaged(string? managed, Span<byte> buffer)
            {
                _allocated = false;

                if (managed is null)
                {
                    _unmanagedValue = null;
                    return;
                }

                int exactByteCount = checked(Encoding.UTF32.GetByteCount(managed) + 4); // + 4 for null terminator
                if (exactByteCount > buffer.Length)
                {
                    buffer = new Span<byte>((byte*)NativeMemory.Alloc((nuint)exactByteCount), exactByteCount);
                    _allocated = true;
                }

                _unmanagedValue = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer));

                int byteCount = Encoding.UTF32.GetBytes(managed, buffer);
                buffer[byteCount..].Fill(0); // null-terminate
            }

            public uint* ToUnmanaged() => _unmanagedValue;

            public void Free()
            {
                if (_allocated)
                    NativeMemory.Free(_unmanagedValue);
            }
        }
    }
}

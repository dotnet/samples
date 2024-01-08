using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace CustomMarshalling
{
    /// <summary>
    /// User-defined type that specifies a marshaller to be used by default.
    /// </summary>
    [NativeMarshalling(typeof(ErrorDataMarshaller))]
    internal struct ErrorData
    {
        public int Code;
        public bool IsFatalError;
        public string? Message;

        public void Print()
        {
            Console.WriteLine(nameof(ErrorData));
            Console.WriteLine($"{nameof(Code)}         : {Code}");
            Console.WriteLine($"{nameof(IsFatalError)} : {IsFatalError}");
            Console.WriteLine($"{nameof(Message)}      : {Message}");
        }
    }

    /// <summary>
    /// Marshaller for <see cref="ErrorData"/>.
    /// </summary>
    /// <remarks>
    /// Attributes are specified such that <see cref="ErrorDataMarshaller"/> is used when marshalling by-value and in
    /// parameters in managed-to-unmanaged scenarios (like P/Invoke), <see cref="ErrorDataMarshaller.Out"/> is used when
    /// returning or marshalling an element of an array out, and <see cref="ErrorDataMarshaller.ThrowOnFatalErrorOut"/> is
    /// used when returning or marshalling out parameters in managed-to-unmanaged scenarios (like P/Invoke). All other
    /// marshalling scenarios are unsupported.
    /// </remarks>
    [CustomMarshaller(typeof(ErrorData), MarshalMode.ManagedToUnmanagedIn, typeof(ErrorDataMarshaller))]
    [CustomMarshaller(typeof(ErrorData), MarshalMode.ManagedToUnmanagedOut, typeof(ThrowOnFatalErrorOut))]
    [CustomMarshaller(typeof(ErrorData), MarshalMode.ElementOut, typeof(Out))]
    internal static unsafe class ErrorDataMarshaller
    {
        /// <summary>
        /// Unmanaged representation of <see cref="ErrorData"/>
        /// </summary>
        internal struct ErrorDataUnmanaged
        {
            public int Code;
            public byte IsFatal;
            public uint* Message;
        }

        public static ErrorDataUnmanaged ConvertToUnmanaged(ErrorData managed)
        {
            return new ErrorDataUnmanaged
            {
                Code = managed.Code,
                IsFatal = (byte)(managed.IsFatalError ? 1 : 0),
                Message = Utf32StringMarshaller.ConvertToUnmanaged(managed.Message),
            };
        }

        public static void Free(ErrorDataUnmanaged unmanaged)
            => Utf32StringMarshaller.Free(unmanaged.Message);

        public static class Out
        {
            public static ErrorData ConvertToManaged(ErrorDataUnmanaged unmanaged)
            {
                return new ErrorData
                {
                    Code = unmanaged.Code,
                    IsFatalError = unmanaged.IsFatal != 0,
                    Message = Utf32StringMarshaller.ConvertToManaged(unmanaged.Message)
                };
            }

            public static ErrorDataUnmanaged ConvertToUnmanaged(ErrorData managed)
                => ErrorDataMarshaller.ConvertToUnmanaged(managed);

            public static void Free(ErrorDataUnmanaged unmanaged)
                => ErrorDataMarshaller.Free(unmanaged);
        }

        public static class ThrowOnFatalErrorOut
        {
            public static ErrorData ConvertToManaged(ErrorDataUnmanaged unmanaged)
            {
                ErrorData data = Out.ConvertToManaged(unmanaged);
                if (data.IsFatalError)
                    throw new ExternalException(data.Message, data.Code);

                return data;
            }

            public static void Free(ErrorDataUnmanaged unmanaged)
                => ErrorDataMarshaller.Free(unmanaged);
        }
    }
}

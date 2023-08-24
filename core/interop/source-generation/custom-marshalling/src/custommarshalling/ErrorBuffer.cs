using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace CustomMarshalling
{
    [NativeMarshalling(typeof(ErrorBufferMarshaller))]
    [InlineArray(4)]
    public struct ErrorBuffer
    {
        private ErrorData _data;
    }
    
    [CustomMarshaller(typeof(ErrorBuffer), MarshalMode.ManagedToUnmanagedIn, typeof(ErrorBufferMarshaller))]
    public static class ErrorBufferMarshaller
    {
        [InlineArray(4)]
        public struct ErrorBufferUnmanaged
        {
            private ErrorDataMarshaller.ErrorDataUnmanaged _data;
        }

        public static ErrorBufferUnmanaged ConvertToUnmanaged(ErrorBuffer managed)
        {
            ErrorBufferUnmanaged unmanaged = new();
            for (int i = 0; i < 4; i++)
            {
                unmanaged[i] = ErrorDataMarshaller.ConvertToUnmanaged(managed[i]);
            }
            return unmanaged;
        }
    }
}
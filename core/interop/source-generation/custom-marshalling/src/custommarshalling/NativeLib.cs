using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace CustomMarshalling
{
    internal partial class NativeLib
    {
        private const string LibName = "nativelib";

        [LibraryImport(LibName)]
        internal static partial void PrintString([MarshalUsing(typeof(Utf32StringMarshaller))] string s);

        [LibraryImport(LibName, StringMarshallingCustomType = typeof(Utf32StringMarshaller))]
        internal static partial void PrintStrings(string[] s, int count);

        [LibraryImport(LibName, StringMarshallingCustomType = typeof(Utf32StringMarshaller))]
        internal static partial string ReverseString([MarshalUsing(typeof(Utf32StringMarshaller))] string s);

        [LibraryImport(LibName)]
        internal static partial void ReverseStringInPlace([MarshalUsing(typeof(Utf32StringMarshaller))] ref string s);

        [LibraryImport(LibName)]
        internal static partial void PrintErrorData(ErrorData errorData);

        [LibraryImport(LibName)]
        internal static partial ErrorData GetFatalErrorIfNegative(int code);

        [LibraryImport(LibName)]
        [return: MarshalUsing(CountElementName = "len")]
        internal static partial ErrorData[] GetErrors(int[] codes, int len);

        [LibraryImport(LibName)]
        internal static partial void GetErrorCodes(ErrorBuffer buffer, int[] codes);
    }
}

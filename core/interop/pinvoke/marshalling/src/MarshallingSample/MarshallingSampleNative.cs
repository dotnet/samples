using System;
using System.Runtime.InteropServices;

namespace MarshallingSample
{
    internal static class MarshallingSampleNative
    {
        // String marshalling APIs
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int CountUtf8Characters(string value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int CountUtf16Characters(string value);

#if WINDOWS
        private const CharSet PlatformSpecificCharSet = CharSet.Unicode;
#else
        private const CharSet PlatformSpecificCharSet = CharSet.Ansi;
#endif
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true, CharSet = PlatformSpecificCharSet)]
        public static extern int CountPlatformSpecificCharacters(string value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int GetStringIntoCalleeAllocatedBuffer(out string value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern string ReturnStringIntoCalleeAllocatedBuffer();

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int GetStringIntoCallerAllocatedBuffer([In, Out, MarshalAs(UnmanagedType.LPArray)] char[] buffer, ref int bufferSize);


        // Int32 marshalling APIs
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int AcceptInt32Argument([In] int value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int AcceptInt32ByRefArgument([In] ref int value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern void GetInt32OutArgument(out int value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern void ModifyInt32InOutArgument(ref int value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int ReturnInt32Argument(int value);


        // Boolean marshalling APIs
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int AcceptBOOLArgument([In] bool value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int AcceptBOOLByRefArgument([In] ref bool value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern void GetBOOLOutArgument(out bool value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern void ModifyBOOLInOutArgument(ref bool value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern bool ReturnBOOLArgument(bool value);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int CountTrueValues(bool value1, [MarshalAs(UnmanagedType.U1)] bool value2, [MarshalAs(UnmanagedType.I1)] bool value3);

        // Marshalling as VariantBool is only supported on Windows
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int CountTrueValuesWindows(bool value1, [MarshalAs(UnmanagedType.U1)] bool value2, [MarshalAs(UnmanagedType.I1)] bool value3, [MarshalAs(UnmanagedType.VariantBool)] bool value4);


        // Enum marshalling APIs
        [Flags]
        public enum EnumFlags
        {
            None = 0,
            A = 1,
            B = 2,
            C = 4
        }

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int CountEnumFlags(EnumFlags enumValue);


        // Numeric marshalling
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern byte SumBytes(byte inValue, [In] ref byte inRef, ref byte inOutRef, out byte outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern sbyte SumSBytes(sbyte inValue, [In] ref sbyte inRef, ref sbyte inOutRef, out sbyte outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern ushort SumUShorts(ushort inValue, [In] ref ushort inRef, ref ushort inOutRef, out ushort outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern short SumShorts(short inValue, [In] ref short inRef, ref short inOutRef, out short outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern uint SumUInts(uint inValue, [In] ref uint inRef, ref uint inOutRef, out uint outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int SumInts(int inValue, [In] ref int inRef, ref int inOutRef, out int outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern ulong SumULongs(ulong inValue, [In] ref ulong inRef, ref ulong inOutRef, out ulong outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern long SumLongs(long inValue, [In] ref long inRef, ref long inOutRef, out long outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern float SumFloats(float inValue, [In] ref float inRef, ref float inOutRef, out float outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern double SumDoubles(double inValue, [In] ref double inRef, ref double inOutRef, out double outRef);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern decimal SumDecimals(decimal inValue, [In] ref decimal inRef, ref decimal inOutRef, out decimal outRef);


        // GUID marshalling
        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern int CompareGuids(Guid a, Guid b);

        [DllImport(nameof(MarshallingSampleNative), ExactSpelling = true)]
        public static extern Guid CountZeroGuids(Guid inValue, [MarshalAs(UnmanagedType.LPStruct)] Guid inRefA, [In] ref Guid inRefB, ref Guid inOutRef, out Guid outRef);
    }
}

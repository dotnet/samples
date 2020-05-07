using System;
using System.Runtime.InteropServices;

namespace MarshalingSample
{
    public class MarshalingSample
    {
        public static int Main(string[] args)
        {
            StringMarshaling.Run();
            Int32Marshaling.Run();
            BooleanMarshaling.Run();
            EnumMarshaling.Run();
            NumericMarshaling.Run();
            GuidMarshaling.Run();

            return 0;
        }
    }
}

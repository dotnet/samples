using System;
using System.Runtime.InteropServices;

namespace MarshallingSample
{
    public class MarshallingSample
    {
        public static int Main(string[] args)
        {
            StringMarshalling.Run();
            Int32Marshalling.Run();
            BooleanMarshalling.Run();
            EnumMarshalling.Run();
            NumericMarshalling.Run();
            GuidMarshalling.Run();

            return 0;
        }
    }
}

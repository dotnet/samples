using System;

namespace MarshalingSample
{
    class StringMarshaling
    {
        public static void Run()
        {
            Console.WriteLine("----- String marshaling samples -----");

            MarshalingSampleNative.CountBytesInString(null); // returns -1
            MarshalingSampleNative.CountBytesInString("some string"); // returns 11
        }
    }
}

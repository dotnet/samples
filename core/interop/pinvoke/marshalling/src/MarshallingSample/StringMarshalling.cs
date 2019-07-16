using System;

namespace MarshallingSample
{
    class StringMarshalling
    {
        public static void Run()
        {
            Console.WriteLine("----- String marshalling samples -----");

            MarshallingSampleNative.CountBytesInString(null); // returns -1
            MarshallingSampleNative.CountBytesInString("some string"); // returns 11
        }
    }
}

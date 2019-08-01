using System;

namespace MarshallingSample
{
    class EnumMarshalling
    {
        public static void Run()
        {
            Console.WriteLine("----- Enum marshalling samples -----");

            // return 2
            int count = MarshallingSampleNative.CountEnumFlags(
                MarshallingSampleNative.EnumFlags.A | MarshallingSampleNative.EnumFlags.C);
        }
    }
}

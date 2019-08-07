using System;

namespace MarshalingSample
{
    class EnumMarshaling
    {
        public static void Run()
        {
            Console.WriteLine("----- Enum marshaling samples -----");

            // return 2
            int count = MarshalingSampleNative.CountEnumFlags(
                MarshalingSampleNative.EnumFlags.A | MarshalingSampleNative.EnumFlags.C);
        }
    }
}

using System;

namespace MarshallingSample
{
    class GuidMarshalling
    {
        public static void Run()
        {
            Console.WriteLine("----- GUID marshalling samples -----");

            Guid a = new Guid("11111111-2222-3333-4444-556677889900");
            Guid b = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeff77889900");

            MarshallingSampleNative.CompareGuids(a, b); // returns 0
            MarshallingSampleNative.CompareGuids(a, a); // returns 1

            Guid inOutRef = Guid.Empty;
            Guid outRef;
            Guid result = MarshallingSampleNative.CountZeroGuids(a, Guid.Empty, ref b, ref inOutRef, out outRef);
        }
    }
}

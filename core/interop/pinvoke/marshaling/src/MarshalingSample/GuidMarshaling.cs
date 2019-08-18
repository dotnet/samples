using System;

namespace MarshalingSample
{
    class GuidMarshaling
    {
        public static void Run()
        {
            Console.WriteLine("----- GUID marshaling samples -----");

            Guid a = new Guid("11111111-2222-3333-4444-556677889900");
            Guid b = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeff77889900");

            MarshalingSampleNative.CompareGuids(a, b); // returns 0
            MarshalingSampleNative.CompareGuids(a, a); // returns 1

            Guid inOutRef = Guid.Empty;
            Guid outRef;
            Guid result = MarshalingSampleNative.CountZeroGuids(a, Guid.Empty, ref b, ref inOutRef, out outRef);
        }
    }
}

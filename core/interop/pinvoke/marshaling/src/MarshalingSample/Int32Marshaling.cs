using System;

namespace MarshalingSample
{
    class Int32Marshaling
    {
        public static void Run()
        {
            Console.WriteLine("----- Int32 marshaling samples -----");

            // Always start with the same value.
            const int initialValue = 7;
            int value;

            // Pass Int32 argument by value.
            value = initialValue;
            MarshalingSampleNative.AcceptInt32Argument(value); // returns 7

            // Pass Int32 argument by refernece.
            value = initialValue;
            MarshalingSampleNative.AcceptInt32ByRefArgument(ref value); // returns 7

            // Get Int32 out parameter.
            MarshalingSampleNative.GetInt32OutArgument(out value); // sets value to 9

            // Pass Int32 in-out argument by reference.
            value = initialValue;
            MarshalingSampleNative.ModifyInt32InOutArgument(ref value);

            // Return Int32 value.
            value = MarshalingSampleNative.ReturnInt32Argument(initialValue);
        }
    }
}

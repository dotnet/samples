using System;
using System.Runtime.InteropServices;

namespace MarshalingSample
{
    class BooleanMarshaling
    {
        public static void Run()
        {
            Console.WriteLine("----- Boolean marshaling samples -----");

            const bool initialValue = true;
            bool value;

            // Pass boolean by value and masrshal it as "Windows" BOOL - which is 32bit int.
            value = initialValue;
            MarshalingSampleNative.AcceptBOOLArgument(value);  // returns 1
            MarshalingSampleNative.AcceptBOOLArgument(false);  // returns 0

            // Pass boolean by reference
            value = initialValue;
            MarshalingSampleNative.AcceptBOOLByRefArgument(ref value);

            // Get boolean out parameter
            MarshalingSampleNative.GetBOOLOutArgument(out value); // sets value to false

            // Pass boolean in-out argument by reference.
            value = initialValue;
            MarshalingSampleNative.ModifyBOOLInOutArgument(ref value); // sets value to false

            // Return boolean value.
            value = MarshalingSampleNative.ReturnBOOLArgument(value);

            // Marshal boolean value in various ways
            int count;
            value = initialValue;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                count = MarshalingSampleNative.CountTrueValuesWindows(!value, value, !value, value); // returns 2
            }
            else
            {
                count = MarshalingSampleNative.CountTrueValues(!value, value, !value); // returns 1
            }
        }
    }
}

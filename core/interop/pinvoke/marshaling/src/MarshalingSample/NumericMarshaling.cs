using System;

namespace MarshalingSample
{
    class NumericMarshaling
    {
        public static void Run()
        {
            Console.WriteLine("----- Numeric marshaling samples -----");

            { // byte
                // Result is 6, both inOutRef and outRef are also set to 6
                byte inRef = 2, inOutRef = 3, outRef;
                byte result = MarshalingSampleNative.SumBytes(1, ref inRef, ref inOutRef, out outRef);
            }

            { // sbyte
                // Result is -6, both inOutRef and outRef are also set to -6
                sbyte inRef = -2, inOutRef = -3, outRef;
                sbyte result = MarshalingSampleNative.SumSBytes(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // ushort
                // Result is 6, both inOutRef and outRef are also set to 6
                ushort inRef = 2, inOutRef = 3, outRef;
                ushort result = MarshalingSampleNative.SumUShorts(1, ref inRef, ref inOutRef, out outRef);
            }

            { // short
                // Result is -6, both inOutRef and outRef are also set to -6
                short inRef = -2, inOutRef = -3, outRef;
                short result = MarshalingSampleNative.SumShorts(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // uint
                // Result is 6, both inOutRef and outRef are also set to 6
                uint inRef = 2, inOutRef = 3, outRef;
                uint result = MarshalingSampleNative.SumUInts(1, ref inRef, ref inOutRef, out outRef);
            }

            { // int
                // Result is -6, both inOutRef and outRef are also set to -6
                int inRef = -2, inOutRef = -3, outRef;
                int result = MarshalingSampleNative.SumInts(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // ulong
                // Result is 6, both inOutRef and outRef are also set to 6
                ulong inRef = 2, inOutRef = 3, outRef;
                ulong result = MarshalingSampleNative.SumULongs(1, ref inRef, ref inOutRef, out outRef);
            }

            { // long
                // Result is -6, both inOutRef and outRef are also set to -6
                long inRef = -2, inOutRef = -3, outRef;
                long result = MarshalingSampleNative.SumLongs(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // float
                // Result is roughly 0.123, both inOutRef and outRef are also set to roughly 0.123
                float inRef = 0.02f, inOutRef = 0.003f, outRef;
                float result = MarshalingSampleNative.SumFloats(0.1f, ref inRef, ref inOutRef, out outRef);
            }

            { // double
                // Result is roughly 0.123, both inOutRef and outRef are also set to roughly 0.123
                double inRef = 0.02f, inOutRef = 0.003f, outRef;
                double result = MarshalingSampleNative.SumDoubles(0.1f, ref inRef, ref inOutRef, out outRef);
            }

            { // decimals
                // Result is 6, both inOutRef and outRef are also set to 6
                decimal inRef = 2, inOutRef = 3, outRef;
                decimal result = MarshalingSampleNative.SumDecimals(1m, ref inRef, ref inOutRef, out outRef);
            }
        }
    }
}

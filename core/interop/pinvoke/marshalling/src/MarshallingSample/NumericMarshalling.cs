using System;

namespace MarshallingSample
{
    class NumericMarshalling
    {
        public static void Run()
        {
            Console.WriteLine("----- Numeric marshalling samples -----");

            { // byte
                // Result is 6, both inOutRef and outRef are also set to 6
                byte inRef = 2, inOutRef = 3, outRef;
                byte result = MarshallingSampleNative.SumBytes(1, ref inRef, ref inOutRef, out outRef);
            }

            { // sbyte
                // Result is -6, both inOutRef and outRef are also set to -6
                sbyte inRef = -2, inOutRef = -3, outRef;
                sbyte result = MarshallingSampleNative.SumSBytes(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // ushort
                // Result is 6, both inOutRef and outRef are also set to 6
                ushort inRef = 2, inOutRef = 3, outRef;
                ushort result = MarshallingSampleNative.SumUShorts(1, ref inRef, ref inOutRef, out outRef);
            }

            { // short
                // Result is -6, both inOutRef and outRef are also set to -6
                short inRef = -2, inOutRef = -3, outRef;
                short result = MarshallingSampleNative.SumShorts(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // uint
                // Result is 6, both inOutRef and outRef are also set to 6
                uint inRef = 2, inOutRef = 3, outRef;
                uint result = MarshallingSampleNative.SumUInts(1, ref inRef, ref inOutRef, out outRef);
            }

            { // int
                // Result is -6, both inOutRef and outRef are also set to -6
                int inRef = -2, inOutRef = -3, outRef;
                int result = MarshallingSampleNative.SumInts(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // ulong
                // Result is 6, both inOutRef and outRef are also set to 6
                ulong inRef = 2, inOutRef = 3, outRef;
                ulong result = MarshallingSampleNative.SumULongs(1, ref inRef, ref inOutRef, out outRef);
            }

            { // long
                // Result is -6, both inOutRef and outRef are also set to -6
                long inRef = -2, inOutRef = -3, outRef;
                long result = MarshallingSampleNative.SumLongs(-1, ref inRef, ref inOutRef, out outRef);
            }

            { // float
                // Result is roughly 0.123, both inOutRef and outRef are also set to roughly 0.123
                float inRef = 0.02f, inOutRef = 0.003f, outRef;
                float result = MarshallingSampleNative.SumFloats(0.1f, ref inRef, ref inOutRef, out outRef);
            }

            { // double
                // Result is roughly 0.123, both inOutRef and outRef are also set to roughly 0.123
                double inRef = 0.02f, inOutRef = 0.003f, outRef;
                double result = MarshallingSampleNative.SumDoubles(0.1f, ref inRef, ref inOutRef, out outRef);
            }

            { // decimals
                // Result is 6, both inOutRef and outRef are also set to 6
                decimal inRef = 2, inOutRef = 3, outRef;
                decimal result = MarshallingSampleNative.SumDecimals(1m, ref inRef, ref inOutRef, out outRef);
            }
        }
    }
}

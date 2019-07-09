using System;
using System.Buffers;

namespace MarshallingSample
{
    class StringMarshalling
    {
        public static void Run()
        {
            Console.WriteLine("----- String marshalling samples -----");

            MarshallingSampleNative.CountUtf8Characters(null); // returns -1
            MarshallingSampleNative.CountUtf8Characters("some string"); // returns 11

            MarshallingSampleNative.CountUtf16Characters(null); // returns -1
            MarshallingSampleNative.CountUtf16Characters("some string"); // returns 11

            // This is an example of a method which will marshal the string differently 
            // for each platform. This is because typically Windows native APIs are UTF16
            // while Linux/Mac native APIs are UTF8.
            MarshallingSampleNative.CountPlatformSpecificCharacters(null); // returns -1
            MarshallingSampleNative.CountPlatformSpecificCharacters("some string"); // returns 11

            // Get a string from native via a callee allocated buffer through out parameter.
            {
                // The marshalling layer will copy the out string into the managed object
                // and then free the buffer.
                string outValue;
                MarshallingSampleNative.GetStringIntoCalleeAllocatedBuffer(out outValue);
            }

            // Get a string from native via a callee allocated buffer returned from the function.
            {
                // The marshalling layer will copy the returned string into the managed object
                // and then free the buffer.
                string outValue = MarshallingSampleNative.ReturnStringIntoCalleeAllocatedBuffer();
            }

            // Get a string from native via a caller allocated buffer.
            {
                // This pattern is typical for Win32 APIs for example.
                // First call the API to determine the size of the buffer needed.
                int bufferSize = 0;
                MarshallingSampleNative.GetStringIntoCallerAllocatedBuffer(null, ref bufferSize);

                // Get the buffer from ArrayPool (helpful for frequent calls)
                char[] buffer = ArrayPool<char>.Shared.Rent((int)bufferSize);

                // Call the API again with the buffer to get the string value
                MarshallingSampleNative.GetStringIntoCallerAllocatedBuffer(buffer, ref bufferSize);

                // Create a string value out of the returned buffer
                string stringValue = new string(buffer, 0, bufferSize - 1);

                // And now free/return the buffer
                ArrayPool<char>.Shared.Return(buffer);
            }
        }
    }
}

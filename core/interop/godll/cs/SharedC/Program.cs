using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SharedC
{
    unsafe class Program
    {
        // shared library.
        struct GoString
        {
            public IntPtr p;
            public Int64 n;
        }

        // GoSlice represents
        // a slice in golang so that we
        // can pass slices to the go external
        // shared library.
        struct GoSlice
        {
            public IntPtr data;
            public Int64 len;
            public Int64 cap;
        }

        static void Main(string[] args)
        {
            // CGO checks
            // Go code may not store a Go pointer in C memory. 
            // C code may store Go pointers in C memory, 
            // subject to the rule above: it must stop storing the Go pointer when 
            // the C function returns.

            Environment.SetEnvironmentVariable("GODEBUG", "cgocheck=2");
            
            // define parameters

            int a     = 10;
            int b     = 2;
            double x  = 100;
            Int64[] t = new Int64[] { 35, 56, 1, 3, 2, 88, 14 };

            // Allocate unmanaged memory for
            // the GoSlice

            int n      = t.Length;
            GCHandle h = GCHandle.Alloc(t, GCHandleType.Pinned);
            GoSlice gs = new GoSlice
            {
                data = h.AddrOfPinnedObject(),
                cap  = n,
                len  = n
            };

            // Allocate unmanaged memory for the
            // GoString

            string msg = "I am the Hal 9000";
            GoString s = new GoString
            {  
                p = Marshal.StringToHGlobalAnsi(msg),
                n = msg.Length
            };

            // call the external functions

            int addResult       = GoMath.Add(a, b);
            int subResult       = GoMath.Sub(a, b);
            double cosineResult = GoMath.Cosine(x);
            int sumResult       = GoMath.Sum(gs);
            var helloResult     = GoHello.HelloWorld(s);
            GoMath.Sort(gs);

            // Read the Sorted GoSlice

            n           = (int) gs.len;
            Int64[] arr = new Int64[n];
            for (int i  = 0; i < n; i++)
            {
                arr[i]  = Marshal.ReadInt64(gs.data, i*Marshal.SizeOf(typeof(Int64)));
             }

            // Read the size of the data returned by HelloWorld
            // The size is an int32, so we read 4 bytes

            byte* buf       = (byte*)helloResult;
            byte[] lenBytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                lenBytes[i] = *buf++;
            }

            // Read the result itself

            n               = BitConverter.ToInt32(lenBytes, 0);
            int j           = 0;
            byte[] bytes    = new byte[n];
            for (int i = 0; i < n; i++)
            {

                // Skip the first 4 bytes because
                // they hold the size

                if (i < 4)
                {
                    *buf++ = 0;
                }
                else
                {
                    bytes[j] = *buf++;
                    j++;
                }   
            }

            // Print results

            Console.WriteLine(
                "#########################################" +
                "\n### .NET Calling Shared-C Golang .dll ###" +
                "\n#########################################\n"
            );
            Console.WriteLine($"HelloWorld: {Encoding.UTF8.GetString(bytes)}");
            Console.WriteLine($"Add: {addResult}");
            Console.WriteLine($"Sub: {subResult}");
            Console.WriteLine($"Cos: {cosineResult}");
            Console.WriteLine($"Sum: {sumResult}");
            Console.WriteLine(Int64ArrayToString(arr));

            // free up allocated unmanaged memory

            if (h.IsAllocated)
            {
                h.Free();
            }
        }

        // Prints an Int64 array to a pretty string
        private static string Int64ArrayToString(Int64[] arr)
        {
            var strBuilder = new StringBuilder("");
            var n          = arr.Length;

            for (int i = 0; i < n; i++)
            {
                if (i == (n - 1))
                {
                    strBuilder = strBuilder.Append($"{arr[i]}\n");
                }

                else if (i == 0)
                {
                    strBuilder = strBuilder.Append($"Sort: {arr[i]}, ");
                }

                else
                {
                    strBuilder = strBuilder.Append($"{arr[i]}, ");
                }
            }

            return strBuilder.ToString();
         }

         static class GoMath
        {
            [DllImport("math.win.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern int Add(int a, int b);
            [DllImport("math.win.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern int Sub(int a, int b);
            [DllImport("math.win.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern double Cosine(double x);
            [DllImport("math.win.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern int Sum(GoSlice vals);
            [DllImport("math.win.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern void Sort(GoSlice vals);
        }

        static class GoHello
        {
            [DllImport("helloworld.win.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern unsafe void* HelloWorld(GoString s);
        }
    }
}


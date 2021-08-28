using System;
using System.Runtime.InteropServices;

namespace ManagedApp
{
    class Program
    {
        static void Main(string[] _)
        {
            CallManagedClass();
            CallPInvoke();
        }

        static void CallManagedClass()
        {
            Console.WriteLine($"=== Managed class ===");
            var c = new global::MixedLibrary.ManagedClass();
            c.Hello();
            c.CallNative("from managed app!");
            Console.WriteLine();
        }

        static void CallPInvoke()
        {
            Console.WriteLine($"=== P/Invoke ===");
            MixedLibrary.NativeEntryPoint_CallNative("from managed app!");
            Console.WriteLine();
        }

        private static class MixedLibrary
        {
            [DllImport(nameof(MixedLibrary), CallingConvention = CallingConvention.StdCall)]
            public static extern void NativeEntryPoint_CallNative([MarshalAs(UnmanagedType.LPWStr)] string msg);
        }
    }
}

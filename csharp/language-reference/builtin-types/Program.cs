using System;

namespace builtin_types
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======== Unmanaged types =========");
            UnmanagedTypes.Main();
            Console.WriteLine();

            Console.WriteLine("====== Nullable value types ======");
            NullableValueTypes.Examples();
            Console.WriteLine();
        }
    }
}

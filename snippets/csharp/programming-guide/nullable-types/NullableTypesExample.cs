using System;

namespace nullable_types
{
    class NullableTypesExample
    {
        internal static void Run()
        {
            // <Snippet1>
            int? num = null;

            int x = num.GetValueOrDefault();
            Console.WriteLine(x);
            
            if (num.HasValue)
            {
                Console.WriteLine($"num = {num.Value}");
            }
            else
            {
                Console.WriteLine("num = null");
            }

            try
            {
                int y = num.Value;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            // The example displays the following output:
            // 0
            // num = null
            // Nullable object must have a value.
            // </Snippet1>
        }
    }
}

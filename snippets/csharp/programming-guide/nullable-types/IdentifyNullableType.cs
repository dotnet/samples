using System;

namespace nullable_types
{
    class IdentifyNullableType
    {
        internal static void Examples()
        {
            WhetherTypeIsNullable();
            GetTypeExample();
            IsOperatorExample();
            WhetherInstanceIsOfNullableType();
        }

        private static void WhetherTypeIsNullable()
        {
            // <Snippet1>
            Console.WriteLine($"int? is {(IsNullable(typeof(int?)) ? "nullable" : "non nullable")} type");
            Console.WriteLine($"int is {(IsNullable(typeof(int)) ? "nullable" : "non nullable")} type");

            bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

            // Output:
            // int? is nullable type
            // int is non nullable type
            // </Snippet1>
        }

        private static void GetTypeExample()
        {
            // <Snippet2>
            int? a = 17;
            Type typeOfA = a.GetType();
            Console.WriteLine(typeOfA.FullName);
            // Output:
            // System.Int32
            // </Snippet2>
        }

        private static void IsOperatorExample()
        {
            // <Snippet3>
            int? a = 14;
            if (a is int)
            {
                Console.WriteLine("int? instance is compatible with int");
            }

            int b = 17;
            if (b is int?)
            {
                Console.WriteLine("int instance is compatible with int?");
            }
            // Output:
            // int? instance is compatible with int
            // int instance is compatible with int?
            // </Snippet3>
        }

        private static void WhetherInstanceIsOfNullableType()
        {
            // <Snippet4>
            int? a = 14;
            int b = 17;
            if (IsOfNullableType(a) && !IsOfNullableType(b))
            {
                Console.WriteLine("int? a is of a nullable type, while int b -- not");
            }

            bool IsOfNullableType<T>(T o)
            {
                var type = typeof(T);
                return Nullable.GetUnderlyingType(type) != null;
            }
            
            // Output:
            // int? a is of a nullable type, while int b -- not
            // </Snippet4>
        }
    }
}

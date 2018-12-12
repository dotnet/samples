using System;

namespace operators
{
    public static class EqualityAndNonEqualityExamples
    {
        public static void Examples()
        {
            Console.WriteLine("Value types:");
            ValueTypesEquality();

            Console.WriteLine("Strings:");
            StringEquality();

            Console.WriteLine("Reference types:");
            ReferenceTypeEquality.Main();

            Console.WriteLine("Non equality:");
            NonEquality();
        }

        private static void ValueTypesEquality()
        {
            // <SnippetValueTypesEquality>
            int a = 1 + 2 + 3;
            int b = 6;
            Console.WriteLine(a == b);  // output: True

            char c1 = 'a';
            char c2 = 'A';
            Console.WriteLine(c1 == c2);  // output: False
            Console.WriteLine(c1 == char.ToLower(c2));  // output: True
            // </SnippetValueTypesEquality>
        }

        private static void StringEquality()
        {
            // <SnippetStringEquality>
            string s1 = "hello!";
            string s2 = "HeLLo!";
            Console.WriteLine(s1 == s2.ToLower());  // output: True

            string s3 = "hello! ";
            Console.WriteLine(s1 == s3);  // output: False
            // </SnippetStringEquality>
        }

        // Rationale for the next snippet.
        // A method cannot contain a class definition. Thus, a standard way to include snippet doesn't work.
        // We want snippets to be interactive. Thus, the whole snippet has a structure of the console program.
        // (Running the code without the ReferenceTypeEquality class doesn't produce any output.)

        // <SnippetReferencyTypeEquality>
        public class ReferenceTypeEquality
        {
            public class MyClass
            {
                private int id;

                public MyClass(int id) => this.id = id;
            }

            public static void Main()
            {
                var a = new MyClass(1);
                var b = new MyClass(1);
                var c = a;
                Console.WriteLine(a == b);  // output: False
                Console.WriteLine(a == c);  // output: True
            }
        }
        // </SnippetReferencyTypeEquality>

        private static void NonEquality()
        {
            // <SnippetNonEquality>
            int a = 1 + 1 + 2 + 3;
            int b = 6;
            Console.WriteLine(a != b);  // output: True

            string s1 = "Hello";
            string s2 = "Hello";
            Console.WriteLine(s1 != s2);  // output: False

            object o1 = 1;
            object o2 = 1;
            Console.WriteLine(o1 != o2);  // output: True
            // </SnippetNonEquality>
        }
    }
}
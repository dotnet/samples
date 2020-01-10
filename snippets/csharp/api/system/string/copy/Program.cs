using System;
using System.Runtime.InteropServices;

namespace copy
{
    class Program
    {
        static void Main()
        {
            PerformStringOperation();
            Console.WriteLine("---");
            UseMutableBuffer();
            Console.WriteLine("---");
            UseUnmanaged();
        }

        private static void PerformStringOperation()
        {
            // <Snippet1>
            var original = "This is a sentence. This is a second sentence.";
            var sentence1 = original.Substring(0, original.IndexOf(".") + 1);
            Console.WriteLine(original);
            Console.WriteLine(sentence1);
            // The example displays the following output:
            //    This is a sentence. This is a second sentence.
            //    This is a sentence.            
            // </Snippet1>
        }

        // <Snippet2>
        private static void UseMutableBuffer()
        {
            var original = "This is a sentence. This is a second sentence.";
            var chars = original.ToCharArray();
            var span = new Span<char>(chars);
            var slice = span.Slice(span.IndexOf('.'), 3);
            slice = MergeSentence(slice);
            Console.WriteLine($"Original string: {original}");
            Console.WriteLine($"Modified string: {span.ToString()}");

            static Span<char> MergeSentence(Span<char> span)
            {
                if (span.Length == 0) return Span<char>.Empty;

                span[0] = ';';
                span[2] = Char.ToLower(span[2]);
                return span;
            }
        }
        // The example displays the following output:
        //    Original string: This is a sentence. This is a second sentence.
        //    Modified string: This is a sentence; this is a second sentence.        
        // </Snippet2>

        // <Snippet3>
        private static void UseUnmanaged()
        {
            var original = "This is a single sentence.";
            var len = original.Length; 
            var ptr = Marshal.StringToHGlobalUni(original);
            string result;
            unsafe 
            {
                char *ch = (char *) ptr.ToPointer();
                while (len-- > 0)
                {
                    char c = Convert.ToChar(Convert.ToUInt16(*ch) + 1);
                    *ch++ = c;
                } 
                result = Marshal.PtrToStringUni(ptr);
                Marshal.FreeHGlobal(ptr);
            }
            Console.WriteLine($"Original string: {original}");
            Console.WriteLine($"String from interop: '{result}'");
        }
        // The example displays the following output:
        //    Original string: This is a single sentence.
        //    String from interop: 'Uijt!jt!b!tjohmf!tfoufodf/'      
        // </Snippet3>
    }
}

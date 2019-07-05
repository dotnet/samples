using System;

namespace operators
{
    public static class StackallocOperator
    {
        public static void Examples()
        {
            Console.WriteLine("No snippets with output");
        }

        private static void AssignToPointer()
        {
            // <SnippetAssignToPointer>
            unsafe
            {
                int length = 3;
                int* numbers = stackalloc int[length];
                for (var i = 0; i < length; i++)
                {
                    numbers[i] = i;
                }
            }
            // </SnippetAssignToPointer>
        }

        private static void AssignToSpan()
        {
            // <SnippetAssignToSpan>
            int length = 3;
            Span<int> numbers = stackalloc int[length];
            for (var i = 0; i < length; i++)
            {
                numbers[i] = i;
            }
            // </SnippetAssignToSpan>
        }

        private static void AsExpression()
        {
            // <SnippetAsExpression>
            int length = 1000;
            Span<byte> buffer = length <= 1024 ? stackalloc byte[length] : new byte[length];
            // </SnippetAsExpression>
        }

        private static void StackallocInit()
        {
            // <SnippetStackallocInit>
            Span<int> first = stackalloc int[3] { 1, 2, 3 };
            Span<int> second = stackalloc int[] { 1, 2, 3 };
            ReadOnlySpan<int> third = stackalloc[] { 1, 2, 3 };
            // </SnippetStackallocInit>
        }
    }
}

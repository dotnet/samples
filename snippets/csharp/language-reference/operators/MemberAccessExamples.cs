using System;
// <SnippetNestedNamespace>
using System.Collections.Generic;
// </SnippetNestedNamespace>

namespace operators
{
    public static class MemberAccessExamples
    {
        public static void Examples()
        {
            TypeMemberAccess();
        }

        private static void QualifiedName()
        {
            // <SnippetQualifiedName>
            System.Collections.Generic.IEnumerable<int> numbers = new int[] { 1, 2, 3 };
            // </SnippetQualifiedName>
        }

        private static void TypeMemberAccess()
        {
            // <SnippetTypeMemberAccess>
            var constants = new List<double>();
            constants.Add(Math.PI);
            constants.Add(Math.E);
            Console.WriteLine($"{constants.Count} values to show:");
            Console.WriteLine(string.Join(", ", constants));
            // Output:
            // 2 values to show:
            // 3.14159265358979, 2.71828182845905
            // </SnippetTypeMemberAccess>
        }
    }
}
using System;

namespace keywords
{
    public static class TrueFalseLiteralsExamples
    {
        public static void Examples()
        {
            TrueExample();
            FalseExample();
        }

        private static void TrueExample()
        {
            // <SnippetTrueLiteral>
            bool check = true;
            Console.WriteLine(check ? "Passed" : "Not passed");
            // Output: 
            // Passed
            // </SnippetTrueLiteral>
        }

        private static void FalseExample()
        {
            // <SnippetFalseLiteral>
            bool check = false;
            Console.WriteLine(check ? "Passed" : "Not passed");
            // Output:
            // Not passed
            // </SnippetFalseLiteral>
        }
    }
}
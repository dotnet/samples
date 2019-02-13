using System;

namespace operators
{
    public static class LogicalNegationExamples
    {
        public static void Examples()
        {
            LogicalNegation();
        }

        private static void LogicalNegation()
        {
            // <SnippetExample>
            bool passed = false;
            Console.WriteLine(!passed);   // output: True
            Console.WriteLine(!true);     // output: False
            // </SnippetExample>
        }
    }
}
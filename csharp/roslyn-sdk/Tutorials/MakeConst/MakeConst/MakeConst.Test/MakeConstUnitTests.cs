using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using MakeConst;

namespace MakeConst.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        // <SnippetAlreadyConst>
        [TestMethod]
        public void WhenNodeIsAlreadyConstantNoDiagnosticsAreReported()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const int i = 0;
            Console.WriteLine(i);
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }
        // </SnippetAlreadyConst>

        // <SnippetCantBeConst>
        [TestMethod]
        public void WhenNodeHasNoInitializerNoDiagnosticsAreReported()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            i = 0;
            Console.WriteLine(i);
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenNodeInitializerIsntConstantNoDiagnosticsAreReported()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = DateTime.Now.DayOfYear;
            Console.WriteLine(i);
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }
        // </SnippetCantBeConst>

        // <SnippetMultipleDeclarations>
        [TestMethod]
        public void WhenNodeHasMultipleDeclarationsAlltNoDiagnosticsAreReported()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0, j = DateTime.Now.DayOfYear;
            Console.WriteLine(i, j);
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }
        // </SnippetMultipleDeclarations>

        // <SnippetVariableChangesValue>
        [TestMethod]
        public void WhenVariablesAreAssignedNoDiagnosticsAreReported()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i++);
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }
        // </SnippetVariableChangesValue>

        //Diagnostic and CodeFix both triggered and checked for
        // <SnippetTestMethodIntConstantV1>
        [TestMethod]
        public void WhenLocalIntCouldBeConstantAnalyzerReportsOneDiagnostic()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MakeConst",
                Message = "can be made constant",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        // </SnippetTestMethodIntConstantV1>

        [TestMethod]
        public void WhenLocalIntCouldBeConstantAnalyzerReportsOneDiagnosticWithFix()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MakeConst",
                Message = "can be made constant",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            // <SnippetTestMethodIntConstantFix>
            var fixtest = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const int i = 0;
            Console.WriteLine(i);
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
            // </SnippetTestMethodIntConstantFix>
        }

        // <SnippetAssignStringToInt>
        [TestMethod]
        public void WhenDeclarationIsInvalidNoDiagnosticIsReported()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = ""abc"";
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }
        // </SnippetAssignStringToInt>

        // <SnippetNoReferenceTypes>
        [TestMethod]
        public void WhenReferenceTypeIsntStringNoDiagnosticIsRaised()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            object s = ""abc"";
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void W()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = ""abc"";
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MakeConst",
                Message = "can be made constant",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
            var fixtest = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const string s = ""abc"";
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }
        // </SnippetNoReferenceTypes>


        // <SnippetReplaceVarDeclarationWithConst>
        [TestMethod]
        public void WhenDeclarationUsesVarConstDeclarationHasType()
        {
            var test = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var item = 4;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MakeConst",
                Message = "can be made constant",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
            var fixtest = @"
using System;

namespace MakeConstTest
{
    class Program
    {
        static void Main(string[] args)
        {
            const int item = 4;
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }
        // </SnippetReplaceVarDeclarationWithConst>



        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MakeConstCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MakeConstAnalyzer();
        }
    }
}

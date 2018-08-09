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

        // This section contains code to analyze where no diagnostic should e reported
private const string AlreadyConst = @"
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

private const string NoInitializer = @"
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

private const string InitializerNotConstant = @"
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

private const string MultipleInitializers = @"
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

private const string VariableAssigned = @"
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

private const string ReferenceTypeIsntString = @"
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

private const string DeclarationIsInvalid = @"
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

// This section contains code to analyze where the diagnostic should trigger,
// followed by the code after the fix has been applied.

private const string LocalIntCouldBeConstant = @"
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

private const string LocalIntCouldBeConstantFixed = @"
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

private const string ConstantIsString = @"
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

private const string ConstantIsStringFixed = @"
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

        private const string DeclarationUsesVar = @"
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
        private const string DeclarationUsesVarFixedHasType = @"
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

        //No diagnostics expected to show up
        // <SnippetAlreadyConst>
        [DataTestMethod]
        [DataRow(AlreadyConst),
         DataRow(NoInitializer),
         DataRow(InitializerNotConstant),
         DataRow(MultipleInitializers),
         DataRow(VariableAssigned),
         DataRow(DeclarationIsInvalid),
         DataRow(ReferenceTypeIsntString)]
        public void WhenTestCodeIsValidNoDiagnosticIsTriggered(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }
        // </SnippetAlreadyConst>


        //Diagnostic and CodeFix both triggered and checked for
        // <SnippetTestMethodIntConstantV1>
        [DataTestMethod]
        [DataRow(LocalIntCouldBeConstant, 10, 13),
         DataRow(ConstantIsString, 10, 13),
         DataRow(DeclarationUsesVar, 10, 13)]
        public void WhenDeclarationCouldBeConstDiagnosticIsRaised(string test, int line, int column)
        {
            var expected = new DiagnosticResult
            {
                Id = MakeConstAnalyzer.DiagnosticId,
                Message = "can be made constant",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        // </SnippetTestMethodIntConstantV1>

        [DataTestMethod]
        [DataRow(LocalIntCouldBeConstant, LocalIntCouldBeConstantFixed, 10, 13),
         DataRow(ConstantIsString, ConstantIsStringFixed, 10, 13),
         DataRow(DeclarationUsesVar, DeclarationUsesVarFixedHasType, 10, 13)]
        public void WhenDiagosticIsRaisedFixUpdatesCode(
            string test,
            string fixTest,
            int line,
            int column)
        {
            var expected = new DiagnosticResult
            {
                Id = MakeConstAnalyzer.DiagnosticId,
                Message = "can be made constant",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, fixTest);
        }

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

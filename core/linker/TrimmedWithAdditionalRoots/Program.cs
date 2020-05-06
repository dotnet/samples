using System;

namespace TrimmedWithAdditionalRoots
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleType = Type.GetType("System.Console,System.Console");
            var writeMethod = consoleType.GetMethod("WriteLine", new[] { typeof(string) });
            writeMethod.Invoke(null, new object[] { "Hello World!" });
        }
    }
}

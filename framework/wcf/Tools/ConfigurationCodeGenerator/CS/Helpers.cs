//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    class Helpers
    {
        internal static string TurnFirstCharLower(string typeName)
        {
            // changes the first char to lower case, so we can define a varName for the type
            char[] chars = typeName.ToCharArray(0, typeName.Length);
            chars[0] = chars[0].ToString().ToLower().ToCharArray()[0];
            return new string(chars);
        }

        internal static void DisplayFile(string srcFile)
        {
            using (StreamReader sr = new StreamReader(srcFile))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }

        internal static bool CompileCode(CodeDomProvider provider, Assembly userAssembly, string[] generatedSrcFiles)
        {
            // Parameters for compilation
            String[] referenceAssemblies = {
                "System.dll", 
                "System.ServiceModel.dll",
                "System.Configuration.dll",
                userAssembly.Location
                                            };
            string compiledFile = "temp.dll";

            CompilerParameters cp = new CompilerParameters(referenceAssemblies, compiledFile, false);
            cp.GenerateExecutable = false;
            
            Console.WriteLine("Compiling files: ");
            foreach (string fileName in generatedSrcFiles)
            {
                Console.WriteLine(fileName);
            }
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, generatedSrcFiles);

            if (cr.Errors.Count > 0)
            {
                Console.WriteLine("Please investigate. The tool encountered errors during compilation");
                foreach (CompilerError ce in cr.Errors)
                    Console.WriteLine(ce.ToString());
                return false;
            }
            else
            {
                Console.WriteLine("No errors encountered during compilation");
                File.Delete(compiledFile);
            }
            return true;
        }
    }
}

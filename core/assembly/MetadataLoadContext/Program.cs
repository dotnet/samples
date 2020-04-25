using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MetadataLoadContextSample
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet MetadataLoadContextSample.dll <assembly path>");
                return 0;
            }

            string inputFile = args[0];

            try
            {
                // Get the array of runtime assemblies.
                // This will allow us to at least inspect types depending only on BCL.
                string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");

                // Create the list of assembly paths consisting of runtime assemblies and the input file.
                var paths = new List<string>(runtimeAssemblies);
                paths.Add(inputFile);

                // Create MetadataLoadContext that can resolve assemblies using the created list.
                var resolver = new PathAssemblyResolver(paths);
                var mlc = new MetadataLoadContext(resolver);

                using (mlc)
                {
                    // Load assembly into MetadataLoadContext.
                    Assembly assembly = mlc.LoadFromAssemblyPath(inputFile);
                    AssemblyName name = assembly.GetName();

                    // Print assembly attribute information.
                    Console.WriteLine(name.Name + " has following attributes: ");

                    foreach (CustomAttributeData attr in assembly.GetCustomAttributesData())
                    {
                        try
                        {
                            Console.WriteLine(attr.AttributeType);
                        }
                        catch (FileNotFoundException ex)
                        {
                            // We are missing the required dependency assembly.
                            Console.WriteLine("Error getting attribute type: " + ex.Message);
                        }
                    }

                    Console.WriteLine();

                    // Print assembly type information.
                    Console.WriteLine(name.Name + " contains following types: ");

                    foreach (TypeInfo t in assembly.GetTypes())
                    {
                        try
                        {
                            Type baseType = t.BaseType;

                            if (t.IsClass)
                            {
                                Console.Write("class ");
                            }
                            else if (t.IsValueType)
                            {
                                if (String.Equals(baseType?.FullName, "System.Enum", StringComparison.InvariantCulture))
                                {
                                    Console.Write("enum ");
                                }
                                else
                                {
                                    Console.Write("struct ");
                                }
                            }
                            else if (t.IsInterface)
                            {
                                Console.Write("interface ");
                            }

                            Console.Write(t.FullName);

                            if (t.IsClass && !String.Equals(baseType.FullName, "System.Object", StringComparison.InvariantCulture))
                            {
                                Console.Write(" : " + baseType.FullName);
                            }

                            Console.WriteLine();
                        }
                        catch (System.IO.FileNotFoundException ex)
                        {
                            // We are missing the required dependency assembly.
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                }

                return 0;
            }
            catch (IOException ex)
            {
                Console.WriteLine("I/O error occured when trying to load assembly: ");
                Console.WriteLine(ex.ToString());
                return 1;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Access denied when trying to load assembly: ");
                Console.WriteLine(ex.ToString());
                return 1;
            }
        }
    }
}

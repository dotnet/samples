//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    // Generate code for a configuration section for a custom BindingElement and a 
    // custom StandardBinding
    // Also generates code you can add into your custom StandardBinding for loading the right config section
    // Either one can be provided; both are not necessary (custom binding element or custom standard binding)
    public class Program
    {
        static CodeDomProvider provider;
        static string bindingElementExtensionSectionSrcFile, stdBindingElementSrcFile;
        static Type bindingElementType = null, stdBindingType = null;
        static Assembly userAssembly = null;
        static string additionalCodeForStdBindingFile;
        static string generatedBEElementClassName = null, generatedSBCollectionElementClassName = null;
        static string assemblyTypeInformation = null;


        static void PrintUsage()
        {
            Console.WriteLine("Usage: ConfigurationCodeGenerator [/be:<BindingElementTypeName>] [/sb:<StandingBindingTypeName>] /dll:<Dll>\n");
            Console.WriteLine("Example Usage: ConfigurationCodeGenerator /be:UdpTransportBindingElement /sb:SampleProfileUdpBinding /dll:UdpTransport.dll");
            Console.WriteLine("This will generate a bunch of .cs files that will expose your binding element and binding to the configuration system");
            Console.WriteLine("\t" + "UdpTransportSection.cs for exposing your binding element to config");
            Console.WriteLine("\t" + "SampleProfileUdpBindingConfigurationElement.cs and SampleProfileBindingSection.cs for exposing your standard binding to config");
            Console.WriteLine("\t" + "UdpTransportDefaults.cs, UdpTransportConfigurationStrings.cs, SampleProfileUdpConfigurationStrings.cs and SampleProfileUdpDefaults.cs as helper classes with constants");
            Console.WriteLine("This will also generate a code snippet in a file called CodeToAddToSampleProfileUdpBinding.cs that you can copy/paste into your standard binding type to connect it to the configuration system");
        }

        static bool ParseCommandLine(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return false;
            }
            string bindingElementTypeName = null, stdBindingTypeName = null, dllName = null;

            // validate if we have the right arguments and parse out the types and dll
            foreach (string arg in args)
            {
                string bePrefix = "/be:", sbPrefix = "/sb:", dllPrefix = "/dll:";
                string lowerCaseArg = arg.ToLower();

                if (lowerCaseArg.ToLower().StartsWith(bePrefix))
                {
                    if (bindingElementTypeName == null)
                    {
                        bindingElementTypeName = arg.Substring(bePrefix.Length);
                    }
                }
                else if (lowerCaseArg.StartsWith(sbPrefix))
                {
                    if (stdBindingTypeName == null)
                    {
                        stdBindingTypeName = arg.Substring(sbPrefix.Length);
                    }
                }
                else if (lowerCaseArg.StartsWith(dllPrefix))
                {
                    if (dllName == null)
                    {
                        dllName = arg.Substring(dllPrefix.Length);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown argument: " + arg);
                    PrintUsage();
                    return false;
                }
            }
            // one of the two /be: /sb: must be specified; /dll: must always be specified
            bool badArgs = false;
            if (dllName == null)
            {
                Console.WriteLine("Must specify /dll:<dllName>");
                badArgs = true;
            }
            if ((bindingElementTypeName == null) && (stdBindingTypeName == null))
            {
                Console.WriteLine("Must specify either /be:<BindingElementTypeName> or /sb:<StandingBindingTypeName>");
                badArgs = true;
            }
            if (badArgs)
            {
                PrintUsage();
                return false;
            }

            if (bindingElementTypeName != null)
            {
                Console.WriteLine("bindingElementTypeName = " + bindingElementTypeName);
            }
            else
            {
                Console.WriteLine("No BindingElementType specified. Continuing to generate code for the StandardBinding configuration handler");
            }
            if (stdBindingTypeName != null)
            {
                Console.WriteLine("stdBindingTypeName = " + stdBindingTypeName);
            }
            else
            {
                Console.WriteLine("No StandardBindingType specified. Continuing to generate code for the BindingElement configuration handler");
            }
            Console.WriteLine("dllName = " + dllName);
            Console.WriteLine("-------------------");

            // If the dll doesnt exist or cant be loaded, this would throw
            string fullPath = Path.GetFullPath(dllName);
            userAssembly = Assembly.LoadFile(fullPath);

            // Verify the types are in the assembly 
            bool foundBEType = false, foundSBType = false;

            // if they are not specified, we dont need to find them
            if (bindingElementTypeName == null)
                foundBEType = true;
            if (stdBindingTypeName == null)
                foundSBType = true;

            foreach (Type t in userAssembly.GetTypes())
            {
                if (!foundBEType)
                {
                    if (t.Name.Equals(bindingElementTypeName) || bindingElementTypeName.EndsWith(t.Name))
                    {
                        Console.WriteLine("Found type: " + t + " that matches: " + bindingElementTypeName);
                        assemblyTypeInformation = t.Assembly.FullName;
                        bindingElementType = t;
                        foundBEType = true;
                    }

                }
                if (!foundSBType)
                {
                    if (t.Name.Equals(stdBindingTypeName) || stdBindingTypeName.EndsWith(t.Name))
                    {
                        Console.WriteLine("Found type: " + t + " that matches: " + stdBindingTypeName);
                        assemblyTypeInformation = t.Assembly.FullName;
                        stdBindingType = t;
                        foundSBType = true;
                    }
                }
                if (foundBEType && foundSBType)
                {
                    break;
                }
            }

            if (!foundBEType || !foundSBType)
            {
                // fail if we couldnt resolve either of the types
                Console.WriteLine("Couldn't find types in the assembly, quitting!");
                return false;
            }
            Console.WriteLine("-------------------");
            if (bindingElementType != null)
            {
                Console.WriteLine("Type: bindingElementType = " + bindingElementType);
            }
            if (stdBindingType != null)
            {
                Console.WriteLine("Type: stdBindingType = " + stdBindingType);
            }
            Console.WriteLine("-------------------");
            return true;
        }

        static void Main(string[] args)
        {
            if (!ParseCommandLine(args))
            {
                return;
            }
            provider = new CSharpCodeProvider();
            ArrayList beFilesGen = GenerateCodeToExposeBindingElementToConfig();
            ArrayList sbFilesGen = GenerateCodeToExposeStandardBindingToConfig();
            string xmlFileName = GenerateConfig();

            Console.WriteLine("\n\n\n\n----------ConfigurationCodeGenerator.exe Summary----------");
            Console.WriteLine("The following files were generated by the tool, please compile these types into the assembly: " + userAssembly.Location + " :" );
            if (beFilesGen != null)
            {
                foreach (string fileName in beFilesGen)
                {
                    Console.WriteLine(fileName);
                }
            }
            if (sbFilesGen != null)
            {
                foreach (string fileName in sbFilesGen)
                {
                    Console.WriteLine(fileName);
                }
            }
            Console.WriteLine(xmlFileName);
        }

        static string GenerateConfig()
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("Generating Sample Configuration Code");
            SampleConfigGenerator sc = new SampleConfigGenerator(generatedBEElementClassName, generatedSBCollectionElementClassName, assemblyTypeInformation);
            sc.Generate();
            Helpers.DisplayFile(sc.XmlFileName);
            return sc.XmlFileName;
        }

        static ArrayList GenerateCodeToExposeBindingElementToConfig()
        {
            string indent = "    ";
            ArrayList generatedFiles = null;

            if (bindingElementType != null)
            {
                bindingElementExtensionSectionSrcFile = bindingElementType.Name.Substring(0, bindingElementType.Name.IndexOf(TypeNameConstants.BindingElement)) + Constants.ElementSuffix + "." + provider.FileExtension;
                using (StreamWriter bindingElementExtensionSectionSW = new StreamWriter(bindingElementExtensionSectionSrcFile, false))
                {
                    using (IndentedTextWriter betw = new IndentedTextWriter(bindingElementExtensionSectionSW, indent))
                    {
                        BindingElementExtensionSectionGenerator bindingElementExtensionSectionGen = new BindingElementExtensionSectionGenerator(bindingElementType, userAssembly, provider);
                        provider.GenerateCodeFromCompileUnit(
                            bindingElementExtensionSectionGen.BuildCodeGraph(out generatedFiles),
                            betw,
                            new CodeGeneratorOptions());
                        generatedBEElementClassName = bindingElementExtensionSectionGen.NamespaceOfGeneratedClass + "." + bindingElementExtensionSectionGen.GeneratedClassName;
                    }
                }
                generatedFiles.Add(bindingElementExtensionSectionSrcFile);
                string[] generatedFilesArr = new string[generatedFiles.Count];
                generatedFiles.CopyTo(0, generatedFilesArr, 0, generatedFiles.Count);

                /* 
                 * Currently the sample does not pick default values for const fields, so compiling the
                 * generated code will result in compilation errors.
                 * If the sample is updated to provide default values for const fields, the following should
                 * be uncommented. The code which needs to be updated is in 
	         * CodeDomCommon.cs - CodeGenHelperMethods.EmitDefaultValuesClass
                 */

                //if (Helpers.CompileCode(provider, userAssembly, generatedFilesArr))
                //{
                //    // display the generated code
                      foreach (string fileName in generatedFilesArr)
                      {
                          Helpers.DisplayFile(fileName);
                      }
                //}
            }
            Console.WriteLine("-------------------");
            return generatedFiles;
        }

        static ArrayList GenerateCodeToExposeStandardBindingToConfig()
        {
            string indent = "    ";
            ArrayList generatedFiles = null;

            if (stdBindingType != null)
            {
                stdBindingElementSrcFile = stdBindingType.Name + Constants.ElementSuffix + "." + provider.FileExtension;
                using (StreamWriter stdBindingElementSW = new StreamWriter(stdBindingElementSrcFile, false))
                {
                    using (IndentedTextWriter sbtw = new IndentedTextWriter(stdBindingElementSW, indent))
                    {
                        StandardBindingSectionGenerator stdBindingElementGen = new StandardBindingSectionGenerator(stdBindingType, userAssembly, provider);
                        provider.GenerateCodeFromCompileUnit(
                            stdBindingElementGen.BuildCodeGraph(out generatedFiles),
                            sbtw,
                            new CodeGeneratorOptions());
                        generatedSBCollectionElementClassName = stdBindingElementGen.NamespaceOfGeneratedClass + "." + stdBindingElementGen.GeneratedCollectionElementClassName;
                        additionalCodeForStdBindingFile = "CodeToAddTo" + stdBindingType.Name + "." + provider.FileExtension;
                        // Since the ApplyConfigurationMethod has to be added to the custom standard binding
                        // class, let's spit out the code they need to add on the screen
                        UserTypeCodeEnhancer.EmitCodeToAddIntoCustomStdBinding(
                                            stdBindingType,
                                            stdBindingElementGen.GeneratedElementClassName,
                                            stdBindingElementGen.GeneratedCollectionElementClassName,
                                            additionalCodeForStdBindingFile);
                        generatedFiles.Add(additionalCodeForStdBindingFile);
                        Console.WriteLine("Add this code to your custom standard binding; this code has been generated in the file " + additionalCodeForStdBindingFile); 
                        Helpers.DisplayFile(additionalCodeForStdBindingFile);
                        Console.WriteLine("You will need to add using System.Globalization; and using System.Configuration; to your custom standard binding code too");
                        Console.WriteLine("You will also need to reference System.Configuration.dll");
                        Console.WriteLine("-------------------");
                    }
                }
                generatedFiles.Add(stdBindingElementSrcFile);
                string[] generatedFilesArr = new string[generatedFiles.Count];
                generatedFiles.CopyTo(0, generatedFilesArr, 0, generatedFiles.Count);

                /* 
                 * Currently the sample does not pick default values for const fields, so compiling the
                 * generated code will result in compilation errors.
                 * If the sample is updated to provide default values for const fields, the following should
                 * be uncommented. The code which needs to be updated is in 
	         * CodeDomCommon.cs - CodeGenHelperMethods.EmitDefaultValuesClass
                 */

                //if (Helpers.CompileCode(provider, userAssembly, generatedFilesArr))
                //{
                    // display the generated code
                    foreach (string fileName in generatedFilesArr)
                    {
                        Helpers.DisplayFile(fileName);
                    }
                //}
            }
            return generatedFiles;
        }
    }
}

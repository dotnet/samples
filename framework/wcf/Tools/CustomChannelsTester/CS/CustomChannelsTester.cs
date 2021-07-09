//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;  

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    static class CustomChannelsTester
    {
        // Generate client and server based on the test specification and run the test using the Binding specified
        // (Custom or Standard)
        
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            if (!ParseCommandLine(args))
            {
                return;
            }
            if (!RunTest())
            {
                Log.Trace("Test Failed");
                return;
            }
            Log.Trace("Test Passed");
            Log.Trace("Time to run the test: " + (DateTime.Now - start));                
        }

        static bool RunTest()
        {
            return (new TestRunner()).Run();
        }

        static bool ParseCommandLine(string[] args)
        {
            if (args.Length < 1)             
            {
                PrintUsage();
                return false;
            }
            string inputFileName = null, bindingDllName = null, bindingName = null;
            Assembly bindingAssembly = null;
            
            // validate if we have the right arguments and parse out the types and dll
            foreach(string arg in args)
            {
                string filePrefix = "/testspec:", dllPrefix = "/dll:", bindingPrefix = "/binding:";
                string lowerCaseArg = arg.ToLower();

                if (lowerCaseArg.ToLower().StartsWith(filePrefix))
                {
                    if (inputFileName == null)
                    {
                        inputFileName = arg.Substring(filePrefix.Length);
                    }
                }
                else if (lowerCaseArg.StartsWith(bindingPrefix))
                {
                    if (bindingName == null)
                    {
                        bindingName = arg.Substring(bindingPrefix.Length);
                    }
                }
                else if (lowerCaseArg.StartsWith(dllPrefix))
                {
                    if (bindingDllName == null)
                    {
                        bindingDllName = arg.Substring(dllPrefix.Length);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown argument: " + arg);
                    PrintUsage();
                    return false;
                }
            }

            if (inputFileName != null)
            {
                inputFileName = Path.Combine(Directory.GetCurrentDirectory(), inputFileName);
                if (!File.Exists(inputFileName))
                {
                    Console.WriteLine("Test Specification File does not exist");
                    return false;
                }
                else
                    Parameters.InputFileName = inputFileName;
            }
            
            //Binding name needs to be specified

            //If the binding is a standard binding provided by Windows Communication Foundation, then 
            // the Dll need not be specified

            if (bindingName != null)
            {
                Parameters.BindingName = bindingName;
            }
            else
            {
                Console.WriteLine("Must specify /binding:<bindingName>");
                return false;
            }
            
            if(bindingDllName != null)
            {
                try
                {
                    // If the dll doesnt exist or cant be loaded, this would throw
                    string fullPath = Path.GetFullPath(bindingDllName);
                    bindingAssembly = Assembly.LoadFile(fullPath);
                    Parameters.BindingAssembly = bindingAssembly;
                }
                catch(Exception e)
                {
                    Console.WriteLine("Unable to Load the Dll Assembly");
                    Console.WriteLine(e.StackTrace);
                    return false;
                }
                bool foundCustomBinding = false;

                // Verify the types is in the assembly 
                foreach (Type t in bindingAssembly.GetTypes())
                {
                    if (!foundCustomBinding)
                    {
                        if (t.Name.Equals(bindingName) || bindingName.EndsWith(t.Name))
                        {
                            Console.WriteLine("Found type: " + t + " that matches: " + bindingName);
                            Parameters.BindingType = t;
                            foundCustomBinding = true;
                            break;
                        }
                    }                    
                }
                if (!foundCustomBinding)
                {
                    // fail if we couldnt resolve the binding type
                    Console.WriteLine("Couldn't find type CustomBinding in the assembly, quitting!");
                    return false;
                }
            }

            return true;            
        }
        
        static void PrintUsage()
        {
            Console.WriteLine("Usage: CustomChannelsTester [/binding:<BindingName>] [/dll:<DLL>] [/testspec:<XML>] \n");
            Console.WriteLine("Example Usage: CustomChannelsTester /binding:BindingName /dll:Transport.dll /testspec:TestSpec.xml");
            Console.WriteLine("               BindingName should contain the name of the binding implemented");
            Console.WriteLine("               Transport.dll should contain the description of the transport and the binding implemented");
            Console.WriteLine("               TestSpec.xml describes the test parameters such as Address, Standard Binding and Contracts");            
        }
    }
}

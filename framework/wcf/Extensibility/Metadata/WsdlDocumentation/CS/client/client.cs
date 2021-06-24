
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.ServiceModel.Samples
{
    //The service contract is defined in generatedClient.cs, 
    // generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // First create a client code file in C# and Visual Basic that imports
            // WSDL annotations as comments in code that support intellisense.
            Uri metadataAddress = new Uri("http://localhost/servicemodelsamples/service.svc?wsdl");
            GenerateCSCodeForService(
              metadataAddress,
              System.Environment.CurrentDirectory + "\\..\\..\\GeneratedContract.cs"
            );

            GenerateVBCodeForService(
              metadataAddress,
              System.Environment.CurrentDirectory + "\\..\\..\\GeneratedContract.vb"
            );

            // Create a client
            ChannelFactory<ICalculator> wcfClientFactory = new ChannelFactory<ICalculator>("");
            ICalculator wcfCalcChannel = wcfClientFactory.CreateChannel();

            // If you use Visual Studio C# or Visual Basic.NET, you can type in  
            // one of the wcfCalcChannel methods and as you type you see 
            // intellisense for all operations, return values, and arguments.

            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = wcfCalcChannel.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = wcfCalcChannel.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = wcfCalcChannel.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = wcfCalcChannel.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);

            //Closing the client gracefully closes the connection and cleans up resources
            ((IClientChannel)wcfCalcChannel).Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void GenerateVBCodeForService(Uri metadataAddress, string outputFile)
        {
          MetadataExchangeClient mexClient = new MetadataExchangeClient(metadataAddress, MetadataExchangeClientMode.HttpGet);
          mexClient.ResolveMetadataReferences = true;
          MetadataSet metaDocs = mexClient.GetMetadata();

          WsdlImporter importer = new WsdlImporter(metaDocs);
          ServiceContractGenerator generator = new ServiceContractGenerator();

          System.Collections.ObjectModel.Collection<ContractDescription> contracts = importer.ImportAllContracts();
          foreach (ContractDescription contract in contracts)
          {
            generator.GenerateServiceContractType(contract);
          }
          if (generator.Errors.Count != 0)
            throw new ApplicationException("There were errors during code compilation.");

          // Write the code dom.
          System.CodeDom.Compiler.CodeGeneratorOptions options = new System.CodeDom.Compiler.CodeGeneratorOptions();
          options.BracingStyle = "C";
          System.CodeDom.Compiler.CodeDomProvider codeDomProvider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("VB");
          System.CodeDom.Compiler.IndentedTextWriter textWriter = new System.CodeDom.Compiler.IndentedTextWriter(new System.IO.StreamWriter(outputFile));
          codeDomProvider.GenerateCodeFromCompileUnit(generator.TargetCompileUnit, textWriter, options);
          textWriter.Close();
        }

      
        static void GenerateCSCodeForService(Uri metadataAddress, string outputFile)
        {
          MetadataExchangeClient mexClient = new MetadataExchangeClient(metadataAddress, MetadataExchangeClientMode.HttpGet);
          mexClient.ResolveMetadataReferences = true;
          MetadataSet metaDocs = mexClient.GetMetadata();

          WsdlImporter importer = new WsdlImporter(metaDocs);
          ServiceContractGenerator generator = new ServiceContractGenerator();

          System.Collections.ObjectModel.Collection<ContractDescription> contracts
            = importer.ImportAllContracts();
          foreach (ContractDescription contract in contracts)
          {
            generator.GenerateServiceContractType(contract);
          }
          if (generator.Errors.Count != 0)
            throw new ApplicationException("There were errors during code compilation.");

          // Write the code dom
          System.CodeDom.Compiler.CodeGeneratorOptions options
            = new System.CodeDom.Compiler.CodeGeneratorOptions();
          options.BracingStyle = "C";
          System.CodeDom.Compiler.CodeDomProvider codeDomProvider
            = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("C#");
          System.CodeDom.Compiler.IndentedTextWriter textWriter
            = new System.CodeDom.Compiler.IndentedTextWriter(new System.IO.StreamWriter(outputFile));
          codeDomProvider.GenerateCodeFromCompileUnit(
            generator.TargetCompileUnit, textWriter, options
          );
          textWriter.Close();
        }
      }
}


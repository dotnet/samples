//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.CryptoAgility
{
    class Client
    {
        static void Main()
        {
            // Register all the custom URI strings to the desired crypto algorithm objects.
            // This should be done before sending the first message/request from the client to the service.
            RegisterCryptoAlgorithm(); 

            // Create a channel factory
            ChannelFactory<ICalculator> factory = new ChannelFactory<ICalculator>(GetClientBinding(), new EndpointAddress("http://localhost:8003/servicemodelsamples/CalculatorService"));
           
            // Create an ICalculator channel
            ICalculator client = factory.CreateChannel();
             
            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
            Console.WriteLine();

            // Close the channel factory.
            factory.Close();

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }

        private static Binding GetClientBinding()
        {
            WSHttpBinding binding = new WSHttpBinding();
            
            //Intializing the custom algorithm suite to be used for crypto operations.
            binding.Security.Message.AlgorithmSuite = new MyCustomAlgorithmSuite();
            
            // Disabling reliable session and security context.
            binding.Security.Message.EstablishSecurityContext = false;
            binding.ReliableSession.Enabled = false;
            return binding;
        }

        // This method demonstrates how a standard cryptographic algorithm or a custom implementation of a 
        // crypto algorithm can be registered to a given algorithm Uri. An alternate way to register a URI 
        // to a standard or custom crypto algorithm is by adding the following entry in the 'mscorlib' 
        // section in machine.config file. This setting is at the machine wide level for WCF.

        //                 <configuration>
        //                    <mscorlib>
        //                       <cryptographySettings>
        //                          <cryptoNameMapping>
        //                             <cryptoClasses>
        //                                    <cryptoClass SHA256CSP="System.Security.Cryptography.SHA256CryptoServiceProvider, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        //                             </cryptoClasses>
        //                             <nameEntry name="http://constoso.com/CustomAlgorithms/CustomHashAlgorithm"
        //                                         class="SHA256CSP" />
        //                          </cryptoNameMapping>
        //                       </cryptographySettings>
        //                   </mscorlib>
        //                </configuration>

        static void RegisterCryptoAlgorithm()
        {
            // Register the custom URI string defined for the hashAlgorithm in MyCustomAlgorithmSuite class to create the 
            // SHA256CryptoServiceProvider hash algorithm object.
            CryptoConfig.AddAlgorithm(typeof(SHA256CryptoServiceProvider), "http://constoso.com/CustomAlgorithms/CustomHashAlgorithm");
        }
    }

    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

}

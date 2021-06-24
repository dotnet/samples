//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.CryptoAgility
{
    // Service class which implements the service contract.
    public class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2)
        {
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            return n1 / n2;
        }

        // Self-host the service in the exe application.
        public static void Main()
        {
            // Register all the custom URI strings to the desired crypto algorithm objects.
            // This should be done before sending the first message/request from the client to the service.
            RegisterCryptoAlgorithm();

            // Create a ServiceHost for the CalculatorService type. The base address is supplied in the app.config
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                serviceHost.AddServiceEndpoint(typeof(ICalculator),
                                                GetServiceBinding(),
                                                new Uri("http://localhost:8003/servicemodelsamples/CalculatorService"));

                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();
                Console.WriteLine("The service is ready. Press <ENTER> to terminate service.");
                Console.ReadLine();
            }
        }

        static Binding GetServiceBinding()
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Message.AlgorithmSuite = new MyCustomAlgorithmSuite();
            binding.Security.Message.EstablishSecurityContext = false;
            binding.ReliableSession.Enabled = false;
            return binding;
        }

        // This method demonstrates how a standard cryptographic algorithm or a custom implementation of a 
        // crypto algorithm can be registered to a given algorithm Uri. An alternate way to register a URI 
        // to a standard or custom crypto algorithm is by adding the following entry in the 'mscorlib' 
        // section in machine.config file. 

        //                 <configuration>
        //                    <mscorlib>
        //                       <cryptographySettings>
        //                          <cryptoNameMapping>
        //                             <cryptoClasses>
        //                                    <cryptoClass SHA256CSP="System.Security.Cryptography.SHA256CryptoServiceProvider, System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
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
   


//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.Security.Permissions;

using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;

namespace Microsoft.Samples.DurableIssuedTokenProvider
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    class CalculatorService : ICalculator
    {
        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            Console.WriteLine("Received Add({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            Console.WriteLine("Received Subtract({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            Console.WriteLine("Received Multiply({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }

        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            Console.WriteLine("Received Divide({0},{1})", n1, n2);
            Console.WriteLine("Return: {0}", result);
            return result;
        }
    }

    class Service
    {
        static void Main(string[] args)
        {
            // Create service host
            ServiceHost sh = new ServiceHost(typeof(CalculatorService));

            // Setting the certificateValidationMode to PeerOrChainTrust means that if the certificate 
            // is in the user's Trusted People store, then it will be trusted without performing a
            // validation of the certificate's issuer chain. This setting is used here for convenience so that the 
            // sample can be run without having to have certificates issued by a certificate authority (CA).
            // This setting is less secure than the default, ChainTrust. The security implications of this 
            // setting should be carefully considered before using PeerOrChainTrust in production code. 
            sh.Credentials.IssuedTokenAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;
            sh.Open();

            try
            {
                foreach (ChannelDispatcher cd in sh.ChannelDispatchers)
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                        Console.WriteLine("Service listening at {0}", ed.EndpointAddress.Uri);

                Console.WriteLine("Press enter to close the service");
                Console.ReadLine();
            }
            finally
            {
                sh.Close();
            }
        }
    }  
}


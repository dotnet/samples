//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.ReliableSessionOverHttps
{
    //The service contract is defined in generatedProxy.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // WARNING: This code is only needed for test certificates such as those created by makecert. It is 
            // not recommended for production code.
            PermissiveCertificatePolicy.Enact("CN=ServiceModelSamples-HTTPS-Server");

            // Create a client
            CalculatorClient client = new CalculatorClient();

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

            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }

    // WARNING: This code is only needed for test certificates such as those created by makecert. It is 
    // not recommended for production code.
    class PermissiveCertificatePolicy
    {
        string subjectName;
        static PermissiveCertificatePolicy currentPolicy;
        PermissiveCertificatePolicy(string subjectName)
        {
            this.subjectName = subjectName;
            ServicePointManager.ServerCertificateValidationCallback +=
                new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertValidate);
        }

        public static void Enact(string subjectName)
        {
            currentPolicy = new PermissiveCertificatePolicy(subjectName);
        }

        bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            if (cert.Subject == subjectName)
            {
                return true;
            }

            return false;
        }
    }


}




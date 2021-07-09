
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Configuration;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Microsoft.ServiceModel.Samples
{

    // Define the service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICalculator
    {
        [OperationContract]
        string GetCallerIdentity();
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }


    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Specify the Metadata Exchange binding and its security mode
            WSHttpBinding mexBinding = new WSHttpBinding(SecurityMode.Message);
            mexBinding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;

            // Create a MetadataExchangeClient for retrieving metadata, and set the certificate details
            MetadataExchangeClient mexClient = new MetadataExchangeClient(mexBinding);
            mexClient.SoapCredentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser, StoreName.My,
                X509FindType.FindBySubjectName, "client.com");
            mexClient.SoapCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;
            mexClient.SoapCredentials.ServiceCertificate.SetDefaultCertificate(
                StoreLocation.CurrentUser, StoreName.TrustedPeople,
                X509FindType.FindBySubjectName, "localhost");

            // The contract we want to fetch metadata for
            Collection<ContractDescription> contracts = new Collection<ContractDescription>();
            ContractDescription contract = ContractDescription.GetContract(typeof(ICalculator));
            contracts.Add(contract);

            // Find endpoints for that contract
            EndpointAddress mexAddress = new EndpointAddress(ConfigurationManager.AppSettings["mexAddress"]);
            ServiceEndpointCollection endpoints = MetadataResolver.Resolve(contracts,
                mexAddress, mexClient);

            // Communicate with each endpoint that supports the ICalculator contract.
            foreach (ServiceEndpoint endpoint in endpoints)
            {
                if (endpoint.Contract.Namespace.Equals(contract.Namespace) && endpoint.Contract.Name.Equals(contract.Name))
                {
                    // Create a channel and set the certificate details to communicate with the Application endpoint
                    ChannelFactory<ICalculator> cf = new ChannelFactory<ICalculator>(endpoint.Binding, endpoint.Address);
                    cf.Credentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My,
                        X509FindType.FindBySubjectName, "client.com");
                    cf.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;

                    ICalculator channel = cf.CreateChannel();

                    // call operations
                    DoCalculations(channel);

                    ((IChannel)channel).Close();
                    cf.Close();

                 }
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
             
        }


        static void DoCalculations(ICalculator channel)
        {
            // Call the GetCallerIdentity service operation
            Console.WriteLine(channel.GetCallerIdentity());

            // Call the Add service operation.
            double value1 = 100.00D;
            double value2 = 15.99D;
            double result = channel.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);


            // Call the Subtract service operation.
            value1 = 145.00D;
            value2 = 76.54D;
            result = channel.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            value1 = 9.00D;
            value2 = 81.25D;
            result = channel.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            value1 = 22.00D;
            value2 = 7.00D;
            result = channel.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
        }
  
    }
}


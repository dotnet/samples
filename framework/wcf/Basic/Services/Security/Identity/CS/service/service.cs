
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.IdentityModel.Selectors;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Samples.Identity
{    
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.Identity")]
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

    // Service class which implements the service contract.
    // Added code to write output to the console window
    public class CalculatorService : ICalculator
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

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type. Base Address is supplied in app.config
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                // The base address is read from the app.config
                Uri dnsrelativeAddress = new Uri(serviceHost.BaseAddresses[0], "dnsidentity");
                Uri certificaterelativeAddress = new Uri(serviceHost.BaseAddresses[0], "certificateidentity");
                Uri rsarelativeAddress = new Uri(serviceHost.BaseAddresses[0], "rsaidentity");

                // Set the service's X509Certificate to protect the messages
                serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                                          StoreName.My,
                                                                          X509FindType.FindBySubjectDistinguishedName,
                                                                          "CN=identity.com, O=Contoso");
                //cache a reference to the server's certificate.
                X509Certificate2 servercert = serviceHost.Credentials.ServiceCertificate.Certificate;

                //Create endpoints for the service using a WSHttpBinding set for anonymous clients
                WSHttpBinding wsAnonbinding = new WSHttpBinding(SecurityMode.Message);
                //Clients are anonymous to the service
                wsAnonbinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                //Secure conversation (session) is turned off
                wsAnonbinding.Security.Message.EstablishSecurityContext = false;

                //Create a service endpoint and change its identity to the DNS for an X509 Certificate
                ServiceEndpoint ep = serviceHost.AddServiceEndpoint(typeof(ICalculator),
                                                                    wsAnonbinding,
                                                                    String.Empty);
                EndpointAddress epa = new EndpointAddress(dnsrelativeAddress, EndpointIdentity.CreateDnsIdentity("identity.com"));
                ep.Address = epa;

                //Create a service endpoint and change its identity to the X509 certificate returned as base64 encoded value
                ServiceEndpoint ep2 = serviceHost.AddServiceEndpoint(typeof(ICalculator),
                                                                     wsAnonbinding,
                                                                     String.Empty);
                EndpointAddress epa2 = new EndpointAddress(certificaterelativeAddress, EndpointIdentity.CreateX509CertificateIdentity(servercert));
                ep2.Address = epa2;

                //Create a service endpoint and change its identity to the X509 certificate's RSA key value
                ServiceEndpoint ep3 = serviceHost.AddServiceEndpoint(typeof(ICalculator), wsAnonbinding, String.Empty);
                EndpointAddress epa3 = new EndpointAddress(rsarelativeAddress, EndpointIdentity.CreateRsaIdentity(servercert));
                ep3.Address = epa3;

                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();

                // List the address and the identity for each ServiceEndpoint
                foreach (ChannelDispatcher channelDispatcher in serviceHost.ChannelDispatchers)
                {
                    foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        Console.WriteLine("Endpoint Address: {0}", endpointDispatcher.EndpointAddress);
                        Console.WriteLine("Endpoint Identity: {0}", endpointDispatcher.EndpointAddress.Identity);
                        Console.WriteLine();
                    }
                }

                //foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
                //{
                //    object resource = endpoint.Address.Identity.IdentityClaim.Resource;
                //    string identityValue;
                //    if (resource.GetType() == typeof(System.Byte[]))
                //    {
                //        identityValue = System.Convert.ToBase64String((System.Byte[])(resource));
                //    }
                //    else
                //        identityValue = resource.ToString();

                //    Console.WriteLine("Service listening Address: {0}", endpoint.Address);
                //    Console.WriteLine("Service listening Identity: {0}", identityValue);
                //    Console.WriteLine();
                //}

                // The service can now be accessed.
                Console.WriteLine();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        // Method for retreving a named certificate from a particular store.
        static X509Certificate2 GetServerCertificate(string name)
        {
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, name, false);
            store.Close();
            if (certs.Count == 0)
                throw new Exception("You have not installed the certificates. Run setup.bat for this project");
            if (certs.Count > 1)
                throw new Exception ("Duplicate certificates found in the store");
            return certs [0];

        }

    }
}



//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Security.Cryptography.X509Certificates;

using System.IdentityModel.Claims;
using System.IdentityModel.Policy;

using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.Identity
{
    //The service contract is defined in generatedclient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client with given client endpoint configuration
            //DNS identity
            CallService("WSHttpBinding_ICalculator");
            //Certificate identity
            CallService("WSHttpBinding_ICalculator1");
            //RSA identity
            CallService("WSHttpBinding_ICalculator2");
            //UPN identity
            CallService("WSHttpBinding_ICalculator3");

            //Create a custom client side identity to authenticate the service
            CallServiceCustomClientIdentity("WSHttpBinding_ICalculator");

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        public static void CallService(string endpointName)
        {
            CalculatorClient client = new CalculatorClient(endpointName);

            Console.WriteLine("Calling Endpoint: {0}", endpointName);
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

            client.Close();

        }

        public static void CallServiceCustomClientIdentity(string endpointName)
        {
            // Create a custom binding that sets a custom IdentityVerifier 
            Binding customSecurityBinding = CreateCustomSecurityBinding();
            // Call the service with DNS identity, setting a custom EndpointIdentity that checks that the certificate
            // returned from the service contains an organization name of Contoso in the subject name i.e. O=Contoso 
            EndpointAddress serviceAddress = new EndpointAddress(new Uri("http://localhost:8003/servicemodelsamples/service/dnsidentity"),
                                                                  new OrgEndpointIdentity("O=Contoso"));

            using (CalculatorClient client = new CalculatorClient(customSecurityBinding, serviceAddress))
            {
                client.ClientCredentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.CurrentUser,
                                                                                  StoreName.TrustedPeople,
                                                                                  X509FindType.FindBySubjectDistinguishedName,
                                                                                  "CN=identity.com, O=Contoso");
                //Setting the certificateValidationMode to PeerOrChainTrust means that if the certificate 
                //is in the user's Trusted People store, then it will be trusted without performing a
                //validation of the certificate's issuer chain. This setting is used here for convenience so that the 
                //sample can be run without having to have certificates issued by a certificate authority (CA).
                //This setting is less secure than the default, ChainTrust. The security implications of this 
                //setting should be carefully considered before using PeerOrChainTrust in production code. 
                client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;

                Console.WriteLine("Calling Endpoint: {0}", endpointName);
                // Call the Add service operation.
                double value1 = 100.00D;
                double value2 = 15.99D;
                double result = client.Add(value1, value2);
                Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);
                Console.WriteLine();

                // Call the Subtract service operation.
                value1 = 145.00D;
                value2 = 76.54D;
                result = client.Subtract(value1, value2);
                Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);
            }
        }

        //Create a custom binding using a WsHttpBinding
        public static Binding CreateCustomSecurityBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.Message);
            //Clients are anonymous to the service
            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            //Secure conversation is turned off for simplification. If secure conversation is turned on then 
            //you also need to set the IdentityVerifier on the secureconversation bootstrap binding.
            binding.Security.Message.EstablishSecurityContext = false;

            //Get the SecurityBindingElement and cast to a SymmetricSecurityBindingElement to set the IdentityVerifier
            BindingElementCollection outputBec = binding.CreateBindingElements();
            SymmetricSecurityBindingElement ssbe = (SymmetricSecurityBindingElement)outputBec.Find<SecurityBindingElement>();

            //Set the Custom IdentityVerifier
            ssbe.LocalClientSettings.IdentityVerifier = new CustomIdentityVerifier();

            return new CustomBinding(outputBec);
        }
    }

    // This custom EndpointIdentity stores an organization name as a string value.
    public class OrgEndpointIdentity : EndpointIdentity
    {
        private string orgClaim;
        public OrgEndpointIdentity(string orgName)
        {
            orgClaim = orgName;
        }

        public string OrganizationClaim
        {
            get { return orgClaim; }
            set { orgClaim = value; }
        }
    }

    //This custom IdentityVerifier uses the supplied OrgEndpointIdentity to check that 
    //X509 certificate's distinguished name claim contains the organization name e.g. O=Contoso 
    class CustomIdentityVerifier : IdentityVerifier
    {
        public override bool CheckAccess(EndpointIdentity identity, AuthorizationContext authContext)
        {
            bool returnvalue = false;

            foreach (ClaimSet claimset in authContext.ClaimSets)
            {
                foreach (Claim claim in claimset)
                {
                    if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname")
                    {
                        X500DistinguishedName name = (X500DistinguishedName)claim.Resource;
                        if (name.Name.Contains(((OrgEndpointIdentity)identity).OrganizationClaim))
                        {
                            Console.WriteLine("Claim Type: {0}", claim.ClaimType);
                            Console.WriteLine("Right: {0}", claim.Right);
                            Console.WriteLine("Resource: {0}", claim.Resource);
                            Console.WriteLine();
                            returnvalue = true;
                        }
                    }
                }

            }
            return returnvalue;
        }

        public override bool TryGetIdentity(EndpointAddress reference, out EndpointIdentity identity)
        {
            return IdentityVerifier.CreateDefault().TryGetIdentity(reference, out identity);
        }
    }
}


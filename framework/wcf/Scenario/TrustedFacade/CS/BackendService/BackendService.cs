
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using System.ServiceModel;

namespace Microsoft.Samples.TrustedFacade
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.TrustedFacade")]
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

    // Service class which implements the service contract.
    public class BackendService : ICalculator
    {
        public string GetCallerIdentity()
        {
            // Facade service is authenticated using Windows authentication. Its identity is accessible
            // on ServiceSecurityContext.Current.WindowsIdentity
            string facadeServiceIdentityName = ServiceSecurityContext.Current.WindowsIdentity.Name;

            // The client name is transmitted using Username authentication on the message level without the password
            // using a supporting encrypted UserNameToken
            // Claims extracted from this supporting token are available in 
            // ServiceSecurityContext.Current.AuthorizationContext.ClaimSets collection.
            string clientName = null;
            foreach (ClaimSet claimSet in ServiceSecurityContext.Current.AuthorizationContext.ClaimSets)
            {
                foreach (Claim claim in claimSet)
                {
                    if (claim.ClaimType == ClaimTypes.Name && claim.Right == Rights.Identity)
                    {
                        clientName = (string)claim.Resource;
                        break;
                    }
                }
            }
            if (clientName == null)
            {
                // In case there was no UserNameToken attached to the request
                // In the real world implementation the service will very likely reject such request.
                return "Anonymous caller via " + facadeServiceIdentityName;
            }

            return clientName + " via " + facadeServiceIdentityName;
        }

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

        static void Main()
        {
            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using (ServiceHost serviceHost = new ServiceHost(typeof(BackendService)))
            {
                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }

    public class MyUserNamePasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            // ignore the password because it is empty, we trust the facade service to authenticate the client
            // we just accept the username information here so that application gets access to it
            if (null == userName)
            {
                Console.WriteLine("Invalid username");
                throw new SecurityTokenValidationException("Invalid username");
            }
        }
    }
}


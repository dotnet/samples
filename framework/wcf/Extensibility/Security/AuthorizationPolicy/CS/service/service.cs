//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.ServiceModel;

namespace Microsoft.Samples.AuthorizationPolicy
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.AuthorizationPolicy")]
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
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	
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
            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();
				
                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
				Console.WriteLine("The service is running in the following account: {0}", WindowsIdentity.GetCurrent().Name);
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }

    public class MyServiceAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            // Extact the action URI from the OperationContext. We will use this to match against the claims
            // in the AuthorizationContext
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;
            Console.WriteLine("action: {0}", action);

            // Iterate through the various claimsets in the authorizationcontext
            foreach (ClaimSet cs in operationContext.ServiceSecurityContext.AuthorizationContext.ClaimSets)
            {
                // Only look at claimsets issued by System.
                if (cs.Issuer == ClaimSet.System)
                {
                    // Iterate through claims of type "http://example.org/claims/allowedoperation"
                    foreach (Claim c in cs.FindClaims("http://example.org/claims/allowedoperation", Rights.PossessProperty))
                    {
                        // Dump the Claim resource to the console.
                        Console.WriteLine("resource: {0}", c.Resource.ToString());

                        // If the Claim resource matches the action URI then return true to allow access
                        if (action == c.Resource.ToString())
                            return true;
                    }
                }
            }

            // If we get here, return false, denying access.
            return false;
        }
    }
        
    public class MyCustomUserNameValidator : UserNamePasswordValidator
    {
        // This method validates users. It allows in two users, test1 and test2 
        // with passwords 1tset and 2tset respectively.
        // This code is for illustration purposes only and 
        // MUST NOT be used in a production environment because it is NOT secure.	
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == "test1" && password == "1tset") && !(userName == "test2" && password == "2tset"))
            {
                throw new SecurityTokenException("Unknown Username or Password");
            }
        }
    }
}


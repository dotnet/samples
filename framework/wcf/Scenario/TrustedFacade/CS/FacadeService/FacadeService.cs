
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using System.ServiceModel;

namespace Microsoft.Samples.TrustedFacade
{
    public class FacadeService : ICalculator
    {
        public string GetCallerIdentity()
        {
            CalculatorClient client = new CalculatorClient();
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string result = client.GetCallerIdentity();
            client.Close();
            return result;
        }

        public double Add(double n1, double n2)
        {
            CalculatorClient client = new CalculatorClient();
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            double result = client.Add(n1, n2);
            client.Close();
            return result;
        }

        public double Subtract(double n1, double n2)
        {
            CalculatorClient client = new CalculatorClient();
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            double result = client.Subtract(n1, n2);
            client.Close();
            return result;
        }

        public double Multiply(double n1, double n2)
        {
            CalculatorClient client = new CalculatorClient();
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            double result = client.Multiply(n1, n2);
            client.Close();
            return result;
        }

        public double Divide(double n1, double n2)
        {
            CalculatorClient client = new CalculatorClient();
            client.ClientCredentials.UserName.UserName = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            double result = client.Divide(n1, n2);
            client.Close();
            return result;
        }

        // Host the service within this EXE console application.
        static void Main()
        {
            // Create a ServiceHost for the CalculatorService type and provide the base address.
            using (ServiceHost serviceHost = new ServiceHost(typeof(FacadeService)))
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
            // check that username matches password
            if (null == userName || userName != password)
            {
                Console.WriteLine("Invalid username or password");
                throw new SecurityTokenValidationException("Invalid username or password");
            }
        }
    }
}


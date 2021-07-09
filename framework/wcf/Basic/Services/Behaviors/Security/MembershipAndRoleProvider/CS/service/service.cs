//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.Security.Permissions;
using System.Security.Principal;

using System.ServiceModel;
using System.ServiceModel.Activation;

using System.Threading;

using System.Web.Security;

namespace Microsoft.Samples.MembershipAndRoleProvider
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.MembershipAndRoleProvider")]
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
    public class CalculatorService : ICalculator
    {
        // Allows all Users to call the Add method
        [PrincipalPermission(SecurityAction.Demand, Role = "Users")]
        public double Add(double n1, double n2)
        {
            double result = n1 + n2;
            return result;
        }

        // Allows all Users to call the Subtract method
        [PrincipalPermission(SecurityAction.Demand, Role = "Users")]
        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;
            return result;
        }

        // Only allow Registered Users to call the Multiply method
        [PrincipalPermission(SecurityAction.Demand, Role = "Registered Users")]
        public double Multiply(double n1, double n2)
        {
            double result = n1 * n2;
            return result;
        }

        // Only allow Super Users to call the Divide method
        [PrincipalPermission(SecurityAction.Demand, Role = "Super Users")]
        public double Divide(double n1, double n2)
        {
            double result = n1 / n2;
            return result;
        }
    }

    public class CalculatorServiceHostFactory : ServiceHostFactoryBase
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            return new CalculatorServiceHost(baseAddresses);
        }
    }

    class CalculatorServiceHost : ServiceHost
    {
        #region CalculatorServiceHost Constructor
        /// <summary>
        /// Constructs a CalculatorServiceHost. Calls into SetupUsersAndroles to 
        /// set up the user and roles that the CalculatorService allows
        /// </summary>
        public CalculatorServiceHost(params Uri[] addresses)
            : base(typeof(CalculatorService), addresses)
        {
            SetupUsersAndRoles();
        }
        #endregion
        
        /// <summary>
        /// Sets up the user and roles that the CalculatorService allows
        /// </summary>
        internal static void SetupUsersAndRoles()
        {
            // Create some arrays for membership and role data
            string[] users = { "Alice", "Bob", "Charlie" };
            string[] emails = { "alice@example.org", "bob@example.org", "charlie@example.org" };
            string[] passwords = { "ecilA-123", "treboR-456", "eilrahC-789" };
            string[] roles = { "Super Users", "Registered Users", "Users" };

            // Clear out existing user information and add fresh user information
            for (int i = 0; i < emails.Length; i++)
            {
                if (Membership.GetUserNameByEmail(emails[i]) != null)
                    Membership.DeleteUser(users[i], true);

                Membership.CreateUser(users[i], passwords[i], emails[i]);
            }

            // Clear out existing role information and add fresh role information
            // This puts Alice, Bob and Charlie in the Users Role, Alice and Bob 
            // in the Registered Users Role and just Alice in the Super Users Role.
            for (int i = 0; i < roles.Length; i++)
            {
                if (Roles.RoleExists(roles[i]))
                {
                    foreach (string u in Roles.GetUsersInRole(roles[i]))
                        Roles.RemoveUserFromRole(u, roles[i]);

                    Roles.DeleteRole(roles[i]);
                }

                Roles.CreateRole(roles[i]);

                string[] userstoadd = new string[i + 1];

                for (int j = 0; j < userstoadd.Length; j++)
                    userstoadd[j] = users[j];

                Roles.AddUsersToRole(userstoadd, roles[i]);
            }
        }
    }
}


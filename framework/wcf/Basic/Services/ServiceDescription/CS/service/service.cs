
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

using System.ServiceModel.Description;

using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    public interface IServiceDescriptionCalculator
    {
        [OperationContract]
        int Add(int n1, int n2);
        [OperationContract]
        int Subtract(int n1, int n2);
        [OperationContract]
        int Multiply(int n1, int n2);
        [OperationContract]
        int Divide(int n1, int n2);
        [OperationContract]
        string GetServiceDescriptionInfo();
    }

    // Service class which implements the service contract.
    public class CalculatorService : IServiceDescriptionCalculator
    {
        public int Add(int n1, int n2)
        {
            return n1 + n2;
        }

        public int Subtract(int n1, int n2)
        {
            return n1 - n2;
        }

        public int Multiply(int n1, int n2)
        {
            return n1 * n2;
        }

        public int Divide(int n1, int n2)
        {
            return n1 / n2;
        }

        // Obtain information from the service description as return it as a multi-line string.

        public string GetServiceDescriptionInfo()
        {
            string info = "";

            OperationContext operationContext = OperationContext.Current;
            ServiceHost host = (ServiceHost)operationContext.Host;
            ServiceDescription desc = host.Description;

            // Enumerate the base addresses in the service host.

            info += "Base addresses:\n";
            foreach (Uri uri in host.BaseAddresses)
            {
                info += "    " + uri + "\n";
            }

            // Enumerate the service endpoints in the service description

            info += "Service endpoints:\n";
            foreach (ServiceEndpoint endpoint in desc.Endpoints)
            {
                info += "    Address:  " + endpoint.Address + "\n";
                info += "    Binding:  " + endpoint.Binding.Name + "\n";
                info += "    Contract: " + endpoint.Contract.Name + "\n";
            }

            return info;
        }
    }
 
}


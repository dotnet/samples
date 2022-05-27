
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.AddressHeaders
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.AddressHeaders")]
    public interface IHello
    {
        [OperationContract]
        string Hello();
    }

    // Service class which implements the service contract.
    public class HelloService : IHello
    {
        public static readonly string IDName = "ID";
        public static readonly string IDNamespace = "http://Microsoft.Samples.AdressHeaders";
    
        public string Hello() 
        {
            string id = null;
            // look at headers on incoming message
            for (int i = 0; i < OperationContext.Current.IncomingMessageHeaders.Count; ++i)
            {
                MessageHeaderInfo h = OperationContext.Current.IncomingMessageHeaders[i];
                // for any reference parameters with the correct name & namespace ...
                if (h.IsReferenceParameter && h.Name == IDName && h.Namespace == IDNamespace)
                {
                    // ... read the value of that header
                    XmlReader xr = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(i);
                    id = xr.ReadElementContentAsString();
                }
            }
            // return a value that includes the info from the reference parameter
            return "Hello, " + id; 
        }
    }
}

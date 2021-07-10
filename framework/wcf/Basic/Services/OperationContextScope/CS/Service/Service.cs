//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples", SessionMode = SessionMode.Allowed)]
    public interface IMessageHeaderReader
    {
        [OperationContract]
        bool RetrieveHeader(String guid);
    }

    //Utility class that acts as the header that client will add to list of incoming headers
    public static class CustomHeader
    {
        public static readonly String HeaderName = "MessageHeaderGUID";
        public static readonly String HeaderNamespace = "http://Microsoft.ServiceModel.Samples/GUID";
    }

    //Use a singleton so that its the same Instance that will determine whether the header is present or not
    //for all incoming channels
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Single)]
    public class MessageHeaderReader : IMessageHeaderReader
    {
        #region IMessageHeaderService Members

        //This method will try to retrieve a header whose value matches the GUID passed by the client.
        //The return value will notify the client whether the service was able to retrieve the header or not.
        public bool RetrieveHeader(string guid)
        {
            MessageHeaders messageHeaderCollection = OperationContext.Current.IncomingMessageHeaders;
            String guidHeader = null;

            Console.WriteLine("Trying to check if IncomingMessageHeader collection contains header with value {0}", guid);
            if (messageHeaderCollection.FindHeader(CustomHeader.HeaderName, CustomHeader.HeaderNamespace) != -1)
            {
                guidHeader = messageHeaderCollection.GetHeader<String>(CustomHeader.HeaderName, CustomHeader.HeaderNamespace);
            }
            else
            {
                Console.WriteLine("No header was found");
            }

            if (guidHeader != null)
            {
                Console.WriteLine("Found header with value {0}. Does it match with GUID sent as parameter: {1}", guidHeader, guidHeader.Equals(guid));
            }

            Console.WriteLine();
            //Return true if header is present and equals the guid sent by client as argument
            return (guidHeader != null && guidHeader.Equals(guid));
        }

        #endregion
    }

    public class SampleServiceHost
    {
        public static void Main(String[] args)
        {
            // Create a ServiceHost for the MessageHeaderReader service type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(MessageHeaderReader)))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }          
        }
    }
}


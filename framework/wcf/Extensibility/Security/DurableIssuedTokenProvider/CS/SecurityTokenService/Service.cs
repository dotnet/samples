//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.Security.Permissions;

using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Samples.DurableIssuedTokenProvider
{
    class Service
    {
        static void Main(string[] args)
        {
            // Create ServiceHost. 
            ServiceHost sh = new ServiceHost(typeof(SecurityTokenService));
            sh.Open();

            try
            {
                foreach (ChannelDispatcher cd in sh.ChannelDispatchers)
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                        Console.WriteLine("STS listening at {0}", ed.EndpointAddress.Uri);

                Console.WriteLine("\nPress enter to exit\n");
                Console.ReadLine();
            }
            finally
            {
                sh.Close();
            }
        }
    }
}


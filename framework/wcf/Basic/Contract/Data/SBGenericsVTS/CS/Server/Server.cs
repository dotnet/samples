//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Microsoft.Samples.SBGenericsVTS.GenericsVTSSerializationBinder;
using Microsoft.Samples.SBGenericsVTS.SharedCode;

namespace Microsoft.Samples.SBGenericsVTS.Server
{
    public class MyServer
    {
        [STAThread]
        static void Main(string[] args)
        {
            IDictionary props = new Hashtable();
            props["port"] = 1243;
            TcpChannel channel = new TcpChannel(props, null, new GenericsVTSSBSinkProvider());
            ChannelServices.RegisterChannel(channel, false);

            // Register several types of service
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Service<Customer>), "ListCustomerObject.rem", WellKnownObjectMode.SingleCall);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Service<string>), "ListStringObject.rem", WellKnownObjectMode.SingleCall);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Service<List<int>>), "ListListIntObject.rem", WellKnownObjectMode.SingleCall);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Service<int[]>), "ListArrayIntObject.rem", WellKnownObjectMode.SingleCall);

            Console.WriteLine("The server is running.");
            Console.WriteLine("");
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.ReadLine();
        }
    }
}

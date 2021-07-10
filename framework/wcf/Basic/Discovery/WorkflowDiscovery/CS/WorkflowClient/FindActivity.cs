//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml;

namespace Microsoft.Samples.Discovery
{
    public class FindActivity : CodeActivity
    {
        public OutArgument<Uri> DiscoveredEndpointUri { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            FindCriteria findCriteria = new FindCriteria();
            // Create a contract to add to the findCriteria. The search will be based on this contract.
            findCriteria.ContractTypeNames.Add(new XmlQualifiedName("IPrintService", "http://tempuri.org/"));
            findCriteria.MaxResults = 1;
            findCriteria.Duration = new TimeSpan(0, 0, 0, 3);

            FindResponse findResponse = discoveryClient.Find(findCriteria);
            discoveryClient.Close();

            if (findResponse.Endpoints.Count == 0)
            {
                throw new EndpointNotFoundException("Client was unable to find any matching endpoints using Discovery.");
            }

            this.DiscoveredEndpointUri.Set(context, findResponse.Endpoints[0].Address.Uri);
        }
    }
}

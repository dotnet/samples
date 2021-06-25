//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;

using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{
    // MessageContract is used as opposed to DataContract so that a WCF client
    // can communicate with WF Receive activity without using the parameterized
    // version of the Receive activity.
    [MessageContract(IsWrapped = false)]
    public class StartApprovalParams
    {
        ApprovalRequest request;
        Uri serviceAddress;

        [MessageBodyMember]
        public ApprovalRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        [MessageBodyMember]
        public Uri ServiceAddress
        {
            get { return serviceAddress; }
            set { serviceAddress = value; }
        }

        public StartApprovalParams() { }

        public StartApprovalParams(ApprovalRequest newRequest, Uri newServiceAddress)
        {
            request = newRequest;
            serviceAddress = newServiceAddress;
        }
    }
}

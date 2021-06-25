//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;

using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{
    [ServiceContract]
    interface IClientApproval
    {
        // Called by ApprovalManager seeking approval of a document
        [OperationContract(IsOneWay=true)]
        void RequestClientResponse(ApprovalRequest docApprovalRequest);

        // Called by ApprovalManager informing client that a approval request has been canceled
        [OperationContract(IsOneWay = true)]
        void CancelRequestClientResponse(ApprovalRequest docApprovalRequest);
    }

    class ApprovalRequestsService : IClientApproval
    {
        public void RequestClientResponse(ApprovalRequest docApprovalRequest)
        {
            ExternalToMainComm.NewApprovalRequest(docApprovalRequest);
        }

        public void CancelRequestClientResponse(ApprovalRequest docApprovalRequest)
        {
            ExternalToMainComm.CancelApprovalRequest(docApprovalRequest);
        }
    }
}

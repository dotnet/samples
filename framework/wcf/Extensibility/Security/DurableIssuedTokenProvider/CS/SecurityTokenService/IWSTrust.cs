//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.DurableIssuedTokenProvider
{
    [ServiceContract]
    interface IWSTrust
    {
        [OperationContract(Action = Constants.Trust.Actions.Issue, ReplyAction = Constants.Trust.Actions.IssueReply)]
        Message Issue(Message request);
    }
}


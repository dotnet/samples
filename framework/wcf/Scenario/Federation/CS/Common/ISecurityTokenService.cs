//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.Federation
{
	[ServiceContract(Name = "SecurityTokenService", Namespace = "http://tempuri.org")]
	public interface ISecurityTokenService
	{
        [OperationContract(Action = Constants.Trust.Actions.Issue,
                           ReplyAction = Constants.Trust.Actions.IssueReply)]        
		Message ProcessRequestSecurityToken(Message message);
	}
}


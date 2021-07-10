' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.Samples.Federation

    <ServiceContract(Name:="SecurityTokenService", [Namespace]:="http://tempuri.org")> _
    Public Interface ISecurityTokenService

        <OperationContract(Action:=Constants.Trust.Actions.Issue, ReplyAction:=Constants.Trust.Actions.IssueReply)> _
        Function ProcessRequestSecurityToken(ByVal message As Message) As Message

    End Interface

End Namespace


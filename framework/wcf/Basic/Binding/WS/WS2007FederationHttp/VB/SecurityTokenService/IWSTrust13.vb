' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System

Imports System.ServiceModel
Imports System.ServiceModel.Channels


Namespace Microsoft.Samples.WS2007FederationHttpBinding
    <ServiceContract()> _
    Interface IWSTrust13
        <OperationContract(Action:=Constants.Trust13.Actions.Issue, ReplyAction:=Constants.Trust13.Actions.IssueReply)> _
        Function Issue(ByVal request As Message) As Message
    End Interface
End Namespace

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

// This file contains the contents of automatically generated clients (using svcutil) for the various 
// services used via WCF.  Service References could have been used here as well.


using System.Runtime.Serialization;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{

    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IApprovalProcess")]
    public interface IApprovalProcess
    {
        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IApprovalProcess/RequestApprovalOf")]
        void RequestApprovalOf(RequestApprovalOf request);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IApprovalProcess/ResponsedToApprovalRequest")]
        void ResponsedToApprovalRequest(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalResponse response);
    }

    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class RequestApprovalOf
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Samples.DocumentApprovalProcess" +
            ".ApprovalMessageContractLibrary", Order = 0)]
        public Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalRequest ApprovalRequest;

        public RequestApprovalOf()
        {
        }

        public RequestApprovalOf(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalRequest ApprovalRequest)
        {
            this.ApprovalRequest = ApprovalRequest;
        }
    }

    public interface IApprovalProcessChannel : IApprovalProcess, System.ServiceModel.IClientChannel
    {
    }

    public partial class ApprovalProcessClient : System.ServiceModel.ClientBase<IApprovalProcess>, IApprovalProcess
    {

        public ApprovalProcessClient()
        {
        }

        public ApprovalProcessClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ApprovalProcessClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ApprovalProcessClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ApprovalProcessClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void IApprovalProcess.RequestApprovalOf(RequestApprovalOf request)
        {
            base.Channel.RequestApprovalOf(request);
        }

        public void RequestApprovalOf(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalRequest ApprovalRequest)
        {
            RequestApprovalOf inValue = new RequestApprovalOf();
            inValue.ApprovalRequest = ApprovalRequest;
            ((IApprovalProcess)(this)).RequestApprovalOf(inValue);
        }

        public void ResponsedToApprovalRequest(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalResponse response)
        {
            base.Channel.ResponsedToApprovalRequest(response);
        }
    }


    //------------------

    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IApprovalResults")]
    public interface IApprovalResults
    {
        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IApprovalResults/StartGetApproval")]
        void StartGetApproval(StartApprovalParams request);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IApprovalResults/ApprovalProcessResults")]
        void ApprovalProcessResults(ApprovalResponse request);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IApprovalResults/CancelApprovalRequest")]
        void CancelApprovalRequest(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalRequest request);
    }

    public interface IApprovalResultsChannel : IApprovalResults, System.ServiceModel.IClientChannel
    {
    }

    public partial class ApprovalResultsClient : System.ServiceModel.ClientBase<IApprovalResults>, IApprovalResults
    {

        public ApprovalResultsClient()
        {
        }

        public ApprovalResultsClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ApprovalResultsClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ApprovalResultsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ApprovalResultsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void IApprovalResults.StartGetApproval(StartApprovalParams request)
        {
            base.Channel.StartGetApproval(request);
        }

        public void StartGetApproval(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalRequest Request, System.Uri ServiceAddress)
        {
            StartApprovalParams inValue = new StartApprovalParams();
            inValue.Request = Request;
            inValue.ServiceAddress = ServiceAddress;
            ((IApprovalResults)(this)).StartGetApproval(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void IApprovalResults.ApprovalProcessResults(ApprovalResponse request)
        {
            base.Channel.ApprovalProcessResults(request);
        }

        public void ApprovalProcessResults(bool Approved, System.Guid Id)
        {
            ApprovalResponse inValue = new ApprovalResponse();
            inValue.Approved = Approved;
            inValue.Id = Id;
            ((IApprovalResults)(this)).ApprovalProcessResults(inValue);
        }

        public void CancelApprovalRequest(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.ApprovalRequest request)
        {
            base.Channel.CancelApprovalRequest(request);
        }
    }

    //---------------

    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "ISubscriptionService")]
    public interface ISubscriptionService
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ISubscriptionService/Subscribe", ReplyAction = "http://tempuri.org/ISubscriptionService/SubscribeResponse")]
        Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User Subscribe(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User newuser);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ISubscriptionService/Unsubscribe", ReplyAction = "http://tempuri.org/ISubscriptionService/UnsubscribeResponse")]
        void Unsubscribe(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User id);
    }

    public interface ISubscriptionServiceChannel : ISubscriptionService, System.ServiceModel.IClientChannel
    {
    }

    public partial class SubscriptionServiceClient : System.ServiceModel.ClientBase<ISubscriptionService>, ISubscriptionService
    {

        public SubscriptionServiceClient()
        {
        }

        public SubscriptionServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public SubscriptionServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SubscriptionServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public SubscriptionServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User Subscribe(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User newuser)
        {
            return base.Channel.Subscribe(newuser);
        }

        public void Unsubscribe(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User id)
        {
            base.Channel.Unsubscribe(id);
        }
    }


    //---------------

    public interface IManageApproval
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ISubscriptionService/Subscribe", ReplyAction = "http://tempuri.org/ISubscriptionService/SubscribeResponse")]
        Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User Subscribe(Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary.User newuser);

    }
}
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Dispatcher;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{
    public sealed class ClientRequestApprovalWorkflow : Activity
    {
        public ClientRequestApprovalWorkflow()
        {
            Variable<CorrelationHandle> corr = new Variable<CorrelationHandle>();
            Variable<ApprovalResponse> response = new Variable<ApprovalResponse>();
            Variable<ApprovalRequest> cancelRequest = new Variable<ApprovalRequest> { Name = "CancelRequest" };
            Variable<StartApprovalParams> startParams = new Variable<StartApprovalParams>();
            Variable<CorrelationHandle> requestReplyHandle = new Variable<CorrelationHandle> { Name = "RequestReplyHandle" };

            // For correlating between approval requests and responses
            MessageQuerySet approvalMQS = new MessageQuerySet
            {
                {
                    "ApprovalProcessId",
                    new XPathMessageQuery("local-name()='Id'", new XPathMessageContext())
                }
            };
 
            this.Implementation = () => 
                new Sequence
                {
                    Variables = { corr, response, startParams, requestReplyHandle, cancelRequest },
                    Activities =
                    {
                        // Called by a local client proxy to trigger the creation of a new approval request
                        new Receive
                        {
                            OperationName = "StartGetApproval",
                            ServiceContractName = "IApprovalResults",
                            Content = new ReceiveMessageContent(new OutArgument<StartApprovalParams>(startParams)),
                            CanCreateInstance = true,
                        },
                        // Ask the approval manager to start the approval process
                        new Send
                        {
                            OperationName = "RequestApprovalOf",
                            ServiceContractName = "IApprovalProcess",
                            EndpointAddress = new InArgument<Uri>(env => startParams.Get(env).ServiceAddress),
                            Endpoint = new Endpoint
                            {
                                 Binding = new BasicHttpBinding(),
                            },
                            Content = new SendMessageContent(new InArgument<ApprovalRequest>(env => startParams.Get(env).Request)),
                            CorrelationInitializers = 
                            {
                                new QueryCorrelationInitializer
                                {
                                    CorrelationHandle = corr,
                                    MessageQuerySet = approvalMQS,
                                },
                            }
                        },
                        // Get a response from the approval manager or a request to cancel the approval
                        //  from the local client proxy (user presses the cancel request button on the ui)
                        new Pick
                        {
                            Branches =
                            {
                                new PickBranch
                                {
                                    Trigger = new Receive
                                    {
                                        OperationName = "ApprovalProcessResults",
                                        ServiceContractName = "IApprovalResults",
                                        CorrelatesWith = corr,
                                        CorrelatesOn = approvalMQS,
                                        Content = new ReceiveMessageContent(new OutArgument<ApprovalResponse>(response)),
                                    },
                                },
                                new PickBranch
                                {
                                    Trigger = new Receive
                                    {
                                        OperationName = "CancelApprovalRequest",
                                        ServiceContractName = "IApprovalResults",
                                        CorrelatesWith = corr,
                                        CorrelatesOn = approvalMQS,
                                        Content = new ReceiveMessageContent(new OutArgument<ApprovalRequest>(cancelRequest)),
                                    },
                                    // Send cancelation to the approval manager to pass on to clients approving document
                                    Action = new Sequence
                                    {
                                        Activities = 
                                        {
                                            new Send
                                            {
                                                OperationName = "CancelApprovalRequest",
                                                ServiceContractName = "IApprovalProcess",
                                                EndpointAddress = new InArgument<Uri>(env => startParams.Get(env).ServiceAddress),
                                                Endpoint = new Endpoint
                                                {
                                                    Binding = new BasicHttpBinding(),
                                                },
                                                Content = new SendMessageContent(new InArgument<ApprovalRequest>(env => startParams.Get(env).Request)),
                                            },
                                            new TerminateWorkflow
                                            {
                                                Reason = "Approval Request Canceled",
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new InvokeMethod
                        {
                            TargetType = typeof(ExternalToMainComm),
                            MethodName = "WriteStatusLine",
                            Parameters = 
                            {
                                new InArgument<String>("Got an approval response..."),
                            },
                        },
                        // update ui with new response
                        new InvokeMethod
                        {
                            TargetType = typeof(ExternalToMainComm),
                            MethodName = "ApprovalRequestResponse",
                            Parameters =
                            {
                                new InArgument<ApprovalResponse>(response),
                            }
                        },
                    }
                };
        }
    }
}

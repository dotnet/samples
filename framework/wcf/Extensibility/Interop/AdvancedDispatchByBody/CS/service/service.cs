
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.AdvancedDispatchBody
{
    /// <summary>
    /// The IDispatchByBody contract is decorated with the Dispatch-
    /// ByBodyElementBehaviorAttribute, which is a contract behavior.
    /// The behavior injects an operation selector into the dispatch 
    /// runtime that dispatches messages by the qualified name of the
    /// message body element.
    /// Except the DefaultOperation method, the methods on this service 
    /// contract are decorated with the DispatchBodyElementAttribute,
    /// which contains the metadata indicating which body element 
    /// corresponds to the respective method.
    /// All operations also have a wildcard for the ReplyAction set,
    /// indicating that the responses don't have a well-known reply
    /// action.
    /// </summary>
    [ServiceContract(Namespace = "http://Microsoft.Samples.AdvancedDispatchBody"),
     DispatchByBodyElementBehavior]
    public interface IDispatchedByBody
    {
        [OperationContract(ReplyAction="*"), 
         DispatchBodyElement("bodyA","http://tempuri.org")]
        Message OperationForBodyA(Message msg);
        [OperationContract(ReplyAction = "*"), 
         DispatchBodyElement("bodyB", "http://tempuri.org")]
        Message OperationForBodyB(Message msg);
        [OperationContract(Action="*", ReplyAction="*")]
        Message DefaultOperation(Message msg);
    }

    /// <summary>
    /// This class implements the sample service. The methods of this 
    /// service accept the incoming message and wrap its body content
    /// into a reply message with a distinct body element. 
    /// </summary>
    public class BodyDispatchedService : IDispatchedByBody
    {
        #region IDispatchedByBody Members

        public Message OperationForBodyA(Message msg)
        {
            // create reply for this operation, wrapping the original request body
            XmlDocument responseBody = new XmlDocument();
            responseBody.AppendChild(responseBody.CreateElement("replyBodyA", "http://tempuri.org"));
            responseBody.DocumentElement.AppendChild(responseBody.ReadNode(msg.GetReaderAtBodyContents()));
            Message reply = Message.CreateMessage(
                msg.Version, 
                null, 
                new XmlNodeReader(responseBody.DocumentElement));
            return reply;
        }

        public Message OperationForBodyB(Message msg)
        {
            // create reply for this operation, wrapping the original request body
            XmlDocument responseBody = new XmlDocument();
            responseBody.AppendChild(responseBody.CreateElement("replyBodyB", "http://tempuri.org"));
            responseBody.DocumentElement.AppendChild(responseBody.ReadNode(msg.GetReaderAtBodyContents()));
            Message reply = Message.CreateMessage(
                msg.Version,
                null,
                new XmlNodeReader(responseBody.DocumentElement));
            return reply;
        }

        public Message DefaultOperation(Message msg)
        {
            // create reply for this operation, wrapping the original request body
            XmlDocument responseBody = new XmlDocument();
            responseBody.AppendChild(responseBody.CreateElement("replyDefault", "http://tempuri.org"));
            responseBody.DocumentElement.AppendChild(responseBody.ReadNode(msg.GetReaderAtBodyContents()));
            Message reply = Message.CreateMessage(
                msg.Version,
                null,
                new XmlNodeReader(responseBody.DocumentElement));
            return reply;
        }

        #endregion
    }

}


//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Runtime.Serialization;

namespace Microsoft.ServiceModel.Samples
{
    [DataContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public class Greeting
    {
        [DataMember]
        string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    [DataContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public class GreetingResponse
    {
        [DataMember]
        string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    public interface IHelloService
    {
        [OperationContract()]
        GreetingResponse Hello(Greeting greeting);
        [OperationContract()]
        Message HelloToo(Message msg);
    }

    // Service class which implements the service contract.
    
    public class HelloService : IHelloService
    {
        public HelloService()
        {
            
        }
        public GreetingResponse Hello(Greeting greeting)
        {
            GreetingResponse response = new GreetingResponse();
            response.Text = "Hello Client!";
            return response;
        }

        public Message HelloToo(Message greeting)
        {
            string ns = "http://Microsoft.ServiceModel.Samples";

            XmlDocument xmlFactory = new XmlDocument();
            XmlElement responseBodyElement = xmlFactory.CreateElement("HelloResponse", ns);
            XmlElement responseContentElement = xmlFactory.CreateElement("HelloResultX", ns); // <--
            XmlElement responseParameterElement = xmlFactory.CreateElement("text", ns);
            responseParameterElement.InnerText = "Hello Client!";
            responseContentElement.AppendChild(responseParameterElement);
            responseBodyElement.AppendChild(responseContentElement);

            return Message.CreateMessage(greeting.Version, "http://Microsoft.ServiceModel.Samples/IHelloService/HelloTooResponse", new XmlNodeReader(responseBodyElement));
        }

    }

}

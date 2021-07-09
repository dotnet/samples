
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;

namespace Microsoft.Samples.AdvancedDispatchBody
{
    
    //Client implementation code.
    class Client
    {
        const string nullAction = null;
        const string requestBodyA = "<q:bodyA xmlns:q=\"http://tempuri.org\">test</q:bodyA>";
        const string requestBodyB = "<q:bodyB xmlns:q=\"http://tempuri.org\">test</q:bodyB>";
        const string requestBodyX = "<q:bodyX xmlns:q=\"http://tempuri.org\">test</q:bodyX>";


        static void SendRequest(string requestBodyXml)
        {
            Message requestMessage;
            Message replyMessage;


            GenericClient genericClient = new GenericClient();
            requestMessage = Message.CreateMessage(
                                    MessageVersion.Default,
                                    nullAction,
                                    XmlReader.Create(new StringReader(requestBodyXml)));
            replyMessage = genericClient.ProcessMessage(requestMessage);
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(Console.Out));
            replyMessage.WriteBodyContents(writer);
            writer.Flush();
            Console.WriteLine();
        }

        static void Main()
        {
            SendRequest(requestBodyA);
            SendRequest(requestBodyB);
            SendRequest(requestBodyX);
        }
    }
}

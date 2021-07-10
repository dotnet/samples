
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Xml.Schema;


namespace Microsoft.ServiceModel.Samples
{
    //Client implementation code.
    class Client
    {
        const string ns = "http://Microsoft.ServiceModel.Samples";
        const string helloAction = "http://Microsoft.ServiceModel.Samples/IHelloService/Hello";
        const string helloTooAction = "http://Microsoft.ServiceModel.Samples/IHelloService/HelloToo";

        static void Main()
        {
            Console.WriteLine("*** Call 'Hello' with svcutil-generated client");
            HelloServiceClient helloClient = new HelloServiceClient();

            Greeting greeting = new Greeting();
            greeting.text = "Hello Server!";
            GreetingResponse response = helloClient.Hello(greeting);
            Console.WriteLine(response.text);
            helloClient.Close();

            try
            {
                Console.WriteLine("*** Call 'Hello' with generic client, no client behavior");
                GenericClient client = new GenericClient();

                Console.WriteLine("--- Sending valid client request:");
                GenericCallValid(client, helloAction);
                Console.WriteLine("--- Sending invalid client request:");
                GenericCallInvalid(client, helloAction);
                client.Close();

            }
            catch (Exception e)
            {
                DumpException(e);
            }

            try
            {
                Console.WriteLine("*** Call 'Hello' with generic client, with client behavior");
                GenericClient client = new GenericClient();

                // Configure client programmatically, adding behavior
                XmlSchema schema = XmlSchema.Read(new StreamReader("messages.xsd"), null);
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(schema);
                client.Endpoint.Behaviors.Add(new SchemaValidationBehavior(schemaSet, true, true));

                Console.WriteLine("--- Sending valid client request:");
                GenericCallValid(client, helloAction);
                Console.WriteLine("--- Sending invalid client request:");
                GenericCallInvalid(client, helloAction);

                client.Close();
            }
            catch (Exception e)
            {
                DumpException(e);
            }

            Console.WriteLine("*** Call 'HelloToo' with generic client, no client behavior");
            try
            {
                GenericClient client = new GenericClient();

                Console.WriteLine("--- Sending valid client request, malformed service reply:");
                GenericCallValid(client, helloTooAction);
                client.Close();

            }
            catch (Exception e)
            {
                DumpException(e);
            }

            Console.WriteLine("*** Call 'HelloToo' with generic client, with client behavior, no service behavior");
            try
            {
                GenericClient client = new GenericClient();

                XmlSchema schema = XmlSchema.Read(new StreamReader("messages.xsd"), null);
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(schema);

                client.Endpoint.Address = new EndpointAddress(client.Endpoint.Address.Uri.ToString() + "/novalidation");
                client.Endpoint.Behaviors.Add(new SchemaValidationBehavior(schemaSet, true, true));

                Console.WriteLine("--- Sending valid client request, malformed service reply:");
                GenericCallValid(client, helloTooAction);
                client.Close();
            }
            catch (Exception e)
            {
                DumpException(e);
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void DumpException(Exception e)
        {
            if (e is RequestClientValidationException)
            {
                Console.WriteLine("> Call failed client-side; invalid request\nMessage: {0}", e.Message);
            }
            else if (e is ReplyClientValidationException)
            {
                Console.WriteLine("> Call failed client-side; invalid reply\nMessage: {0}", e.Message);
            }
            else if (e is FaultException)
            {
                FaultException fault = e as FaultException;
                Console.WriteLine("> Call failed service-side:\nFault code: {0}:{1}\nFault reason:{2}",
                                  fault.Code.Namespace, fault.Code.Name,
                                  fault.Reason.GetMatchingTranslation().Text);
            }
            else
            {
                Console.WriteLine("> Call failed: {0}\nMessage: {1}", e.GetType().Name, e.Message);
            }
        }

        static void CallGeneric(GenericClient genericClient, string action, XmlNode messageBody)
        {
            // Build the message
            Message response;
            Message request;

            request = Message.CreateMessage(
                MessageVersion.Default,
                action,
                new XmlNodeReader(messageBody));

            // send request
            response = genericClient.Request(request);

            // dump the result
            if (!response.IsFault)
            {
                XmlReader resultBody = response.GetReaderAtBodyContents();
                resultBody.ReadToDescendant("text", "http://Microsoft.ServiceModel.Samples");
                string responseText = resultBody.ReadElementContentAsString("text", "http://Microsoft.ServiceModel.Samples");
                Console.WriteLine(responseText);
            }
            else
            {
                MessageFault fault = MessageFault.CreateFault(response, 8192);
                throw new FaultException(fault);
            }

            Console.WriteLine();
        }


        static void GenericCallValid(GenericClient client, string action)
        {
            XmlDocument xmlFactory = new XmlDocument();
            XmlElement requestBodyElement = xmlFactory.CreateElement("Hello", ns);
            XmlElement requestContentElement = xmlFactory.CreateElement("greeting", ns);
            XmlElement requestParameterElement = xmlFactory.CreateElement("text", ns);
            requestParameterElement.InnerText = "Hello Server!";
            requestContentElement.AppendChild(requestParameterElement);
            requestBodyElement.AppendChild(requestContentElement);

            CallGeneric(client, action, requestBodyElement);
        }

        static void GenericCallInvalid(GenericClient client, string action)
        {
            XmlDocument xmlFactory = new XmlDocument();
            XmlElement requestBodyElement = xmlFactory.CreateElement("Hello", ns);
            XmlElement requestContentElement = xmlFactory.CreateElement("greeting", ns);
            XmlElement requestParameterElement = xmlFactory.CreateElement("incorrect", ns); // <--
            requestParameterElement.InnerText = "Hello Server!";
            requestContentElement.AppendChild(requestParameterElement);
            requestBodyElement.AppendChild(requestContentElement);

            CallGeneric(client, action, requestBodyElement);
        }


    }
}

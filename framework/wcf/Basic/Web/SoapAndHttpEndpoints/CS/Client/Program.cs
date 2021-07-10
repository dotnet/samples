//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Samples.SoapAndHttpEndpoints.ServiceReference;

namespace Microsoft.Samples.SoapAndHttpEndpoints
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Invoking service using SOAP client");
            using (ServiceClient proxy = new ServiceClient())
            {
                Console.WriteLine("Calling PutData with value 42");
                Console.WriteLine(proxy.PutData(42));
                Console.WriteLine("GetData returned: " + proxy.GetData());
            }

            Console.WriteLine("Invoking service using HTTP");
            Uri baseAddress = new Uri("http://localhost:33692/Service.svc/Http/");
            Console.WriteLine("Service help page is at: " + baseAddress.AbsoluteUri + "help");
            Console.WriteLine("");

            using (WebClient httpClient = new WebClient())
            {
                httpClient.BaseAddress = baseAddress.AbsoluteUri;
                httpClient.Headers[HttpRequestHeader.ContentType] = "application/xml";
                Console.WriteLine("Calling PutData with value 24");
                Console.WriteLine(httpClient.UploadString("PutData", "<int xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">24</int>"));
                string xmlResponse = httpClient.DownloadString("GetData");
                using (StringReader stringReader = new StringReader(xmlResponse))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        string value = (string)new DataContractSerializer(typeof(string)).ReadObject(xmlReader);
                        Console.WriteLine("GetData returned: " + value);
                    }
                }             
            }

            Console.WriteLine("");
            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }
    }
}

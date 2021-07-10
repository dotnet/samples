//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Web;
using System.Xml;

namespace Microsoft.Samples.WeaklyTypedJson
{
    class Program
    {
        static void Main()
        {
            Uri baseAddress = new Uri("http://localhost:8000/");

            using (WebServiceHost host = new WebServiceHost(typeof(ServerSideProfileService), baseAddress))
            {
                // Open the service host, service is now listening
                host.Open();

                Console.WriteLine("Service listening at {0}.", baseAddress);
                Console.WriteLine("To view its JSON output, point your web browser to {0}GetMemberProfile.", baseAddress);
                Console.WriteLine();


                using (WebChannelFactory<IClientSideProfileService> cf = new WebChannelFactory<IClientSideProfileService>(baseAddress))
                {
                    // Create client side proxy
                    IClientSideProfileService channel = cf.CreateChannel();

                    // Make a request to the service and get the Json response
                    XmlDictionaryReader reader = channel.GetMemberProfile().GetReaderAtBodyContents();

                    // Go through the Json as though it's a dictionary. There is no need to map it to a CLR type.
                    JsonObject json = new JsonObject(reader);


                    string name = json["root"]["personal"]["name"];
                    int age = json["root"]["personal"]["age"];
                    double height = json["root"]["personal"]["height"];
                    bool isSingle = json["root"]["personal"]["isSingle"];
                    int[] luckyNumbers = {
                                             json["root"]["personal"]["luckyNumbers"][0],
                                             json["root"]["personal"]["luckyNumbers"][1],
                                             json["root"]["personal"]["luckyNumbers"][2] 
                                         };
                    string[] favoriteBands = {
                                                 json["root"]["favoriteBands"][0],
                                                 json["root"]["favoriteBands"][1]
                                             };

                    Console.WriteLine("This is {0}'s page. I am {1} years old and I am {2} meters tall.", 
                        name, age, height);
                    Console.WriteLine("I am {0}single.",(isSingle) ? "" : "not ");
                    Console.WriteLine("My lucky numbers are {0}, {1}, and {2}.",
                        luckyNumbers[0], luckyNumbers[1], luckyNumbers[2]);
                    Console.WriteLine("My favorite bands are {0} and {1}.", 
                        favoriteBands[0], favoriteBands[1]);
                }

                Console.WriteLine();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();
            }
        }
    }
}

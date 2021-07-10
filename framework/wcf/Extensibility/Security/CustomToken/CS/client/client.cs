//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.CustomToken
{
    //Client implementation code.
    class Client
    {
        static void Main()
        {
            string creditCardNumber = "11111111";
            DateTime expirationTime = new DateTime(2010, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc);
            string issuer = Constants.TestCreditCardIssuer;
            ChannelFactory<IEchoService> channelFactory = null;
            IEchoService client = null;


            Console.WriteLine("Please enter your credit card information:");
            Console.WriteLine("Please enter a credit card number (hit [enter] to pick 11111111 - a valid number on file): ");
            string input = Console.ReadLine();
            if (input.Trim() != string.Empty)
            {
                creditCardNumber = input;
                bool readDate = false;
                while (!readDate)
                {
                    Console.WriteLine("Please enter the expiration time (yyyy/mm/dd): ");
                    input = Console.ReadLine().Trim();
                    try
                    {
                        expirationTime = DateTime.Parse(input, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AdjustToUniversal);
                        readDate = true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("You did not enter a valid date format. Please try again");
                    }
                }
                Console.WriteLine("Please enter the issuer of the credit card (hit enter to pick {0}", Constants.TestCreditCardIssuer);
                input = Console.ReadLine().Trim();
                if (input != string.Empty)
                    issuer = input;
            }
            try
            {
                Binding creditCardBinding = BindingHelper.CreateCreditCardBinding();
                EndpointAddress serviceAddress = new EndpointAddress("http://localhost/servicemodelsamples/service.svc");

                // Create a client with given client endpoint configuration
                channelFactory = new ChannelFactory<IEchoService>(creditCardBinding, serviceAddress);

                // configure the credit card credentials on the channel factory 
                CreditCardClientCredentials credentials = new CreditCardClientCredentials(new CreditCardInfo(creditCardNumber, issuer, expirationTime));

                // configure the service certificate on the credentials
                credentials.ServiceCertificate.SetDefaultCertificate("CN=localhost", StoreLocation.LocalMachine, StoreName.My);

                // replace ClientCredentials with CreditCardClientCredentials
                channelFactory.Endpoint.Behaviors.Remove(typeof(ClientCredentials));
                channelFactory.Endpoint.Behaviors.Add(credentials);

                client = channelFactory.CreateChannel();

                Console.WriteLine("Echo service returned: {0}", client.Echo());

                ((IChannel)client).Close();
                channelFactory.Close();
            }
            catch (CommunicationException e)
            {
                Abort((IChannel)client, channelFactory);

                // if there is a fault then print it out
                FaultException fe = null;
                Exception tmp = e;
                while (tmp != null)
                {
                    fe = tmp as FaultException;
                    if (fe != null)
                    {
                        break;
                    }
                    tmp = tmp.InnerException;
                }
                if (fe != null)
                {
                    Console.WriteLine("The server sent back a fault: {0}", fe.CreateMessageFault().Reason.GetMatchingTranslation().Text);
                }
                else
                {
                    Console.WriteLine("The request failed with exception: {0}", e);
                }
            }
            catch (TimeoutException)
            {
                Abort((IChannel)client, channelFactory);
                Console.WriteLine("The request timed out");
            }
            catch (Exception e)
            {
                Abort((IChannel)client, channelFactory);
                Console.WriteLine("The request failed with unexpected exception: {0}", e);
            }

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        static void Abort(IChannel channel, ChannelFactory cf)
        {
            if (channel != null)
                channel.Abort();
            if (cf != null)
                cf.Abort();
        }
    }
}



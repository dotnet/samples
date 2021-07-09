//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.MessageInterceptor
{
    // Create a service contract and define the service operations.
    [ServiceContract]
    public interface ISampleContract
    {
        [OperationContract(IsOneWay = true)]
        void ReportWindSpeed(int speed);
    }

    // The Service implementation implements your service contract.
    public class SampleService : ISampleContract
    {
        public void ReportWindSpeed(int speed)
        {
            if (speed >= 64)
            {
                Console.WriteLine("Dangerous wind detected! Reported speed (" + speed + ") is greater than 64 kph.");
            }
        }
    }

    class DroppingServerElement : InterceptingElement
    {
        protected override ChannelMessageInterceptor CreateMessageInterceptor()
        {
            return new DroppingServerInterceptor();
        }
    }

    public class DroppingServerInterceptor : ChannelMessageInterceptor
    {
        int messagesSinceLastReport = 0;
        readonly int reportPeriod = 5;

        public DroppingServerInterceptor() { }

        public override void OnReceive(ref Message msg)
        {
            if (msg.Headers.FindHeader("ByPass", "urn:InterceptorNamespace") > 0)
            {
                if (++messagesSinceLastReport == this.reportPeriod)
                {
                    Console.WriteLine(reportPeriod + " wind speed reports have been received.");
                }
                return;
            }
            // Drop incoming Message if the Message does not have the special header
            msg = null;
        }

        public override ChannelMessageInterceptor Clone()
        {
            return new DroppingServerInterceptor();
        }
    }

    class Service
    {
        static void Main(string[] args)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(SampleService), new Uri("http://localhost:8000/windspeed/"));
            bool success = false;

            try
            {
                serviceHost.Open();

                System.Console.WriteLine("Press ENTER to exit.");
                System.Console.ReadLine();

                serviceHost.Close();
                success = true;
            }
            finally
            {
                if (!success)
                    serviceHost.Abort();
            }
        }
    }
}

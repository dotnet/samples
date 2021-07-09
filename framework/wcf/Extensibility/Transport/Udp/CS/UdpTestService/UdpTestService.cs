//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.Udp
{
    [ServiceContract]
    public interface ICalculatorContract
    {
        [OperationContract]
        int Add(int x, int y);
    }

    [ServiceContract]
    public interface IDatagramContract
    {
        [OperationContract(IsOneWay = true)]
        void Hello();
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    class ConfigurableCalculatorService : CalculatorService
    {
    }

    class CalculatorService : IDatagramContract, ICalculatorContract
    {
        public int Add(int x, int y)
        {
            Console.WriteLine("   adding {0} + {1}", x, y);
            return (x + y);
        }

        public void Hello()
        {
            Console.Out.WriteLine("Hello, world!");
        }
    }

    class UdpTestService
    {
        static void ServiceFromCode()
        {
            Console.Out.WriteLine("Testing Udp From Code.");

            Binding datagramBinding = new CustomBinding(new BinaryMessageEncodingBindingElement(), new UdpTransportBindingElement());

            // using the 2-way calculator method requires a session since UDP is not inherently request-response
            SampleProfileUdpBinding calculatorBinding = new SampleProfileUdpBinding(true);
            calculatorBinding.ClientBaseAddress = new Uri("soap.udp://localhost:8003/");

            Uri calculatorAddress = new Uri("soap.udp://localhost:8001/");
            Uri datagramAddress = new Uri("soap.udp://localhost:8002/datagram");

            // we need an http base address so that svcutil can access our metadata
            ServiceHost service = new ServiceHost(typeof(CalculatorService), new Uri("http://localhost:8000/udpsample/"));
            ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
            metadataBehavior.HttpGetEnabled = true;
            service.Description.Behaviors.Add(metadataBehavior);
            service.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            service.AddServiceEndpoint(typeof(ICalculatorContract), calculatorBinding, calculatorAddress);
            service.AddServiceEndpoint(typeof(IDatagramContract), datagramBinding, datagramAddress);
            service.Open();

            Console.WriteLine("Service is started from code...");
            Console.WriteLine("Press <ENTER> to terminate the service and start service from config...");
            Console.ReadLine();

            service.Close();
        }

        static void ServiceFromConfig()
        {
            Console.Out.WriteLine("Testing Udp From Config.");

            ServiceHost service = new ServiceHost(typeof(ConfigurableCalculatorService));
            service.Open();

            Console.WriteLine("Service is started from config...");
            Console.WriteLine("Press <ENTER> to terminate the service and exit...");
            Console.ReadLine();

            service.Close();
        }

        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            ServiceFromCode();
            ServiceFromConfig();
        }
    }
}

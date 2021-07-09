
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    #region Contracts
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
        void Hello(string greeting);
    }

    [ServiceContract]
    public interface IStatusContract
    {
        [OperationContract]
        void Start();

        [OperationContract]
        string GetStatus();
    }
    #endregion

    class UdpTestConsole
    {
        /// <summary>
        /// Example of using UDP where bindings and addresses are specified in config.
        /// </summary>
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Console.WriteLine("Testing Udp Activation.");

            // 1. Start status service
            Console.WriteLine("Start the status service.");
            IStatusContract status = ChannelFactoryHelper.CreateSingleChannel<IStatusContract>("status");
            status.Start();

            // 2. Sending datagrams
            Console.WriteLine("Sending UDP datagrams.");
            Console.Write("Type a word that you want to say to the server: ");
            string input = Console.ReadLine();
            IDatagramContract datagram = ChannelFactoryHelper.CreateSingleChannel<IDatagramContract>("datagram");
            for (int i = 0; i < 5; i++)
            {
                string greeting = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", input, i);
                Console.WriteLine("    Sending datagram: {0}", greeting);
                datagram.Hello(greeting);
            }
            ((IChannel)datagram).Close();

            // 3. Calling UDP duplex contract
            Console.WriteLine("Calling UDP duplex contract (ICalculatorContract).");
            ICalculatorContract calculator = ChannelFactoryHelper.CreateSingleChannel<ICalculatorContract>("calculator");
            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine("    {0} + {1} = {2}", i, i * 2, calculator.Add(i, i * 2));
            }
            ((IChannel)calculator).Close();

            // 4. Check status service
            Console.WriteLine("Getting status and dump server traces:");
            string statusResult = status.GetStatus();
            string[] traces = statusResult.Split('|');
            for (int i = 0; i < traces.Length; i++)
            {
                Console.WriteLine("    {0}", traces[i]);
            }
            ((IChannel)status).Close();

            Console.WriteLine("Press <ENTER> to complete test.");
            Console.ReadLine();
        }
    }

    public static class ChannelFactoryHelper
    {
        static void BindLifetimes(IChannelFactory factory, IChannel channel)
        {
            channel.Closed += delegate
            {
                IAsyncResult result = factory.BeginClose(FactoryCloseCallback, factory);
                if (result.CompletedSynchronously)
                    factory.EndClose(result);
            };
        }

        static void FactoryCloseCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;
            IChannelFactory factory = (IChannelFactory)result.AsyncState;
            factory.EndClose(result);
        }

        public static T CreateSingleChannel<T>(string endpointConfigurationName)
        {
            return CreateSingleChannel<T>(new ChannelFactory<T>(endpointConfigurationName));
        }

        public static T CreateSingleChannel<T>(Binding binding, EndpointAddress address)
        {
            return CreateSingleChannel<T>(new ChannelFactory<T>(binding, address));
        }

        static T CreateSingleChannel<T>(ChannelFactory<T> factory)
        {
            T channel = factory.CreateChannel();
            BindLifetimes(factory, (IChannel)channel);
            return channel;
        }
    }
}


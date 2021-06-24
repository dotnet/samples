
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using stockservice.contoso.com.wse.samples._2003._06;

namespace Microsoft.Samples.WseTcpTransport
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
        void Hello();
    }
    #endregion

    #region Services
    class ConfigurableCalculatorService : CalculatorService
    {
    }

    class CalculatorService : IDatagramContract, ICalculatorContract
    {
        #region ICalculatorContract implementation
        public int Add(int x, int y)
        {
            Console.WriteLine("   adding {0} + {1}", x, y);
            return (x + y);
        }
        #endregion

        #region IDatagramContract implementation
        public void Hello()
        {
            Console.Out.WriteLine("Hello, world!");
        }
        #endregion
    }
    #endregion

    #region Proxies
    public class CalculatorProxy : ClientBase<ICalculatorContract>, ICalculatorContract
    {
        public CalculatorProxy()
            : base()
        {
        }

        public CalculatorProxy(string configurationName)
            : base(configurationName)
        {
        }

        public CalculatorProxy(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
        }

        public int Add(int x, int y)
        {
            return base.Channel.Add(x, y);
        }
    }

    public class DatagramProxy : ClientBase<IDatagramContract>, IDatagramContract
    {
        public DatagramProxy()
            : base()
        {
        }

        public DatagramProxy(string configurationName)
            : base(configurationName)
        {
        }

        public DatagramProxy(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
        }

        public void Hello()
        {
            base.Channel.Hello();
        }
    }
    #endregion

    class TestConsole
    {
        IChannelFactory<IDuplexSessionChannel> channelFactory;
        IChannelListener<IDuplexSessionChannel> listener;
        Uri uri;
        
        TestConsole()
        {
            CustomBinding binding = new CustomBinding();
            MtomMessageEncodingBindingElement mtomBindingElement = new MtomMessageEncodingBindingElement();
            mtomBindingElement.MessageVersion = MessageVersion.Soap11WSAddressingAugust2004;
            binding.Elements.Add(mtomBindingElement);
            binding.Elements.Add(new WseTcpTransportBindingElement());
            
            // WSE sample uses a logical endpoint. So we need to set the physical via separately.
            EndpointAddress address = new EndpointAddress("soap://stockservice.contoso.com/wse/samples/2003/06/TcpSyncStockService");
            
            // INSERT your HOSTNAME here:
            string hostname = "localhost";
            Uri via = new Uri(string.Format("soap.tcp://{0}/StockService", hostname));
            StockServicePortTypeClient client = new StockServicePortTypeClient(binding, address);
            client.Endpoint.Behaviors.Add(new ClientViaBehavior(via));
            Console.WriteLine("Calling {0}", client.Endpoint.Address.Uri.AbsoluteUri);

            StockQuoteRequest quoteRequest = new StockQuoteRequest();
            quoteRequest.symbols = new ArrayOfString();
            quoteRequest.symbols.Add("FABRIKAM");
            quoteRequest.symbols.Add("CONTOSO");
            StockQuotes stocks = client.GetStockQuotes(quoteRequest);
            foreach (StockQuote quote in stocks)
            {
                Console.WriteLine("");
                Console.WriteLine("Symbol: " + quote.Symbol);
                Console.WriteLine("\tName: " + quote.Name);
                Console.WriteLine("\tLast Price: " + quote.Last);
            }
            Console.WriteLine("Press enter.");
            Console.ReadLine();

            binding = new CustomBinding(new WseTcpTransportBindingElement());
            this.uri = new Uri("wse.tcp://localhost:9000/a/b/");
            this.channelFactory = binding.BuildChannelFactory<IDuplexSessionChannel>();
            this.listener = binding.BuildChannelListener<IDuplexSessionChannel>(this.uri);
        }

        void ServerThread(object state)
        {
            TestConsole thisPtr = (TestConsole)state;
            thisPtr.RunServer();
        }

        void RunServer()
        {
            IDuplexSessionChannel channel = listener.AcceptChannel(TimeSpan.MaxValue);
            channel.Open();
            Message message;
            while ((message = channel.Receive(TimeSpan.MaxValue)) != null)
            {
                string action = message.Headers.Action;
                string body = message.GetBody<string>();
                Console.WriteLine("Received Action: " + action);
                Console.WriteLine("Received Body: " + body);

                if (action == "http://SayHello")
                {
                    Message helloMessage = Message.CreateMessage(message.Version, "http://Hello", "Hello " + body);
                    channel.Send(helloMessage);
                }
                message.Close();
            }
            channel.Close();
        }

        void RunTest()
        {
            listener.Open();
            Thread thread = new Thread(ServerThread);
            thread.Start(this);

            CustomBinding binding = new CustomBinding(new WseTcpTransportBindingElement());
            IChannelFactory<IDuplexSessionChannel> channelFactory = binding.BuildChannelFactory<IDuplexSessionChannel>();
            
            channelFactory.Open();
            IDuplexSessionChannel channel = channelFactory.CreateChannel(new EndpointAddress(this.uri));
            Message requestMessage;
            channel.Open();
            requestMessage = Message.CreateMessage(binding.MessageVersion, "http://SayHello", "to you.");
            channel.Send(requestMessage);
            Message hello = channel.Receive();
            using (hello)
            {
                Console.WriteLine(hello.GetBody<string>());
            }
            Console.WriteLine("Press enter.");
            Console.ReadLine();
            requestMessage = Message.CreateMessage(binding.MessageVersion, "http://NotHello", "to me.");
            channel.Send(requestMessage);
            channel.Close();
            thread.Join();

            channelFactory.Close();
            listener.Close();
            Console.WriteLine("Press enter.");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            TestConsole testConsole = new TestConsole();
            testConsole.RunTest();
        }
    }
}


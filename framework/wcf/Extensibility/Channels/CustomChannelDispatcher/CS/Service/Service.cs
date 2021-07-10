//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Xml;

namespace Microsoft.Samples.CustomChannelDispatcher
{
    public class CustomServiceHostBase : ServiceHostBase
    {
        MyServiceManager serviceManager;
        Uri interestedUri;
        public CustomServiceHostBase(params Uri[] baseAddresses)
        {
            for (int i = 0; i < baseAddresses.Length; i++)
            {
                if (object.ReferenceEquals(baseAddresses[i].Scheme, Uri.UriSchemeHttp))
                {
                    interestedUri = baseAddresses[i];
                    break;
                }
            }

            if (interestedUri == null)
            {
                throw new InvalidOperationException("You need to enable HTTP protocol for this application.");
            }

            serviceManager = new MyServiceManager();
            InitializeDescription(new UriSchemeKeyedCollection(baseAddresses));
        }

        protected override void InitializeRuntime()
        {
            BindingParameterCollection bpc = new BindingParameterCollection();
            VirtualPathExtension virtualPathExtension = this.Extensions.Find<VirtualPathExtension>();
            if (virtualPathExtension != null)
            {
                bpc.Add(virtualPathExtension);
            }

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();

            IChannelListener<IReplyChannel> listener = basicHttpBinding.BuildChannelListener<IReplyChannel>(interestedUri, bpc);

            CustomChannelDispatcher channelDispatcher = new CustomChannelDispatcher(serviceManager, listener);
            this.ChannelDispatchers.Add(channelDispatcher);
        }

        protected override ServiceDescription CreateDescription(out IDictionary<string, ContractDescription> implementedContracts)
        {
            // Not implemented.
            implementedContracts = null;
            return null;
        }

        protected override void ApplyConfiguration()
        {
            // Not implemented.
        }
    }

    class CustomChannelDispatcher : ChannelDispatcherBase
    {
        ServiceHostBase host;
        IChannelListener<IReplyChannel> listener;
        IReplyChannel acceptedChannel;
        MyServiceManager serviceManager;

        AsyncCallback onReceive;
        WaitCallback acceptCallback;

        public CustomChannelDispatcher(MyServiceManager serviceManager, IChannelListener<IReplyChannel> listener)
        {
            this.serviceManager = serviceManager;
            this.listener = listener;

            this.onReceive = new AsyncCallback(this.OnReceive);
            this.acceptCallback = new WaitCallback(this.AcceptCallback);
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        protected override void OnAbort()
        {
        }

        public override ServiceHostBase Host
        {
            get { return this.host; }
        }

        public override IChannelListener Listener
        {
            get { return this.listener; }
        }

        protected override void Attach(ServiceHostBase host)
        {
            this.host = host;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.listener.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.listener.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.listener.EndOpen(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.listener.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.listener.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.listener.EndClose(result);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            ThreadPool.QueueUserWorkItem(this.acceptCallback, this.listener);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            this.acceptedChannel.Close();
        }

        void AcceptCallback(object state)
        {
            IChannelListener<IReplyChannel> listener = (IChannelListener<IReplyChannel>)state;
            this.acceptedChannel = listener.AcceptChannel(TimeSpan.MaxValue);
            if (this.acceptedChannel != null)
            {
                this.acceptedChannel.Open();
                ReceiveRequest(this.acceptedChannel);
            }
            else
            {
                listener.Close();
            }
        }

        void ReceiveRequest(IReplyChannel channel)
        {
            for (; ; )
            {
                IAsyncResult result = channel.BeginReceiveRequest(TimeSpan.MaxValue, this.onReceive, channel);
                if (!result.CompletedSynchronously)
                    break;

                RequestContext context = channel.EndReceiveRequest(result);
                if (!HandleRequest(channel, context))
                {
                    break;
                }
            }
        }

        void OnReceive(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            IReplyChannel channel = (IReplyChannel)result.AsyncState;
            RequestContext context = channel.EndReceiveRequest(result);

            if (HandleRequest(channel, context))
            {
                ReceiveRequest(channel);
            }
        }

        bool HandleRequest(IReplyChannel channel, RequestContext context)
        {
            if (context == null)
            {
                channel.Close();
                return false;
            }

            try
            {
                serviceManager.HandleRequest(context);
            }
            catch (CommunicationException)
            {
                context.Abort();
            }
            finally
            {
                context.Close();
            }
            return true;
        }
    }

    class ResponseBodyWriter : BodyWriter
    {
        string name;
        string ns;
        string resultName;
        string result;

        public ResponseBodyWriter(string name, string ns)
            : base(true)
        {
            this.name = string.Concat(name, "Response");
            this.ns = ns;
            this.resultName = string.Concat(name, "Result");
        }

        public string Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(this.name, this.ns);
            writer.WriteStartElement(this.resultName);
            writer.WriteString(this.result);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }

    class MyServiceManager
    {
        string requestAction;
        string responseAction;
        HelloWorldService serviceInstance;

        public MyServiceManager()
        {
            serviceInstance = new HelloWorldService();

            requestAction = string.Format(CultureInfo.InvariantCulture, "{0}{1}/{2}", HelloWorldService.Namespace,
                HelloWorldService.ServiceName,
                HelloWorldService.OperationName);

            responseAction = string.Concat("{0}{1}/{2}Response", HelloWorldService.Namespace,
                HelloWorldService.ServiceName,
                HelloWorldService.OperationName);
        }

        public void HandleRequest(RequestContext context)
        {
            Debug.Assert(context != null);

            if (context.RequestMessage.Headers.Action == requestAction)
            {
                XmlDictionaryReader reader = context.RequestMessage.GetReaderAtBodyContents();
                DataContractSerializer serializer = new DataContractSerializer(typeof(string));
                reader.ReadStartElement(); // read wrapper begin
                string value = (string)serializer.ReadObject(reader, false);
                reader.ReadEndElement(); // read wrapper end

                reader.Close();

                string result = serviceInstance.Hello(value);
                ResponseBodyWriter responseBodyWriter = new ResponseBodyWriter(
                    HelloWorldService.OperationName,
                    HelloWorldService.Namespace);
                responseBodyWriter.Result = result;

                MessageVersion requestVersion = context.RequestMessage.Version;
                UniqueId requestId = context.RequestMessage.Headers.MessageId;

                Message reply = Message.CreateMessage(requestVersion,
                                                      responseAction,
                                                      responseBodyWriter);

                PrepareReply(reply, requestId);
                if (context.RequestMessage.Headers.ReplyTo != null)
                {
                    reply.Headers.To = context.RequestMessage.Headers.ReplyTo.Uri;
                }

                context.Reply(reply);
            }
            else
            {                
                object property = null;
                if (context.RequestMessage.Properties.TryGetValue(HttpRequestMessageProperty.Name,
                    out property))
                {
                    if (string.Compare(((HttpRequestMessageProperty)property).Method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        HandleHttpGet(context);
                        return;
                    }
                }

                SendMismatchFault(context);
            }
        }

        void HandleHttpGet(RequestContext context)
        {
            Message message = new HtmlMessage(
                context.RequestMessage.Version,
                context.RequestMessage.Headers.ReplyTo.ToString(),
                HelloWorldService.ServiceName,
                HelloWorldService.OperationName);

            HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty();
            responseProperty.StatusCode = HttpStatusCode.OK;
            HttpRequestMessageProperty requestProperty = (HttpRequestMessageProperty)context.RequestMessage.Properties[HttpRequestMessageProperty.Name];
            responseProperty.Headers.Add(HttpResponseHeader.ContentType, requestProperty.Headers[HttpRequestHeader.ContentType]);
            
            message.Properties.Add(HttpResponseMessageProperty.Name, responseProperty);

            context.Reply(message);
        }

        void SendMismatchFault(RequestContext context)
        {
            Message message = Message.CreateMessage(context.RequestMessage.Version, MessageFault.CreateFault(new FaultCode("NotSupported"), "The request is not supported"), responseAction);
            context.Reply(message);
        }

        static void PrepareReply(Message reply, UniqueId messageId)
        {
            MessageHeaders replyHeaders = reply.Headers;

            if (object.ReferenceEquals(replyHeaders.RelatesTo, null))
            {
                replyHeaders.RelatesTo = messageId;
            }
        }
    }

    class HtmlMessage : Message
    {
        MessageVersion version;
        MessageHeaders headers;
        MessageProperties properties;
        string serviceUrl;
        string serviceName;
        string proxyName;
        public HtmlMessage(MessageVersion version, string serviceUrl, string serviceName, string proxyName)
        {
            this.version = version;
            this.serviceUrl = serviceUrl;
            this.serviceName = serviceName;
            this.proxyName = proxyName;
            this.headers = new MessageHeaders(version);
        }

        public override MessageHeaders Headers
        {
            get
            {
                return this.headers;
            }
        }

        protected override void OnWriteMessage(XmlDictionaryWriter writer)
        {
            // only write the body contents (no <Headers/>, and no <Envelope> or <Body> tags)
            OnWriteBodyContents(writer);
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            OnWriteBodyContents(writer);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            string title = string.Format(CultureInfo.InvariantCulture, "{0} Service", this.serviceName);
            writer.WriteStartElement("HTML");
            writer.WriteStartElement("HEAD");
            writer.WriteElementString("TITLE", title);
            writer.WriteEndElement(); // HEAD

            writer.WriteStartElement("BODY");
            writer.WriteStartElement("H1");
            writer.WriteString(title);
            writer.WriteEndElement(); // H1

            writer.WriteStartElement("B");
            writer.WriteString(string.Format(CultureInfo.InvariantCulture, "The service '{0}' at address:", this.serviceName));
            writer.WriteStartElement("UL");
            writer.WriteStartElement("A");
            writer.WriteAttributeString("href", this.serviceUrl);
            writer.WriteString(this.serviceUrl);
            writer.WriteEndElement(); // A
            writer.WriteEndElement(); // UL
            writer.WriteString(" has been compiled successfully.");
            writer.WriteEndElement(); // B
            writer.WriteEndElement(); // HTML
            writer.WriteEndElement(); // BODY
        }

        public override MessageProperties Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new MessageProperties();
                }

                return this.properties;
            }
        }

        public override MessageVersion Version
        {
            get
            {
                return this.version;
            }
        }
    }

    class CustomServiceHostFactory : ServiceHostFactoryBase
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            return new CustomServiceHostBase(baseAddresses);
        }
    }

    class HelloWorldService
    {
        public const string Namespace = "http://tempuri.org/";
        public const string ServiceName = "HelloWorld";
        public const string OperationName = "Hello";

        int count = 0;

        public string Hello(string greeting)
        {
            int localCopy = Interlocked.Increment(ref count);
            return string.Format(CultureInfo.InvariantCulture, "You said: {0}. Message id: {1}", greeting, localCopy);
        }
    }
}

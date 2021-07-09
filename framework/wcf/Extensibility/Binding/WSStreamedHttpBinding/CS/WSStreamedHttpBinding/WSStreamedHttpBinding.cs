//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Security;    
using System.Xml;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    public class WSStreamedHttpBinding : Binding
    {
        StreamedSecurityMode securityMode;
        BindingElement transport;
        StreamedTransferMode transferMode;
        bool flowTransactions;

       // private BindingElements
        HttpTransportBindingElement httpTransport;
        HttpsTransportBindingElement httpsTransport;
        TextMessageEncodingBindingElement textEncoding;
        TransactionFlowBindingElement transactionFlow;

        public WSStreamedHttpBinding() { Initialize(); }
        public WSStreamedHttpBinding(string configurationName) : this() { ApplyConfiguration(configurationName); }

        public HostNameComparisonMode HostNameComparisonMode
        {
            get { return httpTransport.HostNameComparisonMode; }
            set
            {
                httpTransport.HostNameComparisonMode = value;
                httpsTransport.HostNameComparisonMode = value;
            }
        }

        public long MaxReceivedMessageSize
        {
            get { return httpTransport.MaxReceivedMessageSize; }
            set
            {
                httpTransport.MaxReceivedMessageSize = value;
                httpsTransport.MaxReceivedMessageSize = value;
            }
        }

        public int MaxBufferSize
        {
            get { return httpTransport.MaxBufferSize; }
            set
            {
                httpTransport.MaxBufferSize = value;
                httpsTransport.MaxBufferSize = value;
            }
        }

        public Uri ProxyAddress
        {
            get { return httpTransport.ProxyAddress; }
            set
            {
                httpTransport.ProxyAddress = value;
                httpsTransport.ProxyAddress = value;
            }
        }

        public bool BypassProxyOnLocal
        {
            get { return httpTransport.BypassProxyOnLocal; }
            set
            {
                httpTransport.BypassProxyOnLocal = value;
                httpsTransport.BypassProxyOnLocal = value;
            }
        }

        public bool UseDefaultWebProxy
        {
            get { return httpTransport.UseDefaultWebProxy; }
            set
            {
                httpTransport.UseDefaultWebProxy = value;
                httpsTransport.UseDefaultWebProxy = value;
            }
        }

        public StreamedTransferMode TransferMode
        {
            get { return this.transferMode; }
            set
            {
                this.transferMode = value;
                httpTransport.TransferMode = (System.ServiceModel.TransferMode)value;
                httpsTransport.TransferMode = (System.ServiceModel.TransferMode)value;
            }
        }

        public override string Scheme
        {
            get
            {
                if (securityMode == StreamedSecurityMode.None)
                    return "http";
                else
                    return "https";
            }
        }

        public EnvelopeVersion SoapVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        public System.Text.Encoding TextEncoding
        {
            get { return textEncoding.WriteEncoding; }
            set { textEncoding.WriteEncoding = value; }
        }

        public bool FlowTransactions
        {
            get { return this.flowTransactions; }
            set { this.flowTransactions = value; }
        }

        public StreamedSecurityMode SecurityMode
        {
            get { return securityMode; }
            set
            {
                securityMode = value;

                // use either http or https transport depending on security requirements
                if (securityMode == StreamedSecurityMode.None)
                {
                    transport = httpTransport;
                }
                else
                {
                    transport = httpsTransport;
                    httpsTransport.RequireClientCertificate = false;
                    httpsTransport.AuthenticationScheme = AuthenticationSchemes.Anonymous;
                    httpsTransport.ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
                    httpsTransport.Realm = "";
                }
            }
        }

        void Initialize()
        {
            httpTransport = new HttpTransportBindingElement();
            httpsTransport = new HttpsTransportBindingElement();
            transactionFlow = new TransactionFlowBindingElement();
            textEncoding = new TextMessageEncodingBindingElement();

            // setting up configurable settings' default values
            this.MaxReceivedMessageSize = long.MaxValue;
            this.SecurityMode = StreamedSecurityMode.Transport;
            this.TransferMode = StreamedTransferMode.Streamed;

            // setting up non-configurable settings' default values
            this.FlowTransactions = true;
            transactionFlow.TransactionProtocol = TransactionProtocol.WSAtomicTransactionOctober2004;
        }

        void ApplyConfiguration(string configurationName)
        {
            WSStreamedHttpBindingCollectionElement section = WSStreamedHttpBindingCollectionElement.GetCollectionElement();
            WSStreamedHttpBindingConfigurationElement element = section.Bindings[configurationName];
            if (element == null)
            {
                throw new ConfigurationErrorsException();
            }
            else
            {
                element.ApplyConfiguration(this);
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            // return collection of BindingElements
            BindingElementCollection bindingElements = new BindingElementCollection();

            // the order of binding elements within the collection is important: layered channels are applied in the order included, followed by
            // the message encoder, and finally the transport at the end
            if (flowTransactions)
            {
                bindingElements.Add(transactionFlow);
            }
            bindingElements.Add(textEncoding);

            // add transport (http or https)
            bindingElements.Add(transport);
            
            return bindingElements.Clone();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingParameterCollection parameters)
        {
            // validate binding configurations
            if (httpTransport.MaxReceivedMessageSize < httpTransport.MaxBufferSize)
            {
                throw new ConfigurationErrorsException(WSStreamedHttpBindingConstants.InvalidMaxMessageSize);
            }

            return base.BuildChannelFactory<TChannel>(parameters);
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingParameterCollection parameters)
        {
            // validate binding configurations
            if (httpTransport.MaxReceivedMessageSize < httpTransport.MaxBufferSize)
            {
                throw new ConfigurationErrorsException(WSStreamedHttpBindingConstants.InvalidMaxMessageSize);
            }

            return base.BuildChannelListener<TChannel>(parameters);
        }
    }
}

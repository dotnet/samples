//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.Udp
{

    /// <summary>
    /// Binding for Udp. This is our "sample profile" for Udp, which uses Text+Soap 1.2 
    /// and allows for variation in Reliability capabilities. If ReliableSessionEnabled is set
    /// then we will layer RM+CompositeDuplex on top of Udp. Otherwise we will just
    /// have Udp on our stack.  
    /// </summary>
    public class SampleProfileUdpBinding : Binding
    {
        bool reliableSessionEnabled;

        // private BindingElements
        CompositeDuplexBindingElement compositeDuplex;
        ReliableSessionBindingElement session;
        UdpTransportBindingElement transport;
        MessageEncodingBindingElement encoding;

        public SampleProfileUdpBinding()
        {
            Initialize();
        }

        public SampleProfileUdpBinding(bool reliableSessionEnabled) : this()
        {
            this.ReliableSessionEnabled = reliableSessionEnabled;
        }

        public SampleProfileUdpBinding(string configurationName) : this()
        {
            ApplyConfiguration(configurationName);
        }

        public bool OrderedSession
        {
            get { return session.Ordered; }
            set { session.Ordered = value; }
        }

        public bool ReliableSessionEnabled
        {
            get { return reliableSessionEnabled; }
            set { reliableSessionEnabled = value; }
        }

        public override string Scheme { get { return "soap.udp"; } }

        public TimeSpan SessionInactivityTimeout
        {
            get { return this.session.InactivityTimeout; }
            set { this.session.InactivityTimeout = value; }
        }

        public EnvelopeVersion SoapVersion
        {
            get { return EnvelopeVersion.Soap12; }
        }

        public Uri ClientBaseAddress
        {
            get { return this.compositeDuplex.ClientBaseAddress; }
            set { this.compositeDuplex.ClientBaseAddress = value; }
        }

        /// <summary>
        /// Create the set of binding elements that make up this binding. 
        /// NOTE: order of binding elements is important.
        /// </summary>
        /// <returns></returns>
        public override BindingElementCollection CreateBindingElements()
        {   
            BindingElementCollection bindingElements = new BindingElementCollection();

            if (ReliableSessionEnabled)
            {
                bindingElements.Add(session);
                bindingElements.Add(compositeDuplex);
            }

            bindingElements.Add(encoding);
            bindingElements.Add(transport);

            return bindingElements.Clone();
        }

        void ApplyConfiguration(string configurationName)
        {
            SampleProfileUdpBindingCollectionElement section = (SampleProfileUdpBindingCollectionElement)ConfigurationManager.GetSection(UdpConstants.UdpBindingSectionName);
            SampleProfileUdpBindingConfigurationElement element = section.Bindings[configurationName];
            if (element == null)
            {
                throw new ConfigurationErrorsException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "There is no binding named {0} at {1}.", configurationName, section.BindingName));
            }
            else
            {
                element.ApplyConfiguration(this);
            }
        }

        void Initialize()
        {
            transport = new UdpTransportBindingElement();
            session = new ReliableSessionBindingElement();
            compositeDuplex = new CompositeDuplexBindingElement();
            encoding = new TextMessageEncodingBindingElement();
        }

        //initialize a SampleProfileUdpBinding from the info collected in a ReliableSessionBindingElement if one is present.
        void InitializeFrom(UdpTransportBindingElement udpTransportBindingElement,
                    TextMessageEncodingBindingElement textMessageEncodingBindingElement,
                    ReliableSessionBindingElement reliableSessionBindingElement,
                    CompositeDuplexBindingElement compositeDuplexBindingElement)
        {
            this.transport.Multicast = udpTransportBindingElement.Multicast;
            this.transport.MaxBufferPoolSize = udpTransportBindingElement.MaxBufferPoolSize;
            this.transport.MaxReceivedMessageSize = udpTransportBindingElement.MaxReceivedMessageSize;

            ((TextMessageEncodingBindingElement)this.encoding).WriteEncoding = textMessageEncodingBindingElement.WriteEncoding;
            textMessageEncodingBindingElement.ReaderQuotas.CopyTo(((TextMessageEncodingBindingElement)this.encoding).ReaderQuotas);

            this.ReliableSessionEnabled = reliableSessionBindingElement != null;

            if (reliableSessionBindingElement != null)
            {
                this.SessionInactivityTimeout = reliableSessionBindingElement.InactivityTimeout;
                this.OrderedSession = reliableSessionBindingElement.Ordered;
            }

            if (compositeDuplexBindingElement != null)
            {
                this.ClientBaseAddress = compositeDuplexBindingElement.ClientBaseAddress;
            }
        }

        //try to create a SampleProfileUdpBinding from the collection of BindingElement
        //returns true if it is possible, with the resulting binding.
        public static bool TryCreate(BindingElementCollection elements, out Binding binding)
        {
            binding = null;
            if (elements.Count > 4)
            {
                return false;
            }

            ReliableSessionBindingElement reliableSessionBindingElement = null;
            CompositeDuplexBindingElement compositeDuplexBindingElement = null;
            TextMessageEncodingBindingElement textMessageEncodingBindingElement = null;
            UdpTransportBindingElement udpTransportBindingElement = null;

            foreach (BindingElement element in elements)
            {
                if (element is CompositeDuplexBindingElement)
                {
                    compositeDuplexBindingElement = element as CompositeDuplexBindingElement;
                }
                else if (element is TransportBindingElement)
                {
                    udpTransportBindingElement = element as UdpTransportBindingElement;
                }
                else if (element is TextMessageEncodingBindingElement)
                {
                    textMessageEncodingBindingElement = element as TextMessageEncodingBindingElement;
                }
                else if (element is ReliableSessionBindingElement)
                {
                    reliableSessionBindingElement = element as ReliableSessionBindingElement;
                }
                else
                {
                    return false;
                }
            }

            if (udpTransportBindingElement == null)
            {
                return false;
            }
            if (textMessageEncodingBindingElement == null)
            {
                return false;
            }

            if (((reliableSessionBindingElement != null) && (compositeDuplexBindingElement == null))
                || ((reliableSessionBindingElement == null) && (compositeDuplexBindingElement != null)))
            {
                return false;
            }

            SampleProfileUdpBinding sampleProfileUdpBinding = new SampleProfileUdpBinding();
            sampleProfileUdpBinding.InitializeFrom(udpTransportBindingElement, textMessageEncodingBindingElement,
                                            reliableSessionBindingElement, compositeDuplexBindingElement);
            if (!sampleProfileUdpBinding.IsBindingElementsMatch(udpTransportBindingElement, textMessageEncodingBindingElement,
                                                         reliableSessionBindingElement, compositeDuplexBindingElement))
            {
                return false;
            }

            binding = sampleProfileUdpBinding;
            return true;
        }

        bool IsBindingElementsMatch(UdpTransportBindingElement udpTransportBindingElement,
                    TextMessageEncodingBindingElement textMessageEncodingBindingElement,
                    ReliableSessionBindingElement reliableSessionBindingElement,
                    CompositeDuplexBindingElement compositeDuplexBindingElement)
        {
            if (!IsTransportMatch(this.transport, udpTransportBindingElement))
            {
                return false;
            }

            if(!IsEncodingMatch(this.encoding, textMessageEncodingBindingElement))
            {
                return false;
            }

            if (this.ReliableSessionEnabled)
            {
                if (!IsSessionMatch(this.session, reliableSessionBindingElement))
                {
                    return false;
                }
                if (compositeDuplexBindingElement != null)
                {
                    if (!IsCompositeDuplexMatch(this.compositeDuplex, compositeDuplexBindingElement))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (reliableSessionBindingElement != null)
            {
                return false;
            }

            return true;
        }

        bool IsTransportMatch(BindingElement a, BindingElement b)
        {
            if (b == null)
            {
                return false;
            }

            UdpTransportBindingElement transportA = a as UdpTransportBindingElement;
            UdpTransportBindingElement transportB = b as UdpTransportBindingElement;

            if (transportB == null)
            {
                return false;
            }
            if (transportA.MaxBufferPoolSize != transportB.MaxBufferPoolSize)
            {
                return false;
            }
            if (transportA.MaxReceivedMessageSize != transportB.MaxReceivedMessageSize)
            {
                return false;
            }
            if (transportA.Multicast != transportB.Multicast)
            {
                return false;
            }

            return true;
        }

        bool IsEncodingMatch(BindingElement a, BindingElement b)
        {
            if (b == null)
            {
                return false;
            }

            MessageEncodingBindingElement messageEncodingBindingElement = b as MessageEncodingBindingElement;
            if (messageEncodingBindingElement == null)
            {
                return false;
            }

            TextMessageEncodingBindingElement textA = a as TextMessageEncodingBindingElement;
            TextMessageEncodingBindingElement textB = b as TextMessageEncodingBindingElement;
            if (textB == null)
            {
                return false;
            }
            if (textA.MaxReadPoolSize != textB.MaxReadPoolSize)
            {
                return false;
            }
            if (textA.MaxWritePoolSize != textB.MaxWritePoolSize)
            {
                return false;
            }

            // compare XmlDictionaryReaderQuotas
            if (textA.ReaderQuotas.MaxStringContentLength != textB.ReaderQuotas.MaxStringContentLength)
            {
                return false;
            }
            if (textA.ReaderQuotas.MaxArrayLength != textB.ReaderQuotas.MaxArrayLength)
            {
                return false;
            }
            if (textA.ReaderQuotas.MaxBytesPerRead != textB.ReaderQuotas.MaxBytesPerRead)
            {
                return false;
            }
            if (textA.ReaderQuotas.MaxDepth != textB.ReaderQuotas.MaxDepth)
            {
                return false;
            }
            if (textA.ReaderQuotas.MaxNameTableCharCount != textB.ReaderQuotas.MaxNameTableCharCount)
            {
                return false;
            }

            if (textA.WriteEncoding.EncodingName != textB.WriteEncoding.EncodingName)
            {
                return false;
            }
            if(!IsMessageVersionMatch(textA.MessageVersion, textB.MessageVersion))
            {
                return false;
            }

            return true;
        }

        bool IsMessageVersionMatch(MessageVersion a, MessageVersion b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("b");
            }
            if (a.Addressing == null)
            {
                throw new InvalidOperationException("MessageVersion.Addressing cannot be null");
            }

            if (a.Envelope != b.Envelope)
            {
                return false;
            }
            //if (a.Addressing.Namespace != b.Addressing.Namespace)
            //{
            //    return false;
            //}

            return true;
        }

        bool IsSessionMatch(BindingElement a, BindingElement b)
        {
            if (b == null)
            {
                return false;
            }

            ReliableSessionBindingElement sessionA = a as ReliableSessionBindingElement;
            ReliableSessionBindingElement sessionB = b as ReliableSessionBindingElement;

            if (sessionB == null)
            {
                return false;
            }
            if (sessionA.AcknowledgementInterval != sessionB.AcknowledgementInterval)
            {
                return false;
            }
            if (sessionA.FlowControlEnabled != sessionB.FlowControlEnabled)
            {
                return false;
            }
            if (sessionA.InactivityTimeout != sessionB.InactivityTimeout)
            {
                return false;
            }
            if (sessionA.MaxPendingChannels != sessionB.MaxPendingChannels)
            {
                return false;
            }
            if (sessionA.MaxRetryCount != sessionB.MaxRetryCount)
            {
                return false;
            }
            if (sessionA.MaxTransferWindowSize != sessionB.MaxTransferWindowSize)
            {
                return false;
            }
            if (sessionA.Ordered != sessionB.Ordered)
            {
                return false;
            }

            return true;
        }

        bool IsCompositeDuplexMatch(BindingElement a, BindingElement b)
        {
            if (b == null)
            {
                return false;
            }

            CompositeDuplexBindingElement duplexA = a as CompositeDuplexBindingElement;
            CompositeDuplexBindingElement duplexB = b as CompositeDuplexBindingElement;
            if (duplexB == null)
            {
                return false;
            }

            return (duplexB.ClientBaseAddress == duplexA.ClientBaseAddress);
        }
    }
}


//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
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

        public override string Scheme { get { return "net.udp"; } }

        public TimeSpan SessionInactivityTimeout
        {
            get { return this.session.InactivityTimeout; }
            set { this.session.InactivityTimeout = value; }
        }

        public EnvelopeVersion SoapVersion
        {
            get { return EnvelopeVersion.Soap12; }
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

    }
}


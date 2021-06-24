//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Configuration;
using System.Globalization;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.ServiceModel.Samples
{
    public class SampleProfileUdpBindingConfigurationElement : StandardBindingElement
    {
        public SampleProfileUdpBindingConfigurationElement(string configurationName)
            : base(configurationName)
        {
        }

        public SampleProfileUdpBindingConfigurationElement()
            : this(null)
        {
        }

        protected override Type BindingElementType
        {
            get { return typeof(SampleProfileUdpBinding); }
        }

        [ConfigurationProperty(UdpConfigurationStrings.OrderedSession, DefaultValue = UdpDefaults.OrderedSession)]
        public bool OrderedSession
        {
            get { return (bool)base[UdpConfigurationStrings.OrderedSession]; }
            set { base[UdpConfigurationStrings.OrderedSession] = value; }
        }

        [ConfigurationProperty(UdpConfigurationStrings.ReliableSessionEnabled, DefaultValue = UdpDefaults.ReliableSessionEnabled)]
        public bool ReliableSessionEnabled
        {
            get { return (bool)base[UdpConfigurationStrings.ReliableSessionEnabled]; }
            set { base[UdpConfigurationStrings.ReliableSessionEnabled] = value; }
        }

        [ConfigurationProperty(UdpConfigurationStrings.SessionInactivityTimeout, DefaultValue = UdpDefaults.SessionInactivityTimeoutString)]
        [TimeSpanValidator(MinValueString = "00:00:00")]
        public TimeSpan SessionInactivityTimeout
        {
            get { return (TimeSpan)base[UdpConfigurationStrings.SessionInactivityTimeout]; }
            set { base[UdpConfigurationStrings.SessionInactivityTimeout] = value; }
        }

        [ConfigurationProperty(UdpConfigurationStrings.ClientBaseAddress, DefaultValue = null)]
        public Uri ClientBaseAddress
        {
            get { return (Uri)base[UdpConfigurationStrings.ClientBaseAddress]; }
            set { base[UdpConfigurationStrings.ClientBaseAddress] = value; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection properties = base.Properties;
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.OrderedSession, 
                    typeof(Boolean), UdpDefaults.OrderedSession, null, null, ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.ReliableSessionEnabled, 
                    typeof(Boolean), UdpDefaults.ReliableSessionEnabled, null, null, ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.SessionInactivityTimeout, 
                    typeof(TimeSpan), TimeSpan.Parse(UdpDefaults.SessionInactivityTimeoutString), null, 
                    new TimeSpanValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("10675199.02:48:05.4775807"), false), 
                    ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.ClientBaseAddress,
                    typeof(System.Uri), null, null, null, ConfigurationPropertyOptions.None));

                return properties;
            }
        }        
        
        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            SampleProfileUdpBinding udpBinding = (SampleProfileUdpBinding)binding;

            this.OrderedSession = udpBinding.OrderedSession;
            this.ReliableSessionEnabled = udpBinding.ReliableSessionEnabled;
            this.SessionInactivityTimeout = udpBinding.SessionInactivityTimeout;
            if (udpBinding.ClientBaseAddress != null)
                this.ClientBaseAddress = udpBinding.ClientBaseAddress;
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            if (binding.GetType() != typeof(SampleProfileUdpBinding))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Invalid type for binding. Expected type: {0}. Type passed in: {1}.",
                    typeof(SampleProfileUdpBinding).AssemblyQualifiedName,
                    binding.GetType().AssemblyQualifiedName));
            }
            SampleProfileUdpBinding udpBinding = (SampleProfileUdpBinding)binding;

            udpBinding.OrderedSession = this.OrderedSession;
            udpBinding.ReliableSessionEnabled = this.ReliableSessionEnabled;
            udpBinding.SessionInactivityTimeout = this.SessionInactivityTimeout;
            if (this.ClientBaseAddress != null)
                udpBinding.ClientBaseAddress = ClientBaseAddress;
        }
    }
}

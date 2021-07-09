
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


namespace Microsoft.ServiceModel.Samples
{
    #region using
    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    #endregion

    /// <summary>
    /// Configuration section for Udp. 
    /// </summary>
    public class UdpTransportElement : BindingElementExtensionElement
    {
        public UdpTransportElement()
        {
        }

        #region Configuration_Properties
        [ConfigurationProperty(UdpConfigurationStrings.MaxBufferPoolSize, DefaultValue = UdpDefaults.MaxBufferPoolSize)]
        [LongValidator(MinValue = 0)]
        public long MaxBufferPoolSize
        {
            get { return (long)base[UdpConfigurationStrings.MaxBufferPoolSize]; }
            set { base[UdpConfigurationStrings.MaxBufferPoolSize] = value; }
        }

        [ConfigurationProperty(UdpConfigurationStrings.MaxReceivedMessageSize, DefaultValue = UdpDefaults.MaxReceivedMessageSize)]
        [IntegerValidator(MinValue = 1)]
        public int MaxReceivedMessageSize
        {
            get { return (int)base[UdpConfigurationStrings.MaxReceivedMessageSize]; }
            set { base[UdpConfigurationStrings.MaxReceivedMessageSize] = value; }
        }

        [ConfigurationProperty(UdpConfigurationStrings.Multicast, DefaultValue = UdpDefaults.Multicast)]
        public bool Multicast
        {
            get { return (bool)base[UdpConfigurationStrings.Multicast]; }
            set { base[UdpConfigurationStrings.Multicast] = value; }
        }
        #endregion

        public override Type BindingElementType
        {
            get { return typeof(UdpTransportBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
            UdpTransportBindingElement bindingElement = new UdpTransportBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        #region Configuration_Infrastructure_overrides
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);

            UdpTransportBindingElement udpBindingElement = (UdpTransportBindingElement)bindingElement;
            udpBindingElement.MaxBufferPoolSize = this.MaxBufferPoolSize;
            udpBindingElement.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            udpBindingElement.Multicast = this.Multicast;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);

            UdpTransportElement source = (UdpTransportElement)from;
            this.MaxBufferPoolSize = source.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = source.MaxReceivedMessageSize;
            this.Multicast = source.Multicast;
        }

        protected override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);

            UdpTransportBindingElement udpBindingElement = (UdpTransportBindingElement)bindingElement;
            this.MaxBufferPoolSize = udpBindingElement.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = (int)udpBindingElement.MaxReceivedMessageSize;
            this.Multicast = udpBindingElement.Multicast;
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection properties = base.Properties;
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.MaxBufferPoolSize,
                    typeof(long), UdpDefaults.MaxBufferPoolSize, null, new LongValidator(0, Int64.MaxValue), ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.MaxReceivedMessageSize,
                    typeof(int), UdpDefaults.MaxReceivedMessageSize, null, new IntegerValidator(1, Int32.MaxValue), ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(UdpConfigurationStrings.Multicast,
                    typeof(Boolean), UdpDefaults.Multicast, null, null, ConfigurationPropertyOptions.None));
                return properties;
            }
        }
        #endregion
    }
}


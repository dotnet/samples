//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Security;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    public class WSStreamedHttpBindingConfigurationElement : StandardBindingElement
    {
        public WSStreamedHttpBindingConfigurationElement(string configurationName)
            : base(configurationName)
        {
        }

        public WSStreamedHttpBindingConfigurationElement()
            : this(null)
        {
        }

        protected override Type BindingElementType
        {
            get { return typeof(WSStreamedHttpBinding); }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.HostNameComparisonMode, DefaultValue = HostNameComparisonMode.StrongWildcard)]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get { return (HostNameComparisonMode)base[WSStreamedHttpBindingConstants.HostNameComparisonMode]; }
            set { base[WSStreamedHttpBindingConstants.HostNameComparisonMode] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.MaxReceivedMessageSize, DefaultValue = long.MaxValue)]
        [LongValidator(MinValue = 1)]
        public long MaxReceivedMessageSize
        {
            get { return (long)base[WSStreamedHttpBindingConstants.MaxReceivedMessageSize]; }
            set { base[WSStreamedHttpBindingConstants.MaxReceivedMessageSize] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.MaxBufferSize, DefaultValue = 65536)]
        public int MaxBufferSize
        {
            get { return (int)base[WSStreamedHttpBindingConstants.MaxBufferSize]; }
            set { base[WSStreamedHttpBindingConstants.MaxBufferSize] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.ProxyAddress, DefaultValue = null)]
        public Uri ProxyAddress
        {
            get { return (Uri)base[WSStreamedHttpBindingConstants.ProxyAddress]; }
            set { base[WSStreamedHttpBindingConstants.ProxyAddress] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.BypassProxyOnLocal, DefaultValue = true)]
        public bool BypassProxyOnLocal
        {
            get { return (bool)base[WSStreamedHttpBindingConstants.BypassProxyOnLocal]; }
            set { base[WSStreamedHttpBindingConstants.BypassProxyOnLocal] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.UseDefaultWebProxy, DefaultValue = true)]
        public bool UseDefaultWebProxy
        {
            get { return (bool)base[WSStreamedHttpBindingConstants.UseDefaultWebProxy]; }
            set { base[WSStreamedHttpBindingConstants.UseDefaultWebProxy] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.TransferMode, DefaultValue = StreamedTransferMode.Streamed)]
        public StreamedTransferMode TransferMode
        {
            get { return (StreamedTransferMode)base[WSStreamedHttpBindingConstants.TransferMode]; }
            set { base[WSStreamedHttpBindingConstants.TransferMode] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.TextEncoding, DefaultValue = WSStreamedHttpBindingConstants.EncodingString)]
        [TypeConverter(typeof(TextEncodingConverter))]
        public Encoding TextEncoding
        {
            get { return (Encoding)base[WSStreamedHttpBindingConstants.TextEncoding]; }
            set { base[WSStreamedHttpBindingConstants.TextEncoding] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.FlowTransactions, DefaultValue = false)]
        public bool FlowTransactions
        {
            get { return (bool)base[WSStreamedHttpBindingConstants.FlowTransactions]; }
            set { base[WSStreamedHttpBindingConstants.FlowTransactions] = value; }
        }

        [ConfigurationProperty(WSStreamedHttpBindingConstants.SecurityMode, DefaultValue = StreamedSecurityMode.Transport)]
        public StreamedSecurityMode SecurityMode
        {
            get { return (StreamedSecurityMode)base[WSStreamedHttpBindingConstants.SecurityMode]; }
            set { base[WSStreamedHttpBindingConstants.SecurityMode] = value; }
        }

        protected override void InitializeFrom(Binding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }
            if (binding.GetType() != typeof(WSStreamedHttpBinding))
            {
                throw new ArgumentException();
            }
            WSStreamedHttpBinding wssBinding = (WSStreamedHttpBinding)binding;

            this.HostNameComparisonMode = wssBinding.HostNameComparisonMode;
            this.MaxReceivedMessageSize = wssBinding.MaxReceivedMessageSize;
            this.MaxBufferSize = wssBinding.MaxBufferSize;
            this.ProxyAddress = wssBinding.ProxyAddress;
            this.BypassProxyOnLocal = wssBinding.BypassProxyOnLocal;
            this.UseDefaultWebProxy = wssBinding.UseDefaultWebProxy;
            this.TransferMode = wssBinding.TransferMode;
            this.TextEncoding = wssBinding.TextEncoding;
            this.FlowTransactions = wssBinding.FlowTransactions;
            this.SecurityMode = wssBinding.SecurityMode;
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            if (null == binding)
            {
                throw new ArgumentNullException("binding");
            }

            if (binding.GetType() != typeof(WSStreamedHttpBinding))
            {
                throw new ArgumentException();
            }
            WSStreamedHttpBinding wssBinding = (WSStreamedHttpBinding)binding;

            wssBinding.HostNameComparisonMode = this.HostNameComparisonMode;
            wssBinding.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            wssBinding.MaxBufferSize = this.MaxBufferSize;
            wssBinding.ProxyAddress = this.ProxyAddress;
            wssBinding.BypassProxyOnLocal = this.BypassProxyOnLocal;
            wssBinding.UseDefaultWebProxy = this.UseDefaultWebProxy;
            wssBinding.TransferMode = this.TransferMode;
            wssBinding.TextEncoding = this.TextEncoding;
            wssBinding.FlowTransactions = this.FlowTransactions;
            wssBinding.SecurityMode = this.SecurityMode;
        }

        
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection properties = base.Properties;
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.HostNameComparisonMode, typeof(System.ServiceModel.HostNameComparisonMode), System.ServiceModel.HostNameComparisonMode.StrongWildcard, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.MaxReceivedMessageSize, typeof(System.Int64), (long)long.MaxValue, null, new System.Configuration.LongValidator(1, 9223372036854775807, false), System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.MaxBufferSize, typeof(System.Int32), 65536, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.ProxyAddress, typeof(System.Uri), null, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.BypassProxyOnLocal, typeof(System.Boolean), false, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.UseDefaultWebProxy, typeof(System.Boolean), true, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.TransferMode, typeof(StreamedTransferMode), StreamedTransferMode.Streamed, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.TextEncoding, typeof(System.Text.Encoding), "utf-8", new TextEncodingConverter(), null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.FlowTransactions, typeof(System.Boolean), false, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                properties.Add(new ConfigurationProperty(WSStreamedHttpBindingConstants.SecurityMode, typeof(StreamedSecurityMode), StreamedSecurityMode.Transport, null, null, System.Configuration.ConfigurationPropertyOptions.None));
                return properties;
            }
        }
    }

}

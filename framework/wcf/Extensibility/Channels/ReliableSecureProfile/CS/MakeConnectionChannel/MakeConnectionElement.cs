//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.ReliableSecureProfile
{
    sealed partial class MakeConnectionElement : BindingElementExtensionElement
    {
        public MakeConnectionElement()
        {
        }

        public override Type BindingElementType
        {
            get { return typeof(MakeConnectionBindingElement); }
        }

        [TypeConverter(typeof(TimeSpanConverter))]
        [ConfigurationProperty(MakeConnectionConstants.Configuration.ClientPollTimeout,
            DefaultValue = MakeConnectionConstants.Defaults.ClientPollTimeoutString)]
        public TimeSpan ClientPollTimeout
        {
            get { return (TimeSpan)base[MakeConnectionConstants.Configuration.ClientPollTimeout]; }
            set { base[MakeConnectionConstants.Configuration.ClientPollTimeout] = value; }
        }

        [TypeConverter(typeof(TimeSpanConverter))]
        [ConfigurationProperty(MakeConnectionConstants.Configuration.ServerPollTimeout,
            DefaultValue = MakeConnectionConstants.Defaults.ServerPollTimeoutString)]
        public TimeSpan ServerPollTimeout
        {
            get { return (TimeSpan)base[MakeConnectionConstants.Configuration.ServerPollTimeout]; }
            set { base[MakeConnectionConstants.Configuration.ServerPollTimeout] = value; }
        }

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            MakeConnectionBindingElement binding = (MakeConnectionBindingElement)bindingElement;

            this.ApplyConfiguration(binding);
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            MakeConnectionElement source = (MakeConnectionElement)from;

            this.CopyFrom(source);
        }

        protected override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            MakeConnectionBindingElement element = (MakeConnectionBindingElement)bindingElement;

            this.InitializeFrom(element);
        }

        protected override BindingElement CreateBindingElement()
        {
            MakeConnectionBindingElement binding = new MakeConnectionBindingElement();
            this.ApplyConfiguration(binding);
            return binding;
        }

        void ApplyConfiguration(MakeConnectionBindingElement bindingElement)
        {
            bindingElement.ServerPollTimeout = this.ServerPollTimeout;
            bindingElement.ClientPollTimeout = this.ClientPollTimeout;
        }

        void InitializeFrom(MakeConnectionBindingElement bindingElement)
        {
            this.ServerPollTimeout = bindingElement.ServerPollTimeout;
            this.ClientPollTimeout = bindingElement.ClientPollTimeout;
        }

        void CopyFrom(MakeConnectionElement source)
        {
            this.ServerPollTimeout = source.ServerPollTimeout;
            this.ClientPollTimeout = source.ClientPollTimeout;
        }
    }
}

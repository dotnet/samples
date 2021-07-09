//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.HttpCookieSession
{
    /// <summary>
    /// Binding element section HttpCookieSessionBinding. 
    /// Implements the configuration for 
    /// HttpCookieSessionBinding.
    /// </summary>
    public sealed class HttpCookieSessionBindingConfigurationElement
        : StandardBindingElement
    {       
        public HttpCookieSessionBindingConfigurationElement()
            : base()
        {            
        }

        public HttpCookieSessionBindingConfigurationElement(string configurationName)
            :base(configurationName)
        {            
        }

        protected override Type BindingElementType
        {
            get { return typeof(HttpCookieSessionBinding); }
        }

        [ConfigurationProperty(HttpCookieConfigurationStrings.SessionTimeoutProperty,
           DefaultValue = HttpCookieSessionDefaults.SessionTimeoutString)]
        public TimeSpan SessionTimeout
        {
            get
            {
                return (TimeSpan)base[HttpCookieConfigurationStrings.SessionTimeoutProperty];
            }
            set
            {
                base[HttpCookieConfigurationStrings.SessionTimeoutProperty] = value;
            }
        }

        [ConfigurationProperty(HttpCookieConfigurationStrings.ExchangeTerminateMessageProperty,
           DefaultValue = HttpCookieSessionDefaults.ExchangeTerminateMessage)]
        public bool ExchangeTerminateMessage
        {
            get
            {
                return (bool)base[HttpCookieConfigurationStrings.ExchangeTerminateMessageProperty];
            }
            set
            {
                base[HttpCookieConfigurationStrings.ExchangeTerminateMessageProperty] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                base.Properties.Add(
                        new ConfigurationProperty(
                            HttpCookieConfigurationStrings.SessionTimeoutProperty,
                            typeof(TimeSpan), 
                            HttpCookieSessionDefaults.SessionTimeout.ToString()));

                base.Properties.Add(
                        new ConfigurationProperty(
                            HttpCookieConfigurationStrings.ExchangeTerminateMessageProperty, 
                            typeof(bool),
                            HttpCookieSessionDefaults.ExchangeTerminateMessage.ToString().ToLower()));
                
                return base.Properties;
            }
        }

        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            
            HttpCookieSessionBinding sessionBinding = (HttpCookieSessionBinding)binding;

            this.SessionTimeout = sessionBinding.SessionTimeout;
            this.ExchangeTerminateMessage = 
                sessionBinding.ExchangeTerminateMessage;
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            HttpCookieSessionBinding sessionBinding = (HttpCookieSessionBinding)binding;

            sessionBinding.SessionTimeout = this.SessionTimeout;
            sessionBinding.ExchangeTerminateMessage =
                this.ExchangeTerminateMessage;
        }
    }
}

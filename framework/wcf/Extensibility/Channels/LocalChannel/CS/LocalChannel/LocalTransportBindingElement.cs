//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{
    
    public class LocalTransportBindingElement : TransportBindingElement
    {
        long maxReceivedMessageSize;
        public LocalTransportBindingElement()
        {
        }

        protected LocalTransportBindingElement(LocalTransportBindingElement other)
            : base(other)
        {
            this.maxReceivedMessageSize = LocalTransportDefaults.MaxReceivedMessageSize;
        }

        public override string Scheme
        {
            get { return LocalTransportDefaults.UriSchemeLocal; }
        }

        public override long MaxReceivedMessageSize
        {
            get { return this.maxReceivedMessageSize; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(ExceptionMessages.ValueMustBePositive);
                }
                this.maxReceivedMessageSize = value;
            }
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)(LocalTransportDefaults.MessageVersionLocal);
            }

            return base.GetProperty<T>(context);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!this.CanBuildChannelFactory<TChannel>(context))
            {
                throw new ArgumentException(String.Format(
                    ExceptionMessages.UnsupportedChannelType,typeof(TChannel).Name));
            }

            ValidateEncoderElement(context);

            return (IChannelFactory<TChannel>)(object)new LocalChannelFactory(context);
        }

        public override IChannelListener<TChannel>BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (string.Compare(context.ListenUriBaseAddress.Scheme, 
                    LocalTransportDefaults.UriSchemeLocal, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new ArgumentException(String.Format
                    (ExceptionMessages.InvalidUriScheme, context.ListenUriBaseAddress.Scheme, 
                        LocalTransportDefaults.UriSchemeLocal));
            }

            ValidateBaseAddress(context.ListenUriBaseAddress, "baseAddress");

            if (!this.CanBuildChannelListener<TChannel>(context))
            {
                throw new ArgumentException(String.Format(
                    ExceptionMessages.UnsupportedChannelType, typeof(TChannel).Name));
            }

            ValidateEncoderElement(context);

            return (IChannelListener<TChannel>)(object)new LocalChannelListener(context);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return (typeof(TChannel).TypeHandle.Equals(typeof(IDuplexSessionChannel).TypeHandle));
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return (typeof(TChannel).TypeHandle.Equals(typeof(IDuplexSessionChannel).TypeHandle));
        }

        public override BindingElement Clone()
        {
            return new LocalTransportBindingElement(this);
        }

        static void ValidateBaseAddress(Uri uri, string argumentName)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException(ExceptionMessages.BaseAddressMustBeAbsolute);
            }

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                throw new ArgumentException(ExceptionMessages.BaseAddressCannotHaveUserInfo);
            }

            if (!string.IsNullOrEmpty(uri.Query))
            {
                throw new ArgumentException(ExceptionMessages.BaseAddressCannotHaveQuery);
            }

            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                throw new ArgumentException(ExceptionMessages.BaseAddressCannotHaveFragment);
            }
        }

        void ValidateEncoderElement(BindingContext context)
        {
            Collection<MessageEncodingBindingElement> messageEncodingBindingElements
                = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

            if (messageEncodingBindingElements.Count > 0)
            {
                throw new InvalidOperationException(ExceptionMessages.MessageEncoderNotAllowed);
            }
        }
    }
}

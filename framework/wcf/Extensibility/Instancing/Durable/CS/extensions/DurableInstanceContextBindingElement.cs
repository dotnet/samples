using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel.Security;

namespace Microsoft.ServiceModel.Samples
{
    public class DurableInstanceContextBindingElement : BindingElement
    {
        string contextStoreLocation;
        ContextType contextType;


        public DurableInstanceContextBindingElement()
        {
            InitializeContextStoreLocation();
        }

        public DurableInstanceContextBindingElement(BindingElement other)
            : base(other)
        {
            DurableInstanceContextBindingElement otherBindingElement = other as DurableInstanceContextBindingElement;

            if (otherBindingElement != null)
            {
                this.contextStoreLocation = otherBindingElement.contextStoreLocation;
                this.contextType = otherBindingElement.contextType;

                if ((contextStoreLocation == null) ||
                    (contextStoreLocation.Trim().Length == 0))
                {
                    InitializeContextStoreLocation();
                }
                else
                {
                    // Throw if the specified contextStoreLocation is 
                    // invalid.
                    if (!Directory.Exists(contextStoreLocation))
                    {
                        throw new InvalidOperationException(
                            ResourceHelper.GetString(@"ExInvalidContextStorePath"));
                    }

                }
            }
            else
            {
                this.contextType = ContextType.MessageHeader;
                InitializeContextStoreLocation();
            }
        }

        public override bool CanBuildChannelFactory<TChannel>(
            BindingContext context)
        {
            ValidateTransport(context.Binding);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(
            BindingContext context)
        {
            ValidateTransport(context.Binding);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            ValidateTransport(context.Binding);
            IChannelFactory<TChannel> innerChannelFactory = context.BuildInnerChannelFactory<TChannel>();

            IChannelFactory<TChannel> channelFactory =
                new DurableInstanceChannelFactory<TChannel>(contextStoreLocation,
                    contextType, innerChannelFactory);

            return channelFactory;
        }

        public override IChannelListener<TChannel>
            BuildChannelListener<TChannel>(BindingContext context)
        {
            ValidateTransport(context.Binding);

            return new DurableInstanceChannelListener<TChannel>(contextType, contextStoreLocation, context);
        }

        public override BindingElement Clone()
        {
            return new DurableInstanceContextBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ContextType))
            {
                return (T)(object)this.ContextType;
            }

            return context.GetInnerProperty<T>();
        }

        public ContextType ContextType
        {
            get { return this.contextType; }
            set { this.contextType = value; }
        }

        public string ContextStoreLocation
        {
            get { return this.contextStoreLocation; }
            set { this.contextStoreLocation = value; }
        }

        //If the context type is set to HttpCookie this 
        //function verifies that the current binding
        //is using the HTTP transport.
        void ValidateTransport(CustomBinding binding)
        {
            if (this.contextType == ContextType.HttpCookie)
            {
                HttpTransportBindingElement httpTransport =
                    binding.Elements.Find<HttpTransportBindingElement>();

                if (httpTransport == null)
                {
                    throw new InvalidOperationException(
                        ResourceHelper.GetString("ExInvalidTransport"));
                }
            }
        }

        //Initializes contextStoreLocation.  
        //This method checks whether the directory that is used as the 
        //context store location exists and creates it if it is not
        //available.
        void InitializeContextStoreLocation()
        {
            // Get the path of the temp directory of the current user.
            string temp = Path.GetTempPath();

            // Build the required directory path in the Local Settings
            // directory.
            temp += DurableInstanceContextUtility.ContextStoreDirectory;

            // Create the directory if it does not exist.
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            this.contextStoreLocation = temp;
        }

    }
}


#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.IO;
using System.Xml;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    //This class implements binding element section to 
    //hook up this binding element to the configuration
    //system.
    public sealed class DurableInstanceContextBindingElementSection
        : BindingElementExtensionElement
    {
        string contextStoreLocation;
        ContextType contextType;
        ConfigurationPropertyCollection properties;

        public DurableInstanceContextBindingElementSection()
        {
        }

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);

            DurableInstanceContextBindingElement typedBindingElement =
                (DurableInstanceContextBindingElement)bindingElement;

            this.contextStoreLocation = typedBindingElement.ContextStoreLocation;
            this.contextType = typedBindingElement.ContextType;
        }

        public override Type BindingElementType
        {
            get { return typeof(DurableInstanceContextBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
            DurableInstanceContextBindingElement bindingElement =
                new DurableInstanceContextBindingElement();

            bindingElement.ContextStoreLocation =
                this.contextStoreLocation;

            bindingElement.ContextType = this.contextType;
            ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    properties = new ConfigurationPropertyCollection();

                    properties.Add(new ConfigurationProperty(
                        DurableInstanceContextUtility.ContextStoreLocation,
                        typeof(string),
                        this.contextStoreLocation));

                    properties.Add(new ConfigurationProperty(
                        DurableInstanceContextUtility.ContextType,
                        typeof(ContextType),
                        this.contextType));
                }

                return properties;
            }
        }

        protected override void DeserializeElement(XmlReader reader,
            bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
            string attributeValue;

            attributeValue =
                reader.GetAttribute(DurableInstanceContextUtility.ContextStoreLocation);

            if (attributeValue != null)
            {
                this.contextStoreLocation = attributeValue;
                attributeValue = null;
            }

            attributeValue =
                reader.GetAttribute(DurableInstanceContextUtility.ContextType);

            if (attributeValue != null)
            {
                switch (attributeValue)
                {
                    case DurableInstanceContextUtility.HttpCookie:
                        this.contextType = ContextType.HttpCookie;
                        break;
                    case DurableInstanceContextUtility.MessageHeader:
                        this.contextType = ContextType.MessageHeader;
                        break;
                    default:
                        throw new InvalidOperationException(
                            ResourceHelper.GetString(@"ExInvalidContextType"));
                }
            }
        }

        public string ContextStoreLocation
        {
            get
            {
                return this.contextStoreLocation;
            }
            set
            {
                this.contextStoreLocation = value;
            }
        }

        public ContextType ContextType
        {
            get
            {
                return this.contextType;
            }
            set
            {
                this.contextType = value;
            }
        }
    }

}


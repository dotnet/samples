//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System;
using System.Activities.Tracking;
using System.Collections.Specialized;
using System.Configuration;
using System.ServiceModel.Activities.Tracking.Configuration;
using System.Xml;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{
    public class TrackingComponentElement : TrackingConfigurationElement
    {
        const string nameParameter = "name";
        const string typeParameter = "type";
        const string profileNameParameter = "profileName";
        int argumentsHash;

        NameValueCollection trackingComponentArguments;

        public TrackingComponentElement()
            : base()
        {
            this.trackingComponentArguments = new NameValueCollection();
            this.argumentsHash = this.ComputeArgumentsHash();
        }

        public override object ElementKey
        {
            get { return this.Name; }
        }

        [ConfigurationProperty(nameParameter,
            Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        [StringValidator(MinLength = 0)]
        public string Name
        {
            get
            {
                return (string)this[nameParameter];
            }
            set
            {
                this[nameParameter] = value;
            }
        }

        public NameValueCollection TrackingComponentArguments
        {
            get { return this.trackingComponentArguments; }
        }

        public TrackingProfile TrackingProfile
        {
            get;
            set;
        }

        [ConfigurationProperty(typeParameter, Options = ConfigurationPropertyOptions.IsRequired)]
        [StringValidator(MinLength = 0)]
        public string Type
        {
            get
            {
                return (string)this[typeParameter];
            }
            set
            {
                this[typeParameter] = value;
            }
        }

        [ConfigurationProperty(profileNameParameter, Options = ConfigurationPropertyOptions.None)]
        [StringValidator(MinLength = 0)]
        public string ProfileName
        {
            get
            {
                return (string)this[profileNameParameter];
            }
            set
            {
                this[profileNameParameter] = value;
            }
        }

        //Cache the runtime type. It will be used to create a tracking component per instance.
        internal Type ComponentRuntimeType
        {
            get;
            set;
        }

        protected override bool IsModified()
        {
            return base.IsModified() || this.argumentsHash != this.ComputeArgumentsHash();
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            this.trackingComponentArguments.Add(name, value);

            return true;
        }

        protected override void PostDeserialize()
        {
            this.argumentsHash = this.ComputeArgumentsHash();
            base.PostDeserialize();
        }

        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            bool result;

            if (writer != null)
            {
                if (!serializeCollectionKey)
                {
                    foreach (string key in this.trackingComponentArguments.AllKeys)
                    {
                        writer.WriteAttributeString(key, this.trackingComponentArguments[key]);
                    }
                }

                result = base.SerializeElement(writer, serializeCollectionKey);
                result |= this.trackingComponentArguments.Count > 0;
                this.argumentsHash = this.ComputeArgumentsHash();
            }
            else
            {
                result = base.SerializeElement(writer, serializeCollectionKey);
            }

            return result;
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            if (parentElement is TrackingComponentElement)
            {
                this.trackingComponentArguments = new NameValueCollection(((TrackingComponentElement)parentElement).trackingComponentArguments);
            }
            base.Reset(parentElement);
        }

        protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
        {
            TrackingComponentElement trackingElement = (TrackingComponentElement)sourceElement;
            this.trackingComponentArguments = new NameValueCollection(trackingElement.trackingComponentArguments);
            this.argumentsHash = trackingElement.argumentsHash;
            base.Unmerge(sourceElement, parentElement, saveMode);
        }

        int ComputeArgumentsHash()
        {
            int result = 0;

            foreach (string key in this.trackingComponentArguments.AllKeys)
            {
                if (this.trackingComponentArguments[key] != null)
                {
                    result ^= key.GetHashCode() ^ this.trackingComponentArguments[key].GetHashCode();
                }
            }
            return result;
        }
    }
}

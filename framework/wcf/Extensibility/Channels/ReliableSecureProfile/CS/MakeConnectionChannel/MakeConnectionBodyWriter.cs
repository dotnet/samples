//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    sealed class MakeConnectionBodyWriter : BodyWriter
    {
        string id;

        public MakeConnectionBodyWriter(string id)
            : base(true)
        {
            this.id = id;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(
                MakeConnectionConstants.Prefix,
                MakeConnectionConstants.MakeConnectionMessage.Name,
                MakeConnectionConstants.Namespace);

            writer.WriteElementString(
                MakeConnectionConstants.Prefix,
                MakeConnectionConstants.MakeConnectionMessage.AddressElement,
                MakeConnectionConstants.Namespace,
                this.id);

            writer.WriteEndElement();
        }

        public static MakeConnectionMessageInfo Create(XmlDictionaryReader reader)
        {
            MakeConnectionMessageInfo makeConnectionInfo = new MakeConnectionMessageInfo();

            if (reader.IsStartElement(MakeConnectionConstants.MakeConnectionMessage.Name, MakeConnectionConstants.Namespace))
            {
                reader.ReadStartElement();
                reader.MoveToContent();

                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement(MakeConnectionConstants.MakeConnectionMessage.AddressElement, MakeConnectionConstants.Namespace))
                    {
                        if (!string.IsNullOrEmpty(makeConnectionInfo.Address))
                        {
                            makeConnectionInfo.MultipleAddressHeaders = true;
                            reader.Skip();
                        }
                        else
                        {
                            makeConnectionInfo.Address = reader.ReadElementContentAsString();
                        }
                    }
                    else if (reader.IsStartElement(MakeConnectionConstants.MakeConnectionMessage.IdentifierElement, MakeConnectionConstants.Namespace))
                    {
                        if (makeConnectionInfo.Identifier != null)
                        {
                            makeConnectionInfo.MultipleIdentifierHeaders = true;
                            reader.Skip();
                        }
                        else
                        {
                            makeConnectionInfo.Identifier = reader.ReadElementContentAsUniqueId();
                        }
                    }
                    else 
                    {
                        if (string.IsNullOrEmpty(makeConnectionInfo.UnknownSelection))
                        {
                            makeConnectionInfo.UnknownSelection = reader.LocalName;
                        }

                        reader.Skip();
                    }
                }

                reader.ReadEndElement();
            }

            return makeConnectionInfo;
        }
    }

}

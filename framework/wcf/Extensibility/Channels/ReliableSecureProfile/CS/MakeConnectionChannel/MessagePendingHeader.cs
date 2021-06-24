//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    class MessagePendingHeader : MessageHeader
    {
        bool isPending;
        static bool mustUnderstandValue = true;

        MessagePendingHeader(bool isPending)
            : base()
        {
            this.isPending = isPending;
        }

        public override bool MustUnderstand
        {
            get { return mustUnderstandValue; }
        }

        public override string Name
        {
            get { return MakeConnectionConstants.MessagePending.Name; }
        }

        public override string Namespace
        {
            get { return MakeConnectionConstants.Namespace; }
        }

        public static void AddToMessage(Message message, bool isPending)
        {
            message.Headers.Add(new MessagePendingHeader(isPending));
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement(MakeConnectionConstants.Prefix, this.Name, this.Namespace);
            writer.WriteAttributeString(MakeConnectionConstants.MessagePending.AttibuteName, isPending.ToString().ToLower());
        }

        public static bool FindHeader(Message message)
        {
            bool isPending = false;
            try
            {
                int index = message.Headers.FindHeader(MakeConnectionConstants.MessagePending.Name, MakeConnectionConstants.Namespace);
                if (index != -1)
                {
                    isPending = ReadHeaderValue(message.Headers.GetReaderAtHeader(index));
                    message.Headers.UnderstoodHeaders.Add(message.Headers[index]);
                }
            }
            catch (XmlException)
            {
            }

            return isPending;
        }

        static bool ReadHeaderValue(XmlDictionaryReader reader)
        {
            bool isPending = false;

            if (reader.IsStartElement(MakeConnectionConstants.MessagePending.Name, MakeConnectionConstants.Namespace))
            {
                string isPendingString = reader.GetAttribute(MakeConnectionConstants.MessagePending.AttibuteName, MakeConnectionConstants.Namespace);
                if (isPendingString != null && ToBoolean(isPendingString))
                {
                    isPending = true;
                }
                else
                {
                    isPending = false;
                }

                while (reader.IsStartElement())
                {
                    reader.Skip();
                }

                reader.ReadEndElement();
            }

            return isPending;
        }

        static bool ToBoolean(string value)
        {
            try
            {
                return XmlConvert.ToBoolean(value);
            }
            catch (FormatException exception)
            {
                throw new XmlException(exception.Message);
            }
        }
    }
}

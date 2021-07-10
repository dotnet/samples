//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.LocalChannel
{
    
    internal class LocalByRefMessage : ContentOnlyMessage
    {
        MessageHeaders headers;
        MessageProperties properties;
        int data = int.MinValue;

        internal int Index
        {
            get { return this.data; }
        }

        private LocalByRefMessage(MessageHeaders headers, int id)
        {
            this.headers = headers;
            this.data = id;
        }
        
        internal LocalByRefMessage(MessageVersion version, string action, int id) : base()
        {
            this.headers = new MessageHeaders(version);
            this.headers.Action = action;
            this.data = id;
        }

        public override MessageHeaders Headers
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(ExceptionMessages.MessageClosed);
                }

                return this.headers;
            }
        }


        public override MessageProperties Properties
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(ExceptionMessages.MessageClosed);
                }

                if (this.properties == null)
                {
                    this.properties = new MessageProperties();
                }

                return this.properties;
            }
        }

        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {
            if (this.data != int.MinValue)
            {
                writer.WriteElementString("ID", this.data.ToString());
            }
        }

        public override MessageVersion Version
        {
            get
            {
                return headers.MessageVersion;
            }
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            OnWriteBodyContents(writer);
        }

        internal Message PrepareForSend()
        {
            return new LocalByRefMessage(this.headers, this.data);
        }
    }
}

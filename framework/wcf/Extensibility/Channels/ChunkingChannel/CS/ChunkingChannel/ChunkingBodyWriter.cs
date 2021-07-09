//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.ChunkingChannel
{
    internal class ChunkBodyWriter : BodyWriter
    {
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            //this is where we write the message content
            //this particular implementation calls a delegate to write the content
            this.writeBodyCallback(writer, this.state);
        }

        internal ChunkBodyWriter(WriteBody writeBodyCallback, object state)
            : base(true)
        {
            this.writeBodyCallback = writeBodyCallback;
            this.state = state;
        }

        private WriteBody writeBodyCallback;
        private object state;
        internal delegate void WriteBody(XmlDictionaryWriter writer, object state);
    }
}

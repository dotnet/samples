//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Samples.FormPost
{
    // This class extends the System.ServiceModel.Channels.Message class to expose the
    //  body of the message as a stream instead of the normal XmlDictionaryReader
    public static class MessageExtensions
    {
        public static Stream GetBodyAsStream(this Message message)
        {
            XmlDictionaryReader reader = message.GetReaderAtBodyContents();
            return new XmlReaderStream(reader);
        }

        private class XmlReaderStream : Stream
        {
            XmlDictionaryReader innerReader;

            internal XmlReaderStream(XmlDictionaryReader xmlReader)
            {
                this.innerReader = xmlReader;
                // All "raw" data is tunneled through the Message class by putting it within a
                //  a "Binary" element.  Calling ReadContentAsBase64 on the inner content of
                //  the "Binary" element will get the raw data.
                this.innerReader.ReadStartElement("Binary");
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
                //no-op
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.innerReader.ReadContentAsBase64(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
}

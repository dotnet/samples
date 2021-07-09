//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;

namespace Microsoft.ServiceModel.Samples
{
    [DataContract]
    class FramingData
    {
        [DataMember]
        Uri to;

        [DataMember]
        byte[] payload;

        public FramingData() { }
        public FramingData(Uri to, byte[] payload)
        {
            this.to = to;
            this.payload = payload;
        }

        public Uri To
        {
            get
            {
                return this.to;
            }

            set
            {
                this.to = value;
            }
        }

        public byte[] Payload
        {
            get
            {
                return this.payload;
            }

            set
            {
                this.payload = value;
            }
        }
    }

    class FramingCodec
    {
        static byte[] EncodeInt(int value)
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)((value >> 24) & 0xFF);
            buffer[1] = (byte)((value >> 16) & 0xFF);
            buffer[2] = (byte)((value >> 8) & 0xFF);
            buffer[3] = (byte)(value & 0xFF);
            return buffer;
        }

        static int DecodeInt(byte[] buffer, int offset)
        {
            return (int)((buffer[offset] << 24) |
                (buffer[offset+1] << 16) |
                (buffer[offset+2] << 8) |
                buffer[offset+3]);
        }

        public static FramingData Decode(ArraySegment<byte> buffer)
        {
            // 1. Get Uri length
            int offset = buffer.Offset;
            int uriLength = DecodeInt(buffer.Array, offset);

            // 2. Get Uri
            offset += 4;
            byte[] uriBuffer = new byte[uriLength];
            Buffer.BlockCopy(buffer.Array, offset, uriBuffer, 0, uriLength);
            Uri uri = new Uri (UnicodeEncoding.Unicode.GetString(uriBuffer));

            // 3. Get payload length
            offset += uriLength;
            int payloadLength = DecodeInt(buffer.Array, offset);

            // 4. Get payload
            offset += 4;
            byte[] payload = new byte[payloadLength];
            Buffer.BlockCopy(buffer.Array, offset, payload, 0, payload.Length);
            return new FramingData(uri, payload);
        }

        public static ArraySegment<byte> Encode(Uri uri, ArraySegment<byte> messageBuffer, BufferManager bufferManager)
        {
            byte[] uriBuffer = UnicodeEncoding.Unicode.GetBytes(uri.ToString());
            byte[] uriLengthBuffer = EncodeInt(uriBuffer.Length);
            byte[] payloadLengthBuffer = EncodeInt(messageBuffer.Count);

            // Encode the following fields:
            // Uri length (4 bytes)
            // Uri (size specified by uri length)
            // Payload length (4 bytes)
            // Payload (size specified by payload length)
            byte[] buffer = bufferManager.TakeBuffer(uriLengthBuffer.Length + uriBuffer.Length +
                payloadLengthBuffer.Length + messageBuffer.Count);
           
            int destOffset = 0;
            Buffer.BlockCopy(uriLengthBuffer, 0, buffer, destOffset, uriLengthBuffer.Length);
            destOffset += uriLengthBuffer.Length;
            Buffer.BlockCopy(uriBuffer, 0, buffer, destOffset, uriBuffer.Length);
            destOffset += uriBuffer.Length;
            Buffer.BlockCopy(payloadLengthBuffer, 0, buffer, destOffset, payloadLengthBuffer.Length);
            destOffset += payloadLengthBuffer.Length;
            Buffer.BlockCopy(messageBuffer.Array, messageBuffer.Offset, buffer, destOffset, messageBuffer.Count);

            bufferManager.ReturnBuffer(messageBuffer.Array);
            return new ArraySegment<byte>(buffer);
        }
    }
}


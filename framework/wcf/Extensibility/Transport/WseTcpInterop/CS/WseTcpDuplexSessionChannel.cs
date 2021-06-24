
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Microsoft.Samples.WseTcpTransport
{
    abstract class WseTcpDuplexSessionChannel : ChannelBase, IDuplexSessionChannel
    {
        const int maxBufferSize = 64 * 1024;
        BufferManager bufferManager;
        MessageEncoder encoder;
        EndpointAddress localAddress;
        EndpointAddress remoteAddress;
        Uri via;
        IDuplexSession session;
        Socket socket;
        object readLock = new object();
        object writeLock = new object();

        static class Constants
        {
            public static byte[] WseEndRecord = { 
                    0x0A, 0x40, 0, 0, // version 0x01+ME, no type, no options
                    0, 0, 0, 0, 0, 0, 0, 0 }; // no lengths
        }


        protected MessageEncoder MessageEncoder
        {
            get { return this.encoder; }
        }

        internal static readonly EndpointAddress AnonymousAddress = 
            new EndpointAddress("http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous");

        protected WseTcpDuplexSessionChannel(
            MessageEncoderFactory messageEncoderFactory, BufferManager bufferManager,
            EndpointAddress remoteAddress, EndpointAddress localAddress, Uri via, ChannelManagerBase channelManager)
            : base (channelManager)
        {
            
            this.remoteAddress = remoteAddress;
            this.localAddress = localAddress;
            this.via = via;
            this.session = new TcpDuplexSession(this);
            this.encoder = messageEncoderFactory.CreateSessionEncoder();
            this.bufferManager = bufferManager;
        }

        protected void InitializeSocket(Socket socket)
        {
            if (this.socket != null)
            {
                throw new InvalidOperationException("Socket is already set");
            }

            this.socket = socket;
        }

        protected static Exception ConvertSocketException(SocketException socketException, string operation)
        {
            if (socketException.ErrorCode == 10049 // WSAEADDRNOTAVAIL 
                || socketException.ErrorCode == 10061 // WSAECONNREFUSED 
                || socketException.ErrorCode == 10050 // WSAENETDOWN 
                || socketException.ErrorCode == 10051 // WSAENETUNREACH 
                || socketException.ErrorCode == 10064 // WSAEHOSTDOWN 
                || socketException.ErrorCode == 10065) // WSAEHOSTUNREACH
            {
                return new EndpointNotFoundException(string.Format(operation + " error: {0} ({1})", socketException.Message, socketException.ErrorCode), socketException);
            }
            if (socketException.ErrorCode == 10060) // WSAETIMEDOUT
            {
                return new TimeoutException(operation + " timed out.", socketException);
            }
            else
            {
                return new CommunicationException(string.Format(operation + " error: {0} ({1})", socketException.Message, socketException.ErrorCode), socketException);
            }
        }

        void SocketSend(byte[] buffer)
        {
            SocketSend(new ArraySegment<byte>(buffer));
        }

        IAsyncResult BeginSocketSend(byte[] buffer, AsyncCallback callback, object state)
        {
            return BeginSocketSend(new ArraySegment<byte>(buffer), callback, state);
        }

        IAsyncResult BeginSocketSend(ArraySegment<byte> buffer, AsyncCallback callback, object state)
        {
            return new SocketSendAsyncResult(buffer, this, callback, state);
        }

        void EndSocketSend(IAsyncResult result)
        {
            SocketSendAsyncResult.End(result);
        }

        void SocketSend(ArraySegment<byte> buffer)
        {
            try
            {
                socket.Send(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None);
            }
            catch (SocketException socketException)
            {
                throw ConvertSocketException(socketException, "Send");
            }
        }

        IAsyncResult BeginSocketReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            try
            {
                return socket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
            }
            catch (SocketException socketException)
            {
                throw ConvertSocketException(socketException, "BeginReceive");
            }

        }

        int EndSocketReceive(IAsyncResult result)
        {
            try
            {
                return socket.EndReceive(result);
            }
            catch (SocketException socketException)
            {
                throw ConvertSocketException(socketException, "EndReceive");
            }
        }

        int SocketReceive(byte[] buffer, int offset, int size)
        {
            try
            {
                return socket.Receive(buffer, offset, size, SocketFlags.None);
            }
            catch (SocketException socketException)
            {
                throw ConvertSocketException(socketException, "Receive");
            }
        }

        IAsyncResult BeginSocketReceiveBytes(int size, AsyncCallback callback, object state)
        {
            return BeginSocketReceiveBytes(size, true, callback, state);
        }

        IAsyncResult BeginSocketReceiveBytes(int size, bool throwOnEmpty, AsyncCallback callback, object state)
        {
            return new SocketReceiveAsyncResult(size, throwOnEmpty, this, callback, state);
        }

        byte[] EndSocketReceiveBytes(IAsyncResult result)
        {
            return SocketReceiveAsyncResult.End(result);
        }

        byte[] SocketReceiveBytes(int size)
        {
            return SocketReceiveBytes(size, true);
        }

        byte[] SocketReceiveBytes(int size, bool throwOnEmpty)
        {
            int bytesReadTotal = 0;
            int bytesRead = 0;
            byte[] data = bufferManager.TakeBuffer(size);

            while (bytesReadTotal < size)
            {
                bytesRead = SocketReceive(data, bytesReadTotal, size - bytesReadTotal);
                bytesReadTotal += bytesRead;
                if (bytesRead == 0)
                {
                    if (bytesReadTotal == 0 && !throwOnEmpty)
                    {
                        bufferManager.ReturnBuffer(data);
                        return null;
                    }
                    else
                    {
                        throw new CommunicationException("Premature EOF reached");
                    }
                }
            }

            return data;
        }

        // Address the Message and serialize it into a byte array.
        ArraySegment<byte> EncodeMessage(Message message)
        {
            try
            {
                this.RemoteAddress.ApplyTo(message);
                return encoder.WriteMessage(message, maxBufferSize, bufferManager);
            }
            finally
            {
                // we've consumed the message by serializing it, so clean up
                message.Close();
            }
        }

        Message DecodeMessage(ArraySegment<byte> data)
        {
            if (data.Array == null)
                return null;
            else
                return encoder.ReadMessage(data, bufferManager);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            base.ThrowIfDisposedOrNotOpen();
            lock (writeLock)
            {
                try
                {
                    ArraySegment<byte> encodedBytes = EncodeMessage(message);
                    WriteData(encodedBytes);
                }
                catch (SocketException socketException)
                {
                    throw ConvertSocketException(socketException, "Receive");
                }
            }
        }

        public void Send(Message message)
        {
            this.Send(message, DefaultSendTimeout);
        }

        IAsyncResult BeginReadData(AsyncCallback callback, object state)
        {
            return new ReadDataAsyncResult(this, callback, state);
        }

        ArraySegment<byte> EndReadData(IAsyncResult result)
        {
            return ReadDataAsyncResult.End(result);
        }

        void PrepareDummyRead(byte[] preambleBytes, out int idLength, out int typeLength)
        {
            // drain the ID + TYPE
            idLength = (preambleBytes[4] << 8) + preambleBytes[5];
            typeLength = (preambleBytes[6] << 8) + preambleBytes[7];

            // need to also drain padding
            if ((idLength % 4) > 0)
            {
                idLength += (4 - (idLength % 4));
            }

            if ((typeLength % 4) > 0)
            {
                typeLength += (4 - (typeLength % 4));
            }
        }

        int PrepareDataRead(byte[] preambleBytes,out int bytesToRead)
        {
            // now read the data itself
            int dataLength = (preambleBytes[8] << 24)
                + (preambleBytes[9] << 16)
                + (preambleBytes[10] << 8)
                + preambleBytes[11];

            // total to read should include padding
            bytesToRead = dataLength;
            if ((dataLength % 4) > 0)
            {
                bytesToRead += (4 - (dataLength % 4));
            }
            return dataLength;
        }

        ArraySegment<byte> ReadData()
        {
            // 4 bytes for WSE preamble and 8 bytes for lengths

            byte[] preambleBytes = SocketReceiveBytes(12, false);
            if (preambleBytes == null)
            {
                return new ArraySegment<byte>();
            }

            int idLength, typeLength;

            PrepareDummyRead(preambleBytes, out idLength, out typeLength);
            
            byte[] dummy = SocketReceiveBytes(idLength + typeLength);
            
            this.bufferManager.ReturnBuffer(dummy);

            int bytesToRead;
            int dataLength = PrepareDataRead(preambleBytes, out bytesToRead);
            
            byte[] data = SocketReceiveBytes(bytesToRead);

            if ((preambleBytes[0] & 0x02) == 0)
            {
                byte[] endRecord = SocketReceiveBytes(WseTcpDuplexSessionChannel.Constants.WseEndRecord.Length);
                for (int i = 0; i < WseTcpDuplexSessionChannel.Constants.WseEndRecord.Length; i++)
                {
                    if (endRecord[i] != WseTcpDuplexSessionChannel.Constants.WseEndRecord[i])
                    {
                        throw new CommunicationException("Invalid second framing record");
                    }
                }
                this.bufferManager.ReturnBuffer(endRecord);
            }
            this.bufferManager.ReturnBuffer(preambleBytes);

            return new ArraySegment<byte>(data, 0, dataLength);
        }

        IAsyncResult BeginWriteData(ArraySegment<byte> data, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new WriteDataAsyncResult(data, timeout, this, callback, state);
        }

        void EndWriteData(IAsyncResult result)
        {
            WriteDataAsyncResult.End(result);
        }


        void WriteData(ArraySegment<byte> data)
        {
            ArraySegment<byte> buffer = GetPreDataBuffer(data);
                
            try
            {
                SocketSend(buffer);
                SocketSend(data);

                if ((data.Count % 4) > 0) // need to pad data to multiple of 4 bytes as well
                {
                    byte[] padBytes = new byte[4 - (data.Count % 4)];
                    SocketSend(padBytes);
                }
            }
            finally
            {
                bufferManager.ReturnBuffer(buffer.Array);
            }
        }

        ArraySegment<byte> GetPreDataBuffer(ArraySegment<byte> data)
        {
            byte[] ID = { 0x00, 0x00, 0x00, 0x00 };

            // WSE 3.0 uses the SOAP namespace
            byte[] WsePreamble = {
                0x0E, // version 0x01+MB+ME
                0x20, 0, 0 }; // TYPE_T=URI, no options

            byte[] TYPE;

            if (MessageEncoder.MessageVersion.Envelope == EnvelopeVersion.Soap11)
            {
                TYPE = Encoding.UTF8.GetBytes("http://schemas.xmlsoap.org/soap/envelope/");
            }
            else
            {
                TYPE = Encoding.UTF8.GetBytes("http://www.w3.org/2003/05/soap-envelope");
            }

            // then get the length fields(8 bytes)
            byte[] lengthBytes = new byte[] {
                (byte)((ID.Length & 0x0000FF00) >> 8),
                (byte)(ID.Length & 0x000000FF),
                (byte)((TYPE.Length & 0x0000FF00) >> 8),
                (byte)(TYPE.Length & 0x000000FF),
                (byte)((data.Count & 0xFF000000) >> 24),
                (byte)((data.Count & 0x00FF0000) >> 16),
                (byte)((data.Count & 0x0000FF00) >> 8),
                (byte)(data.Count & 0x000000FF)
                };

            // need to pad to multiple of 4 bytes
            int padLength = 4 - (TYPE.Length % 4);

            int sendLength = TYPE.Length
                + WsePreamble.Length
                + lengthBytes.Length
                + ID.Length
                + padLength;

            byte[] buffer = bufferManager.TakeBuffer(sendLength);
            bool success = false;
            try
            {
                int offset = 0;
                Buffer.BlockCopy(WsePreamble, 0, buffer, offset, WsePreamble.Length);
                offset+=WsePreamble.Length;

                Buffer.BlockCopy(lengthBytes, 0, buffer, offset, lengthBytes.Length);
                offset += lengthBytes.Length;

                Buffer.BlockCopy(ID, 0, buffer, offset, ID.Length);
                offset += ID.Length;

                Buffer.BlockCopy(TYPE, 0, buffer, offset, TYPE.Length);

                success = true;
            }
            finally
            {
                if (!success)
                {
                    bufferManager.ReturnBuffer(buffer);
                }
            }

            return new ArraySegment<byte>(buffer, 0, sendLength);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override void OnAbort()
        {
            if (this.socket != null)
            {
                socket.Close(0);
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            socket.Close((int)timeout.TotalMilliseconds);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            socket.Close((int)timeout.TotalMilliseconds);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        public EndpointAddress LocalAddress
        {
            get { return this.localAddress; }
        }

        public Message Receive()
        {
            return this.Receive(DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            base.ThrowIfDisposedOrNotOpen();
            lock (readLock)
            {
                try
                {
                    ArraySegment<byte> encodedBytes = ReadData();
                    return DecodeMessage(encodedBytes);
                }
                catch (SocketException socketException)
                {
                    throw ConvertSocketException(socketException, "Receive");
                }
            }
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            try
            {
                message = Receive(timeout);
                return true;
            }
            catch (TimeoutException)
            {
                message = null;
                return false;
            }
        }

        public IDuplexSession Session
        {
            get { return this.session; }
        }

        class TcpDuplexSession : IDuplexSession
        {
            WseTcpDuplexSessionChannel channel;
            string id;

            public TcpDuplexSession(WseTcpDuplexSessionChannel channel)
            {
                this.channel = channel;
                this.id = Guid.NewGuid().ToString();
            }

            public void CloseOutputSession(TimeSpan timeout)
            {
                if (channel.State != CommunicationState.Closing)
                {
                    channel.ThrowIfDisposedOrNotOpen();
                }
                channel.socket.Shutdown(SocketShutdown.Send);
            }

            public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
            {
                CloseOutputSession(timeout);
                return new CompletedAsyncResult(callback, state);
            }

            public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
            {
                return BeginCloseOutputSession(channel.DefaultCloseTimeout, callback, state);
            }

            public void EndCloseOutputSession(IAsyncResult result)
            {
                CompletedAsyncResult.End(result);
            }

            public void CloseOutputSession()
            {
                CloseOutputSession(channel.DefaultCloseTimeout);
            }


            public string Id
            {
                get { return this.id; }
            }

        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ReceiveAsyncResult(timeout, this, callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfDisposedOrNotOpen();
            return new TryReceiveAsyncResult(timeout, this, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Message EndReceive(IAsyncResult result)
        {
            return ReceiveAsyncResult.End(result);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            try
            {
                return TryReceiveAsyncResult.End(result, out message);
            }
            catch (TimeoutException)
            {
                message = null;
                return false;
            }
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfDisposedOrNotOpen();
            return new SendAsyncResult(message, timeout, this, callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            SendAsyncResult.End(result);
        }

        public EndpointAddress RemoteAddress
        {
            get { return this.remoteAddress; }
        }

        public Uri Via
        {
            get { return this.via; }
        }

        class ReceiveAsyncResult : AsyncResult
        {
            WseTcpDuplexSessionChannel channel;
            Message message;

            static AsyncCallback onReadDataComplete = new AsyncCallback(OnReadData);

            public ReceiveAsyncResult(TimeSpan timeout, WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;

                if (!channel.IsDisposed)
                {
                    IAsyncResult readDataResult = channel.BeginReadData(callback, state);
                    if (!readDataResult.CompletedSynchronously)
                    {
                        return;
                    }

                    CompleteReadData(readDataResult);
                }

                base.Complete(true);
            }

            void CompleteReadData(IAsyncResult result)
            {
                ArraySegment<byte> data = channel.EndReadData(result);
                this.message = channel.DecodeMessage(data);
            }

            static void OnReadData(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ReceiveAsyncResult thisPtr = (ReceiveAsyncResult)result.AsyncState;

                Exception completionException = null;
                try
                {
                    thisPtr.CompleteReadData(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            public static Message End(IAsyncResult result)
            {
                ReceiveAsyncResult thisPtr = AsyncResult.End<ReceiveAsyncResult>(result);
                return thisPtr.message;
            }
        }

        class ReadDataAsyncResult : AsyncResult
        {
            ArraySegment<byte> buffer;
            WseTcpDuplexSessionChannel channel;
            int dataLength;
            byte[] preambleBytes;
            byte[] data;

            static AsyncCallback drainPreambleCallback = new AsyncCallback(OnDrainPreamble);
            static AsyncCallback dummyCallback = new AsyncCallback(OnDummy);
            static AsyncCallback readDataCallback = new AsyncCallback(OnReadData);
            static AsyncCallback readRecordCallback = new AsyncCallback(OnReadRecord);

            public ReadDataAsyncResult(WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;

                bool success = false;
                try
                {
                    IAsyncResult drainPreambleResult = channel.BeginSocketReceiveBytes(12, false, drainPreambleCallback, this);
                    if (drainPreambleResult.CompletedSynchronously)
                    {
                        if (CompleteDrainPreamble(drainPreambleResult))
                        {
                            base.Complete(true);
                        }
                    }
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        this.Cleanup();
                    }
                }
            }

            bool CompleteDrainPreamble(IAsyncResult result)
            {
                this.preambleBytes = channel.EndSocketReceiveBytes(result);
                if (this.preambleBytes == null)
                {
                    this.buffer = new ArraySegment<byte>();
                    return true;
                }

                int idLength, typeLength;
                channel.PrepareDummyRead(preambleBytes, out idLength, out typeLength);

                IAsyncResult dummyResult = channel.BeginSocketReceiveBytes(idLength + typeLength, dummyCallback, this);
                if (!dummyResult.CompletedSynchronously)
                {
                    return false;
                }

                return CompleteDummy(dummyResult);
            }

            bool CompleteDummy(IAsyncResult result)
            {
                byte[] bytes = channel.EndSocketReceiveBytes(result);
                channel.bufferManager.ReturnBuffer(bytes);

                int bytesToRead;
                dataLength = channel.PrepareDataRead(preambleBytes, out bytesToRead);

                IAsyncResult readDataResult = channel.BeginSocketReceiveBytes(bytesToRead, readDataCallback, this);
                if (!readDataResult.CompletedSynchronously)
                {
                    return false;
                }

                return CompleteReadData(result);
            }

            bool CompleteReadData(IAsyncResult result)
            {
                data = channel.EndSocketReceiveBytes(result);

                if ((preambleBytes[0] & 0x02) == 0)
                {
                    IAsyncResult readRecordResult = channel.BeginSocketReceiveBytes(
                        WseTcpDuplexSessionChannel.Constants.WseEndRecord.Length, readRecordCallback, this);
                    if (!readRecordResult.CompletedSynchronously)
                    {
                        return false;
                    }

                    return CompleteReadRecord(readRecordResult);
                }
                else
                {
                    this.buffer = new ArraySegment<byte>(data, 0, dataLength);

                    CleanupPreamble();
                    return true;
                }
            }

            bool CompleteReadRecord(IAsyncResult result)
            {
                byte[] endRecord = channel.EndSocketReceiveBytes(result);
                for (int i = 0; i < WseTcpDuplexSessionChannel.Constants.WseEndRecord.Length; i++)
                {
                    if (endRecord[i] != WseTcpDuplexSessionChannel.Constants.WseEndRecord[i])
                    {
                        this.channel.bufferManager.ReturnBuffer(endRecord);
                        throw new CommunicationException("Invalid second framing record");
                    }
                }
                
                this.channel.bufferManager.ReturnBuffer(endRecord);

                this.buffer = new ArraySegment<byte>(data, 0, dataLength);
                
                CleanupPreamble(); 
                return true;
            }

            static void OnDrainPreamble(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ReadDataAsyncResult thisPtr = (ReadDataAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteDrainPreamble(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                    thisPtr.Cleanup();
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            static void OnDummy(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ReadDataAsyncResult thisPtr = (ReadDataAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteDummy(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                    thisPtr.Cleanup();
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            static void OnReadData(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ReadDataAsyncResult thisPtr = (ReadDataAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteReadData(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                    thisPtr.Cleanup();
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }


            static void OnReadRecord(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ReadDataAsyncResult thisPtr = (ReadDataAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteReadRecord(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                    thisPtr.Cleanup();
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            public static ArraySegment<byte> End(IAsyncResult result)
            {
                ReadDataAsyncResult thisPtr = AsyncResult.End<ReadDataAsyncResult>(result);
                return thisPtr.buffer;
            }

            void Cleanup()
            {
                if (this.data != null)
                {
                    this.channel.bufferManager.ReturnBuffer(data);
                    this.data = null;
                }
                CleanupPreamble();
            }

            void CleanupPreamble()
            {
                if (this.preambleBytes != null)
                {
                    this.channel.bufferManager.ReturnBuffer(preambleBytes);
                    this.preambleBytes = null;
                }

            }
        }

        class SocketReceiveAsyncResult : AsyncResult
        {
            WseTcpDuplexSessionChannel channel;
            int size;
            int bytesReadTotal;
            byte[] buffer;
            bool throwOnEmpty;

            static AsyncCallback readBytesCallback = new AsyncCallback(OnReadBytes);

            public SocketReceiveAsyncResult(int size, bool throwOnEmpty, WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.size = size;
                this.channel = channel;
                this.throwOnEmpty = throwOnEmpty;
                this.bytesReadTotal = 0;
                this.buffer = channel.bufferManager.TakeBuffer(size);

                bool success = false;
                try
                {
                    IAsyncResult socketReceiveResult = channel.BeginSocketReceive(this.buffer, bytesReadTotal, size, readBytesCallback, this);
                    if (socketReceiveResult.CompletedSynchronously)
                    {
                        if (CompleteReadBytes(socketReceiveResult))
                        {
                            base.Complete(true);
                        }
                    }
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        this.Cleanup();
                    }
                }
            }

            void Cleanup()
            {
                if (this.buffer != null)
                {
                    channel.bufferManager.ReturnBuffer(this.buffer);
                    this.buffer = null;
                }
            }

            bool CompleteReadBytes(IAsyncResult result)
            {
                int bytesRead = channel.EndSocketReceive(result);
                bytesReadTotal += bytesRead;
                if (bytesRead == 0)
                {
                    if (size == 0 && !throwOnEmpty)
                    {
                        channel.bufferManager.ReturnBuffer(this.buffer);
                        this.buffer = null;
                        return true;
                    }
                    else
                    {
                        throw new CommunicationException("Premature EOF reached");
                    }
                }

                while (bytesReadTotal < size)
                {
                    IAsyncResult socketReceiveResult = channel.BeginSocketReceive(buffer, bytesReadTotal, size - bytesReadTotal, readBytesCallback, this);
                    if (!socketReceiveResult.CompletedSynchronously)
                    {
                        return false;
                    }
                }

                return true;
            }

            static void OnReadBytes(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                SocketReceiveAsyncResult thisPtr = (SocketReceiveAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteReadBytes(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                    thisPtr.Cleanup();
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            public static byte[] End(IAsyncResult result)
            {
                SocketReceiveAsyncResult thisPtr = AsyncResult.End<SocketReceiveAsyncResult>(result);
                return thisPtr.buffer;
            }
        }

        class TryReceiveAsyncResult : AsyncResult
        {
            WseTcpDuplexSessionChannel channel;
            static AsyncCallback receiveCallback = new AsyncCallback(OnReceive);
            bool receiveSuccess;
            Message message;

            public TryReceiveAsyncResult(TimeSpan timeout, WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;

                bool completeSelf = true;
                if (!channel.IsDisposed)
                {
                    try
                    {
                        IAsyncResult beginReceiveResult = this.channel.BeginReceive(timeout, receiveCallback, this);
                        if (beginReceiveResult.CompletedSynchronously)
                        {
                            CompleteReceive(beginReceiveResult);
                        }
                        else
                        {
                            completeSelf = false;
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }

                if (completeSelf)
                {
                    base.Complete(true);
                }
            }

            public static bool End(IAsyncResult result, out Message message)
            {
                TryReceiveAsyncResult thisPtr = AsyncResult.End<TryReceiveAsyncResult>(result);
                message = thisPtr.message;
                return thisPtr.receiveSuccess;
            }

            void CompleteReceive(IAsyncResult result)
            {
                this.message = this.channel.EndReceive(result);
                this.receiveSuccess = true;
            }

            static void OnReceive(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                TryReceiveAsyncResult thisPtr = (TryReceiveAsyncResult)result.AsyncState;
                Exception completionException = null;
                try
                {
                    thisPtr.CompleteReceive(result);
                }
                catch (TimeoutException)
                {
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }
        }

        class SendAsyncResult : AsyncResult
        {
            WseTcpDuplexSessionChannel channel;
            AsyncCallback writeCallback = new AsyncCallback(OnWrite);

            public SendAsyncResult(Message message, TimeSpan timeout, WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                : base(callback, state)

            {
                this.channel = channel;

                ArraySegment<byte> encodedBytes = this.channel.EncodeMessage(message);

                IAsyncResult writeResult = channel.BeginWriteData(encodedBytes, timeout, writeCallback, this);
                if (!writeResult.CompletedSynchronously)
                {
                    return;
                }

                CompleteWrite(writeResult);
                base.Complete(true);
            }

            void CompleteWrite(IAsyncResult result)
            {
                try
                {
                    channel.EndWriteData(result);
                }
                catch (SocketException socketException)
                {
                    throw ConvertSocketException(socketException, "Receive");
                }
            }

            static void OnWrite(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                SendAsyncResult thisPtr = (SendAsyncResult)result.AsyncState;
                Exception completionException = null;
                try
                {
                    thisPtr.CompleteWrite(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SendAsyncResult>(result);
            }

        }

        class WriteDataAsyncResult : AsyncResult
        {
            WseTcpDuplexSessionChannel channel;
            ArraySegment<byte> data;
            ArraySegment<byte> preDataBytes;
            AsyncCallback preDataCallback = new AsyncCallback(OnPreData);
            AsyncCallback sendDataCallback = new AsyncCallback(OnSendData);
            AsyncCallback padDataCallback = new AsyncCallback(OnPadData);

            public WriteDataAsyncResult(ArraySegment<byte> data, TimeSpan timeout, WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;
                this.data = data;
                bool success = false;
                try
                {
                    preDataBytes = this.channel.GetPreDataBuffer(data);

                    IAsyncResult preDataResult = channel.BeginSocketSend(preDataBytes, preDataCallback, this);
                    if (!preDataResult.CompletedSynchronously)
                    {
                        return;
                    }

                    if (CompletePreData(preDataResult))
                    {
                        Cleanup();
                        base.Complete(true);
                    }
                    success = true;
                }
                finally
                {
                    if (!success)
                    {
                        Cleanup();
                    }
                }
            }

            bool CompletePreData(IAsyncResult result)
            {
                channel.EndSocketSend(result);

                IAsyncResult sendDataResult = channel.BeginSocketSend(data, sendDataCallback, this);
                if (!sendDataResult.CompletedSynchronously)
                {
                    return false;
                }

                return CompleteSendData(sendDataResult);
            }

            bool CompleteSendData(IAsyncResult result)
            {
                channel.EndSocketSend(result);

                if ((data.Count % 4) > 0) // need to pad data to multiple of 4 bytes as well
                {
                    byte[] padBytes = new byte[4 - (data.Count % 4)];
                    IAsyncResult padResult = channel.BeginSocketSend(padBytes, padDataCallback, this);
                    if (!padResult.CompletedSynchronously)
                    {
                        return false;
                    }

                    CompletePadData(padResult);
                }

                return true;
            }

            void CompletePadData(IAsyncResult result)
            {
                channel.EndSocketSend(result);
            }

            static void OnPreData(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                WriteDataAsyncResult thisPtr = (WriteDataAsyncResult)result.AsyncState;
                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompletePreData(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                }
                finally
                {
                    thisPtr.Cleanup();
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            static void OnSendData(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                WriteDataAsyncResult thisPtr = (WriteDataAsyncResult)result.AsyncState;
                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteSendData(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            static void OnPadData(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                WriteDataAsyncResult thisPtr = (WriteDataAsyncResult)result.AsyncState;
                Exception completionException = null;
                try
                {
                    thisPtr.CompletePadData(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            

            public void Cleanup()
            {
                if (preDataBytes != null && preDataBytes.Array != null)
                {
                    this.channel.bufferManager.ReturnBuffer(preDataBytes.Array);
                    preDataBytes = new ArraySegment<byte>();
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<WriteDataAsyncResult>(result);
            }
        }

        class SocketSendAsyncResult : AsyncResult
        {
            WseTcpDuplexSessionChannel channel;
            static AsyncCallback sendCallback = new AsyncCallback(OnSend);

            public SocketSendAsyncResult(ArraySegment<byte> buffer, WseTcpDuplexSessionChannel channel, AsyncCallback callback, object state) 
                : base (callback, state)
            {
                this.channel = channel;

                IAsyncResult sendResult = channel.socket.BeginSend(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, sendCallback,this);
                if (!sendResult.CompletedSynchronously)
                {
                    return;
                }

                CompleteSend(sendResult);
                base.Complete(true);
            }

            void CompleteSend(IAsyncResult result)
            {
                try
                {
                    channel.socket.EndSend(result);
                }
                catch (SocketException socketException)
                {
                    throw ConvertSocketException(socketException, "Send");
                }
            }

            static void OnSend(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                SocketSendAsyncResult thisPtr = (SocketSendAsyncResult)result.AsyncState;
                Exception completionException = null;
                try
                {
                    thisPtr.CompleteSend(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }


            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SocketSendAsyncResult>(result);
            }
        }
    }
}

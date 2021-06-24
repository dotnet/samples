
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.ServiceModel.Samples
{
    class UdpSocketListener
    {
        bool multicast = false;
        List<Socket> listenSockets;
        object syncRoot = new object();
        int maxMessageSize;
        BufferManager bufferManager;
        AsyncCallback onReceive;
        int refCount;
        bool closed = false;
        IPAddress ipAddress;
        int port;
        DataReceivedCallback dataReceivedCallback;

        public UdpSocketListener(IPAddress ipAddress, int port, bool multicast, int maxBufferPoolSize, int maxMessageSize, DataReceivedCallback dataReceivedCallback)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.multicast = multicast;
            this.maxMessageSize = maxMessageSize;
            this.listenSockets = null;
            this.bufferManager = BufferManager.CreateBufferManager(maxBufferPoolSize, maxMessageSize);
            this.dataReceivedCallback = dataReceivedCallback;
        }

        // Listen sockets have been created.
        public UdpSocketListener(List<Socket> listenSockets, int maxBufferPoolSize, int maxMessageSize, DataReceivedCallback dataReceivedCallback)
        {
            this.listenSockets = listenSockets;
            this.maxMessageSize = maxMessageSize;
            this.bufferManager = BufferManager.CreateBufferManager(maxBufferPoolSize, maxMessageSize);
            this.dataReceivedCallback = dataReceivedCallback;
        }

        object ThisLock
        {
            get
            {
                return syncRoot;
            }
        }

        public void Open()
        {
            if (listenSockets == null)
            {
                listenSockets = new List<Socket>();
                if (!ipAddress.Equals(IPAddress.Broadcast))
                {
                    listenSockets.Add(CreateListenSocket(this.ipAddress, this.port, multicast));
                }
                else
                {
                    listenSockets.Add(CreateListenSocket(IPAddress.Any, this.port, multicast));
                    if (Socket.OSSupportsIPv6)
                    {
                        listenSockets.Add(CreateListenSocket(IPAddress.IPv6Any, this.port, multicast));
                    }
                }
            }

            this.onReceive = new AsyncCallback(this.OnReceive);
            WaitCallback startReceivingCallback = new WaitCallback(StartReceiving);

            Socket[] socketsSnapshot = listenSockets.ToArray();
            for (int i = 0; i < socketsSnapshot.Length; i++)
            {
                ThreadPool.QueueUserWorkItem(startReceivingCallback, socketsSnapshot[i]);
            }
        }

        public int AddRef()
        {
            return Interlocked.Increment(ref refCount);
        }

        public int Release()
        {
            int count = Interlocked.Decrement(ref refCount);
            if (count == 0)
            {
                Stop();
            }

            return count;
        }

        void CloseListenSockets(TimeSpan timeout)
        {
            // FUTURE: telescoping timeout
            for (int i = 0; i < listenSockets.Count; i++)
            {
                this.listenSockets[i].Close((int)timeout.TotalMilliseconds);
            }
            listenSockets.Clear();
        }

        void Stop()
        {
            // FUTURE: use appropriate timeout
            CloseListenSockets(TimeSpan.MaxValue);

            if (this.bufferManager != null)
            {
                this.bufferManager.Clear();
            }
        }

        public static Socket CreateListenSocket(IPAddress ipAddress, int port, bool multicast)
        {
            bool isIPv6 = (ipAddress.AddressFamily == AddressFamily.InterNetworkV6);
            Socket socket = null;

            if (multicast)
            {
                IPAddress anyIPAddr = IPAddress.Any;
                if (isIPv6)
                    anyIPAddr = IPAddress.IPv6Any;

                IPEndPoint endPoint = new IPEndPoint(anyIPAddr, port);
                socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                socket.Bind(endPoint);

                if (isIPv6)
                {
                    socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
                        new IPv6MulticastOption(ipAddress));
                }
                else
                {
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(ipAddress));
                }
            }
            else
            {
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(endPoint);
            }

            return socket;
        }

        EndPoint CreateDummyEndPoint(Socket socket)
        {
            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                return new IPEndPoint(IPAddress.Any, 0);
            }
            else
            {
                return new IPEndPoint(IPAddress.IPv6Any, 0);
            }
        }

        void StartReceiving(object state)
        {
            Socket listenSocket = (Socket)state;
            IAsyncResult result = null;

            try
            {
                lock (ThisLock)
                {
                    if (!closed)
                    {
                        EndPoint dummy = CreateDummyEndPoint(listenSocket);
                        byte[] buffer = this.bufferManager.TakeBuffer(maxMessageSize);
                        result = listenSocket.BeginReceiveFrom(buffer, 0, buffer.Length,
                            SocketFlags.None, ref dummy, this.onReceive, new SocketReceiveState(listenSocket, buffer));
                    }
                }

                if (result != null && result.CompletedSynchronously)
                {
                    ContinueReceiving(result, listenSocket);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in receiving from the socket.");
                Debug.WriteLine(e.ToString());
            }
        }

        void ContinueReceiving(IAsyncResult receiveResult, Socket listenSocket)
        {
            bool continueReceiving = true;

            while (continueReceiving)
            {
                FramingData data = null;

                if (receiveResult != null)
                {
                    data = EndReceive(listenSocket, receiveResult);
                    receiveResult = null;
                }

                lock (ThisLock)
                {
                    if (!closed)
                    {
                        EndPoint dummy = CreateDummyEndPoint(listenSocket);
                        byte[] buffer = this.bufferManager.TakeBuffer(maxMessageSize);
                        receiveResult = listenSocket.BeginReceiveFrom(buffer, 0, buffer.Length,
                            SocketFlags.None, ref dummy, this.onReceive, new SocketReceiveState(listenSocket, buffer));
                    }
                }

                if (receiveResult == null || !receiveResult.CompletedSynchronously)
                {
                    continueReceiving = false;
                    Dispatch(data);
                }
                else if (data != null)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DispatchCallback), data);
                }
            }
        }

        FramingData EndReceive(Socket listenSocket, IAsyncResult result)
        {
            // if we've started the shutdown process, then we've disposed
            // the socket and calls to socket.EndReceive will throw 
            if (closed)
                return null;

            byte[] buffer = ((SocketReceiveState)result.AsyncState).Buffer;
            Debug.Assert(buffer != null);
            FramingData data = null;

            try
            {
                int count = 0;

                lock (ThisLock)
                {
                    // if we've started the shutdown process, socket is disposed
                    // and calls to socket.EndReceive will throw 
                    if (!closed)
                    {
                        EndPoint dummy = CreateDummyEndPoint(listenSocket);
                        count = listenSocket.EndReceiveFrom(result, ref dummy);
                    }
                }

                if (count > 0)
                {
                    data = FramingCodec.Decode(new ArraySegment<byte>(buffer, 0, count));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in completing the async receive via EndReceiveFrom method.");
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                if (data == null)
                {
                    this.bufferManager.ReturnBuffer(buffer);
                    buffer = null;
                }
            }

            return data;
        }

        //Called when an ansynchronous receieve operation completes
        //on the listening socket.
        void OnReceive(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ContinueReceiving(result, ((SocketReceiveState)result.AsyncState).Socket);
        }

        void DispatchCallback(object state)
        {
            Dispatch((FramingData)state);
        }

        void Dispatch(FramingData data)
        {
            if (data == null)
            {
                // FUTURE: Error handling
                return;
            }

            dataReceivedCallback(data);
        }

        class SocketReceiveState
        {
            Socket socket;
            byte[] buffer;
            public SocketReceiveState(Socket socket, byte[] buffer)
            {
                this.socket = socket;
                this.buffer = buffer;
            }

            public Socket Socket
            {
                get { return this.socket; }
            }

            public byte[] Buffer
            {
                get { return this.buffer; }
            }
        }
    }
}


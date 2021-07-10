//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{
    static class LocalTransportManager
    {
        static Dictionary<Uri, LocalChannelListener>routingTable = new Dictionary<Uri, LocalChannelListener>();

        public static void RegisterListener(LocalChannelListener listener)
        {
            LocalChannelListener listenerOut;
            lock (routingTable)
            {
                if (!routingTable.TryGetValue(listener.Uri, out listenerOut))
                {
                    routingTable.Add(listener.Uri, listener);
                }
                else
                {
                    throw new InvalidOperationException(String.Format
                        (ExceptionMessages.ListenerAlreadyRegistered, listener.Uri));
                }
            }
        }

        public static void UnregisterListener(LocalChannelListener listener)
        {
            lock (routingTable)
            {
                if (!routingTable.Remove(listener.Uri))
                {
                    throw new InvalidOperationException(String.Format
                        (ExceptionMessages.NoListenerRegistered, listener.Uri));
                }
            }
        }

        public static void ConnectServerChannel(InputQueue<Message> inputQueue,
            InputQueue<Message> outputQueue,
            EndpointAddress remoteAddress, Uri via,
            TimeSpan timeout)
        {
            LocalChannelListener channelListener;
            IDuplexSessionChannel serverChannel = CreateServerChannel
                (inputQueue, outputQueue, remoteAddress, via, out channelListener);
            channelListener.EnqueueNewChannel(serverChannel, timeout);
        }

        public static IAsyncResult BeginConnectServerChannel(InputQueue<Message> inputQueue,
            InputQueue<Message> outputQueue,
            EndpointAddress remoteAddress, Uri via,
            TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            LocalChannelListener channelListener;
            IDuplexSessionChannel serverChannel = CreateServerChannel
                (inputQueue, outputQueue, remoteAddress, via, out channelListener);
            return channelListener.BeginEnqueueNewChannel(serverChannel, timeout, callback, state);
        }

        public static void EndConnectServerChannel(IAsyncResult result, Uri via)
        {
            LocalChannelListener channelListener;
            lock (routingTable)
            {
                if (!routingTable.TryGetValue(via, out channelListener))
                {
                    throw new EndpointNotFoundException(String.Format
                        (ExceptionMessages.ChannelListenerNotFound, via.ToString()));
                }
            }

            channelListener.EndEnqueueNewChannel(result);
        }

        static IDuplexSessionChannel CreateServerChannel(InputQueue<Message> inputQueue,
            InputQueue<Message> outputQueue,
            EndpointAddress remoteAddress, Uri via,
            out LocalChannelListener channelListener)
        {
            lock (routingTable)
            {
                if (!routingTable.TryGetValue(via, out channelListener))
                {
                    throw new EndpointNotFoundException(String.Format
                        (ExceptionMessages.ChannelListenerNotFound, via.ToString()));
                }
            }

            return new ServerLocalDuplexSessionChannel(channelListener, 
                inputQueue, outputQueue, remoteAddress, via);
        }

        class ServerLocalDuplexSessionChannel : LocalDuplexSessionChannel
        {
            internal ServerLocalDuplexSessionChannel(LocalChannelListener listener, 
                InputQueue<Message>receiveQueue, InputQueue<Message>sendQueue,
                EndpointAddress address, Uri via)
                : base(listener, receiveQueue, sendQueue, address, via)
            {
            }

            protected override void OnOpen(TimeSpan timeout)
            {
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return new CompletedAsyncResult(callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                CompletedAsyncResult.End(result);
            }
        }
    }
}

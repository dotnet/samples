//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{
    class LocalChannelFactory : ChannelFactoryBase<IDuplexSessionChannel>
    {
        internal LocalChannelFactory(BindingContext context)
            : base(context.Binding)
        {
        }

        public override T GetProperty<T>()
        {
            if (typeof(T).TypeHandle.Equals(typeof(MessageVersion).TypeHandle))
            {
                return (T)(object)LocalTransportDefaults.MessageVersionLocal;
            }

            return base.GetProperty<T>();
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

        protected override IDuplexSessionChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            ValidateScheme(via);

            InputQueue<Message> forwardQueue = new InputQueue<Message>();
            InputQueue<Message> reverseQueue = new InputQueue<Message>();

            return new ClientLocalDuplexSessionChannel(this, reverseQueue, forwardQueue, remoteAddress, via);
        }

        void ValidateScheme(Uri via)
        {
            if (string.Compare(via.Scheme, LocalTransportDefaults.UriSchemeLocal, 
                    StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new ArgumentException(String.Format(ExceptionMessages.InvalidUriScheme, 
                    via.Scheme, LocalTransportDefaults.UriSchemeLocal));
            }
        }
    }

    class ClientLocalDuplexSessionChannel : LocalDuplexSessionChannel
    {
        public ClientLocalDuplexSessionChannel(LocalChannelFactory factory,
                InputQueue<Message> receiveQueue, InputQueue<Message> sendQueue,
                    EndpointAddress address, Uri via)
            : base(factory, receiveQueue, sendQueue, address, via)
        {
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            LocalTransportManager.ConnectServerChannel(this.sendQueue, this.receiveQueue, this.LocalAddress, this.Via, timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return LocalTransportManager.BeginConnectServerChannel(this.sendQueue, this.receiveQueue,
                this.LocalAddress, this.Via, timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            LocalTransportManager.EndConnectServerChannel(result, this.Via);
        }
    }
}

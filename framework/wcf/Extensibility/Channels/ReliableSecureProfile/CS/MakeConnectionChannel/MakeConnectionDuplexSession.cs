//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public class MakeConnectionDuplexSession : IDuplexSession
    {
        TimeSpan closeTimeout;
        ISession innerSession;

        MakeConnectionDuplexSession(IDefaultCommunicationTimeouts timeouts, ISession innerSession)
        {
            this.closeTimeout = timeouts.CloseTimeout;
            this.innerSession = innerSession;
        }

        public static IDuplexSession CreateClientSession(IDefaultCommunicationTimeouts timeouts, ISession innerSession)
        {
            if (innerSession is ISecureConversationSession)
            {
                return new MakeConnectionDuplexSecureConversationSession(timeouts, innerSession);
            }
            else if (innerSession is ISecuritySession)
            {
                return new MakeConnectionDuplexSecuritySession(timeouts, innerSession);
            }

            return new MakeConnectionDuplexSession(timeouts, innerSession);
        }

        public static IDuplexSession CreateServerSession(MakeConnectionDuplexSessionServiceChannel channel, ISession innerSession)
        {
            if (innerSession is ISecureConversationSession)
            {
                return new MakeConnectionDuplexServerSecureConversationSession(channel, innerSession);
            }
            else if (innerSession is ISecuritySession)
            {
                return new MakeConnectionDuplexServerSecuritySession(channel, innerSession);
            }

            return new MakeConnectionDuplexServerSession(channel, innerSession);
        }

        public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnBeginCloseOutputSession(timeout, callback, state);
        }

        public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
        {
            return this.BeginCloseOutputSession(this.closeTimeout, callback, state);
        }

        protected virtual IAsyncResult OnBeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        public void CloseOutputSession(TimeSpan timeout)
        {
            OnCloseOutputSession(timeout);
        }

        public void CloseOutputSession()
        {
            this.CloseOutputSession(closeTimeout);
        }

        protected virtual void OnCloseOutputSession(TimeSpan timeout)
        {
        }

        public void EndCloseOutputSession(IAsyncResult result)
        {
            OnEndCloseOutputSession(result);
        }

        protected virtual void OnEndCloseOutputSession(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        public string Id
        {
            get
            {
                return this.innerSession.Id;
            }
        }

        protected ISession InnerSession
        {
            get
            {
                return this.innerSession;
            }
        }

        class MakeConnectionDuplexSecureConversationSession : MakeConnectionDuplexSecuritySession, ISecureConversationSession
        {
            public MakeConnectionDuplexSecureConversationSession(IDefaultCommunicationTimeouts timeouts, ISession innerSession)
                : base(timeouts, innerSession)
            {
            }

            public bool TryReadSessionTokenIdentifier(XmlReader reader)
            {
                return ((ISecureConversationSession)this.InnerSession).TryReadSessionTokenIdentifier(reader);
            }

            public void WriteSessionTokenIdentifier(XmlDictionaryWriter writer)
            {
                ((ISecureConversationSession)this.InnerSession).WriteSessionTokenIdentifier(writer);
            }
        }

        class MakeConnectionDuplexSecuritySession : MakeConnectionDuplexSession, ISecuritySession
        {
            public MakeConnectionDuplexSecuritySession(IDefaultCommunicationTimeouts timeouts, ISession innerSession)
                : base(timeouts, innerSession)
            {
            }

            public EndpointIdentity RemoteIdentity
            {
                get 
                { 
                    return ((ISecuritySession)this.InnerSession).RemoteIdentity; 
                }
            }
        }

        class MakeConnectionDuplexServerSecureConversationSession : MakeConnectionDuplexServerSecuritySession, ISecureConversationSession
        {
            public MakeConnectionDuplexServerSecureConversationSession(MakeConnectionDuplexSessionServiceChannel channel, ISession innerSession)
                : base(channel, innerSession)
            {
            }

            public bool TryReadSessionTokenIdentifier(XmlReader reader)
            {
                return ((ISecureConversationSession)this.InnerSession).TryReadSessionTokenIdentifier(reader);
            }

            public void WriteSessionTokenIdentifier(XmlDictionaryWriter writer)
            {
                ((ISecureConversationSession)this.InnerSession).WriteSessionTokenIdentifier(writer);
            }
        }

        class MakeConnectionDuplexServerSecuritySession : MakeConnectionDuplexServerSession, ISecuritySession
        {
            public MakeConnectionDuplexServerSecuritySession(MakeConnectionDuplexSessionServiceChannel channel, ISession innerSession)
                : base(channel, innerSession)
            {
            }

            public EndpointIdentity RemoteIdentity
            {
                get
                {
                    return ((ISecuritySession)this.InnerSession).RemoteIdentity;
                }
            }
        }

        class MakeConnectionDuplexServerSession : MakeConnectionDuplexSession
        {
            MakeConnectionDuplexSessionServiceChannel channel;

            public MakeConnectionDuplexServerSession(MakeConnectionDuplexSessionServiceChannel channel, ISession innerSession)
                : base((IDefaultCommunicationTimeouts)channel, innerSession)
            {
                this.channel = channel;
            }

            protected override void OnCloseOutputSession(TimeSpan timeout)
            {
                this.channel.DispatcherManager.Shutdown();
            }

            protected override IAsyncResult OnBeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
            {
                this.channel.DispatcherManager.Shutdown();
                return base.OnBeginCloseOutputSession(timeout, callback, state);
            }
        }
    }
}

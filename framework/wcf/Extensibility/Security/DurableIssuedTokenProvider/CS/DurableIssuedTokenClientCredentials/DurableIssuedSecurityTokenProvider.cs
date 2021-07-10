//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using System.ServiceModel;
using System.ServiceModel.Security.Tokens;

using System.Threading;

namespace Microsoft.Samples.DurableIssuedTokenProvider
{
    class DurableIssuedSecurityTokenProvider : SecurityTokenProvider, ICommunicationObject
    {
        IssuedSecurityTokenProvider innerTokenProvider;
        IssuedTokenCache cache;
        EndpointAddress target;
        EndpointAddress issuer;

        public DurableIssuedSecurityTokenProvider(IssuedSecurityTokenProvider innerTokenProvider, IssuedTokenCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            if (innerTokenProvider == null)
            {
                throw new ArgumentNullException("innerTokenProvider");
            }
            this.innerTokenProvider = innerTokenProvider;
            this.cache = cache;
            this.target = innerTokenProvider.TargetAddress;
            this.issuer = innerTokenProvider.IssuerAddress;
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            GenericXmlSecurityToken token;
            if (!this.cache.TryGetToken(target, issuer, out token))
            {
                token = (GenericXmlSecurityToken) this.innerTokenProvider.GetToken(timeout);
                this.cache.AddToken(token, target, issuer);
            }
            return token;
        }

        protected override IAsyncResult  BeginGetTokenCore(TimeSpan timeout, AsyncCallback callback, object state)
        {
            GenericXmlSecurityToken token;
            if (this.cache.TryGetToken(target, issuer, out token))
            {
                return new CompletedAsyncResult(token, callback, state);
            }
            else
            {
                return this.innerTokenProvider.BeginGetToken(timeout, callback, state);
            }
        }

        protected override SecurityToken EndGetTokenCore(IAsyncResult result)
        {
            CompletedAsyncResult caResult = result as CompletedAsyncResult;

            if (caResult != null)
            {
                return caResult.Token;
            }
            else
            {
                GenericXmlSecurityToken token = (GenericXmlSecurityToken) this.innerTokenProvider.EndGetToken(result);
                this.cache.AddToken(token, target, issuer);
                return token;
            }
        }

        #region ICommunicationObject Members

        public void Abort()
        {
            this.innerTokenProvider.Abort();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerTokenProvider.BeginClose(timeout, callback, state);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return this.innerTokenProvider.BeginClose(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerTokenProvider.BeginOpen(timeout, callback, state);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return this.innerTokenProvider.BeginOpen(callback, state);
        }

        public void Close(TimeSpan timeout)
        {
            this.innerTokenProvider.Close(timeout);
        }

        public void Close()
        {
            this.innerTokenProvider.Close();
        }

        public event EventHandler Closed
        {
            add { this.innerTokenProvider.Closed += value; }
            remove { this.innerTokenProvider.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { this.innerTokenProvider.Closing += value; }
            remove { this.innerTokenProvider.Closing -= value; }
        }

        public void EndClose(IAsyncResult result)
        {
            this.innerTokenProvider.EndClose(result);
        }

        public void EndOpen(IAsyncResult result)
        {
            this.innerTokenProvider.EndOpen(result);
        }

        public event EventHandler Faulted
        {
            add { this.innerTokenProvider.Faulted += value; }
            remove { this.innerTokenProvider.Faulted -= value; }
        }

        public void Open(TimeSpan timeout)
        {
            this.innerTokenProvider.Open(timeout);
        }

        public void Open()
        {
            this.innerTokenProvider.Open();
        }

        public event EventHandler Opened
        {
            add { this.innerTokenProvider.Opened += value; }
            remove { this.innerTokenProvider.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { this.innerTokenProvider.Opening += value; }
            remove { this.innerTokenProvider.Opening -= value; }
        }

        public CommunicationState State
        {
            get { return this.innerTokenProvider.State; }
        }

        #endregion

        class CompletedAsyncResult : IAsyncResult
        {
            GenericXmlSecurityToken token;
            //AsyncCallback callback;
            object state;
            WaitHandle handle;
            object thisLock = new object();

            public CompletedAsyncResult(GenericXmlSecurityToken token, AsyncCallback callback, object state)
            {
                this.token = token;
                this.state = state;
                if (callback != null)
                {
                //    this.callback = callback;
                    callback(this);
                }
            }

            public GenericXmlSecurityToken Token
            {
                get
                {
                    return this.token;
                }
            }

            public object AsyncState
            {
                get { return this.state; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    lock (thisLock)
                    {
                        if (this.handle == null)
                        {
                            this.handle = new ManualResetEvent(true);
                        }
                    }
                    return this.handle;
                }
            }

            public bool CompletedSynchronously
            {
                get { return true; }
            }

            public bool IsCompleted
            {
                get { return true; }
            }
        }
    }
}


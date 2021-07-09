//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.HttpCookieSession
{
    class HttpCookieReplySession : IInputSession
    {
        string id;

        public HttpCookieReplySession()
        {
            this.id = string.Format("ASP.NET_SessionId={0}", Guid.NewGuid().ToString());
        }
        public string Id
        {
            get { return this.id; }
        }
    }
}

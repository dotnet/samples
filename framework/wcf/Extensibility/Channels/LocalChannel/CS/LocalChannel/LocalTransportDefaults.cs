//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{
    static class LocalTransportDefaults
    {
        public readonly static MessageVersion MessageVersionLocal = MessageVersion.Default;
        public const string UriSchemeLocal = "net.local";
        internal const long MaxReceivedMessageSize = 65536;
    }
}

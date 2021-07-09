//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;

namespace Microsoft.Samples.ReliableSecureProfile
{
    static class MakeConnectionConstants
    {
        public const string AnonymousUriTemplate = "http://docs.oasis-open.org/ws-rx/wsmc/200702/anonymous?id=";
        public const string Namespace = "http://docs.oasis-open.org/ws-rx/wsmc/200702";
        public const string Prefix = "wsmc";

        internal static class Configuration
        {
            public const string ClientPollTimeout = "clientPollTimeout";
            public const string ServerPollTimeout = "serverPollTimeout";
        }

        internal static class Defaults
        {
            public static readonly TimeSpan ClientPollTimeout = TimeSpan.FromMinutes(5);
            public const string ClientPollTimeoutString = "00:05:00";
            public static readonly TimeSpan ServerPollTimeout = TimeSpan.FromSeconds(15);
            public const string ServerPollTimeoutString = "00:00:15";

            // note this can be as long as it is becase we only queue this up if there were pending readers
            public static readonly TimeSpan ApplicationMessagePollTimeout = TimeSpan.FromMilliseconds(15);
            public const int MaxFaultSize = 65536;
        }

        internal static class Fault
        {
            public const string Action = "http://docs.oasis-open.org/ws-rx/wsmc/200702/fault";
            public const string MissingSelectionFault = "MissingSelection";
            public const string UnsupportedSelectionFault = "UnsupportedSelection";
        }

        internal static class MakeConnectionMessage
        {
            public const string Action = "http://docs.oasis-open.org/ws-rx/wsmc/200702/MakeConnection";
            public const string Name = "MakeConnection";
            public const string AddressElement = "Address";
            public const string IdentifierElement = "Identifier";
        }

        internal static class MessagePending
        {
            public const string Name = "MessagePending";
            public const string AttibuteName = "pending";
        }
        
        internal static class Policy
        {
            public const string Assertion = "MCSupported";
        }
    }
}

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.HttpCookieSession
{
    static class HttpCookieSessionDefaults
    {
        public static TimeSpan SessionTimeout { get { return TimeSpan.FromMinutes(10); } }
        public const string SessionTimeoutString = "00:10:00";

        // do not exchange the terminate message by default.
        public const bool ExchangeTerminateMessage = false;

        // Name of the resource file.
        public const string ResourceFileName =
            "Microsoft.Samples.HttpCookieSession.Properties.Resources";
    }

    static class HttpCookieConfigurationStrings
    {
        public const string ExchangeTerminateMessageProperty = "exchangeTerminateMessage";
        public const string SessionTimeoutProperty = "sessionTimeout";
        public const string HttpCookieSessionBindingSectionName =
            "system.serviceModel/bindings/httpCookieSessionBinding";
    }

    static class HttpCookiePolicyStrings
    {
        public const string Prefix = "hc";
        public const string Namespace = "http://samples.microsoft.com/wcf/session/policy";
        public const string HttpCookiePolicyElement = "httpCookie";
        public const string ExchangeTerminateAttribute = "exchangeTerminate";
    }
}

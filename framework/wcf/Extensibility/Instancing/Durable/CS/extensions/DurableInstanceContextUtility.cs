using System;

namespace Microsoft.ServiceModel.Samples
{
    static class DurableInstanceContextUtility
    {

        // Constant values related to the custom protocol.
        public const string HeaderName = @"instanceId";
        public const string HeaderNamespace = @"http://samples.microsoft.com/wcf/durableinstancing";
        public const string ContextIdProperty = @"ContextIdProperty";

        // Constant values related to the configuration.
        public const string ContextStoreLocation = @"contextStoreLocation";
        public const string ContextType = @"contextType";
        public const string HttpCookie = @"HttpCookie";
        public const string MessageHeader = @"MessageHeader";
        public const string ContextStoreDirectory = @"\ContextStore";

        // Constant values related to the HttpCookieContextManager.
        public const string HttpCookieKey = @"ContextId=";

        // Constant values related to the Sql server storage manager.
        public const string ConnectionString = @"SqlContextStore";

        // Other constants.
        public const string ResourceFile = @"Microsoft.ServiceModel.Samples.Properties.Resources";
    }
}


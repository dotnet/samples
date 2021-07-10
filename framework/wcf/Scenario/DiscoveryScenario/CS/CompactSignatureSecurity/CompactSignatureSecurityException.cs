//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.Discovery
{

    [Serializable]
    class CompactSignatureSecurityException : Exception
    {
        public CompactSignatureSecurityException(string message) : base(message) {}
        public CompactSignatureSecurityException(string message, Exception innerException) : base(message, innerException) { }
    }
}

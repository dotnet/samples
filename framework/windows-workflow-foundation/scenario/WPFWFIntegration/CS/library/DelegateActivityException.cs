//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.WPFWFIntegration
{

    [Serializable]
    public class DelegateActivityException : Exception
    {
        public DelegateActivityException() { }
        public DelegateActivityException(string message) : base(message) { }
        public DelegateActivityException(string message, Exception inner) : base(message, inner) { }
        protected DelegateActivityException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.ReliableSecureProfile
{
    interface IMakeConnectionListenerSettings
    {
        TimeSpan ServerPollTimeout { get; }
    }
}

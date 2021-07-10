//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Globalization;
using System.Threading;

namespace Microsoft.Samples.Discovery
{

    class SecurityIdGenerator
    {
        string prefix;
        int count;

        public SecurityIdGenerator()
            : this("_d_")
        {
        }

        public SecurityIdGenerator(string prefix)
        {
            this.prefix = prefix;
            this.count = 0;
        }

        public string GenerateId()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", this.prefix, Interlocked.Increment(ref this.count));
        }
    }
}

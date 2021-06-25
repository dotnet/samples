//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.Activities.Data
{
    public class Role
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Code + " - " + Name;
        }
    }
}

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Text;

namespace Microsoft.Samples.ExtendedProtection
{
    public class GetKey : IGetKey
    {
        public string GetKeyFromPasscode(string passCode)
        {
            // Gets a key from the input passcode string
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(passCode));
        }
    }
}

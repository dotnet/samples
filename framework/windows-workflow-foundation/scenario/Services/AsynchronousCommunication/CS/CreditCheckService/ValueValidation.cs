
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Activities;

namespace Microsoft.Samples.CreditCheckService
{

    public sealed class ValueValidation : CodeActivity
    {
        public InArgument<Int32> Value { get; set; }
        public OutArgument<Boolean> Approve { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Approve if the value of the house is less than $1,000,000
            Approve.Set(context, Value.Get(context) < 1000000 ? true : false);
        }
    }
}

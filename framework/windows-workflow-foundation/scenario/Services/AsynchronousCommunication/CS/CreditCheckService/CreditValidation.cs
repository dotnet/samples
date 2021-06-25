
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Activities;

namespace Microsoft.Samples.CreditCheckService
{

    public sealed class CreditValidation : CodeActivity
    {
        public InArgument<Int32> Credit { get; set; }
        public OutArgument<Boolean> Approve { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            // Approve if the credit value of that person is 500 or more
            Approve.Set(context, Credit.Get(context) >= 500 ? true : false);
        }
    }
}

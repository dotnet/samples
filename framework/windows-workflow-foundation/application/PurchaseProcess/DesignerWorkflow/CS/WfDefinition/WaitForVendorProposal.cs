//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Activities;
using System.Globalization;

namespace Microsoft.Samples.WF.PurchaseProcess
{
    
    /// <summary>
    /// This custom activity creates a named bookmark that
    /// must be resumed later by a vendor when 
    /// submitting the proposal
    /// </summary>
    public sealed class WaitForVendorProposal: NativeActivity<double>
    {
        public InArgument<int> VendorId { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            string name = "waitingFor_" + this.VendorId.Get(context).ToString();

            if (VendorId.Get(context) == 0)
            {
                throw new Exception("Vendor identifier is required");
            }

            context.CreateBookmark(name, new BookmarkCallback(OnReadComplete));
        }

        void OnReadComplete(NativeActivityContext context, Bookmark bookmark, object state)
        {          
            double input = Convert.ToDouble(state, new CultureInfo("EN-us"));
            context.SetValue(this.Result, input);
        }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }
    }
}

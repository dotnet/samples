//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.WebClient
{
    public partial class UserControls_Top : System.Web.UI.UserControl
    {
        public string Text
        {
            get { return this.lblPageName.Text; }
            set { this.lblPageName.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
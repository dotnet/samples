//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;

namespace Microsoft.Samples.WebClient
{
    public partial class UserControls_Title : System.Web.UI.UserControl
    {
        public string Text
        {
            get { return this.lblCaption.Text; }
            set { this.lblCaption.Text = value; }
        }
    }
}
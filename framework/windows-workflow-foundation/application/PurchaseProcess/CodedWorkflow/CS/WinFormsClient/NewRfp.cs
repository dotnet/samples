//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using Microsoft.Samples.WF.PurchaseProcess;

namespace Microsoft.Samples.WinFormsClient
{

    public partial class NewRfp : Form
    {
        public PurchaseProcessHost PurchaseProcessHost;

        public NewRfp()
        {
            InitializeComponent();
        }

        // load the form
        private void NewRfp_Load(object sender, EventArgs e)
        {
            foreach (Vendor vendor in VendorRepository.RetrieveAll())
            {
                this.chkVendors.Items.Add(vendor);                                
            }
        }

        // cancel the operation
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        // create a new Rfp and launch the WF instance
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // create the RFP
            RequestForProposal rfp = new RequestForProposal();
            rfp.Title = this.txtTitle.Text;
            rfp.Description = this.txtDescription.Text;
            foreach (Vendor vendor in chkVendors.CheckedItems)
            {
                rfp.InvitedVendors.Add(vendor);
            }

            // create the proposal within the host
            this.PurchaseProcessHost.CreateAndRun(rfp);
                 
            // show final message
            this.Hide();
        }
    }
}

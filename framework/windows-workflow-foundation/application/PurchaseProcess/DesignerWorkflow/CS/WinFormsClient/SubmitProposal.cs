//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Samples.WF.PurchaseProcess;

namespace Microsoft.Samples.WinFormsClient
{

    public partial class SubmitProposal : Form
    {
        public PurchaseProcessHost PurchaseProcessHost;
        public Guid RfpId;
        public int VendorId;     

        public SubmitProposal()
        {
            InitializeComponent();
        }     
        
        // show the rfp and set the UI 
        void ViewRfp_Load(object sender, EventArgs e)
        {
            // retrieve rfp from repository
            RequestForProposal rfp = RfpRepository.Retrieve(this.RfpId);

            // set form caption
            this.Text = string.Format("Submit Proposal (Vendor {0})", this.VendorId);

            // show general info
            this.txtTitle.Text = rfp.Title;
            this.txtDescription.Text = rfp.Description;
            this.txtCreated.Text = rfp.CreationDate.ToString();
                        
            // set UI for vendor
            if (rfp.IsInvited(this.VendorId))
            {
                if (this.PurchaseProcessHost.CanSubmitProposalToInstance(this.RfpId, this.VendorId))
                {
                    this.SetProposalUI("Please submit your proposal.", true);
                }
                else
                {
                    this.SetProposalUI("You have already submited your proposal.", false);                 
                }
            }
            else
            {
                this.SetProposalUI("You are not invited to submit a proposal.", false);
            }
        }

        // save the proposal from the vendor
        void btnSave_Click(object sender, EventArgs e)
        {
            // check again if the vendor can submit the propsal
            if (this.PurchaseProcessHost.CanSubmitProposalToInstance(this.RfpId, this.VendorId))
            {
                // send the proposal to the workflow
                this.PurchaseProcessHost.SubmitVendorProposal(this.RfpId, this.VendorId, Convert.ToDouble(this.txtProposal.Text));
            }

            this.Hide();
        }

        // close this window
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        // set the UI framgment for introducing the proposal
        void SetProposalUI(string message, bool canSubmit)
        {
            this.txtProposal.Enabled = canSubmit;
            this.txtProposal.Text = canSubmit ? "" : "-----";
            this.lblMessage.Text = message;
            this.lblMessage.ForeColor = canSubmit ? Color.Green : Color.Red;
            this.btnSave.Visible = canSubmit;
        }
    }
}

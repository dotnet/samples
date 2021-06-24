//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Samples.WF.PurchaseProcess;

namespace Microsoft.Samples.WinFormsClient
{

    public partial class ViewRfp : Form
    {
        public PurchaseProcessHost PurchaseProcessHost;
        public Guid RfpId;

        public ViewRfp()
        {
            InitializeComponent();
        }

        // show the request for proposals
        void ViewProposal_Load(object sender, EventArgs e)
        {
            // retrieve rfp from the repository
            RequestForProposal rfp = RfpRepository.Retrieve(this.RfpId);

            // general info
            this.txtTitle.Text = rfp.Title;
            this.txtDescription.Text = rfp.Description;
            this.txtCreated.Text = rfp.CreationDate.ToString();            

            // show best proposal and completion date (depending on the status of the rfp)
            if (rfp.IsFinished())
            {
                this.txtFinished.Text = rfp.CompletionDate.ToString();
                if (rfp.BestProposal != null)
                {
                    this.txtBestProposal.Text = string.Format("{0} USD from '{1}' ({2}))", rfp.BestProposal.Value, rfp.BestProposal.Vendor.Name, rfp.BestProposal.Date);
                    this.txtBestProposal.ForeColor = Color.Green;
                }
                else
                {
                    this.txtBestProposal.Text = "No vendor proposals received for this RfP";
                }
            }
            else
            {
                this.txtFinished.Text = "Not finished yet";
                this.txtBestProposal.Text = "Not finished yet";
            }

            // show invited vendors
            foreach (Vendor vendor in rfp.InvitedVendors)
            {
                if (this.txtInvitedVendors.Text.Length > 0)
                {
                    this.txtInvitedVendors.Text += ", ";
                }
                this.txtInvitedVendors.Text += vendor.Name;
            }

            // show received proposals
            this.AddHeaderToList(this.lstReceivedProposals, "Vendor", 150);
            this.AddHeaderToList(this.lstReceivedProposals, "Value", 100);
            this.AddHeaderToList(this.lstReceivedProposals, "Date", 200);
            foreach (var proposal in rfp.VendorProposals.Values)
            {
                ListViewItem item = new ListViewItem(proposal.Vendor.ToString());
                item.SubItems.Add(proposal.Value.ToString());
                item.SubItems.Add(proposal.Date.ToString());
                this.lstReceivedProposals.Items.Add(item);
            }
        }

        // add a header to a listView
        void AddHeaderToList(ListView list, string headerCaption, int headerWidth)
        {
            ColumnHeader header = new ColumnHeader();
            header.Text = headerCaption;
            header.Width = headerWidth;
            list.Columns.Add(header);
        }

        // close this window
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

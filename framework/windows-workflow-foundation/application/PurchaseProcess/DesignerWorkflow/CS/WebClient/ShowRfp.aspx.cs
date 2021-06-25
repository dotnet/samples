//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Text;

namespace Microsoft.Samples.WF.PurchaseProcess.WebClient
{
    public partial class ShowRfp : System.Web.UI.Page
    {
        // retrieve the Rquest for Proposals and show it in the UI
        protected void Page_Load(object sender, EventArgs e)
        {
            // get data from the request   
            Guid instanceId = new Guid(Request["id"]);

            // retrieve the Request for Proposals from the repository
            RequestForProposal rfp = RfpRepository.Retrieve(instanceId);

            // show general info
            this.Title = string.Format("Showing '{0}'", rfp.Title);
            this.lblTitle.Text = rfp.Title;
            this.lblDescription.Text = rfp.Description;
            this.lblCreated.Text = rfp.CreationDate.ToString();

            // show best offer and completion date
            if (rfp.IsFinished())
            {
                this.lblEndDate.Text = rfp.CompletionDate.ToString();
                if (rfp.BestProposal != null)
                {
                    this.pnlBestProposal.Visible = true;
                    this.lblBestProposalValue.Text = rfp.BestProposal.Value.ToString();
                    this.lblBestProposalVendor.Text = rfp.BestProposal.Vendor.Name;
                }
            }
            else
            {
                this.lblEndDate.Text = "Not finished yet";
            }

            // show invited vendors
            foreach (Vendor vendor in rfp.InvitedVendors)
            {
                if (this.lblInvitedVendors.Text.Length > 0)
                {
                    this.lblInvitedVendors.Text += ", ";
                }
                this.lblInvitedVendors.Text += vendor.Name;
            }

            // show received proposals in the list
            StringBuilder buffer = new StringBuilder();
            foreach (var proposal in rfp.VendorProposals.Values)
            {
                buffer.Append("<tr>");
                buffer.Append(string.Format("<td>{0}</td>", proposal.Vendor.Name));
                buffer.Append(string.Format("<td>{0}</td>", proposal.Value.ToString()));
                buffer.Append(string.Format("<td>{0}</td>", proposal.Date.ToString()));
                buffer.Append("</tr>");
            }
            this.litVendorProposalsTableRows.Text = buffer.ToString();
        }

        // get instance of the host
        PurchaseProcessHost GetHost()
        {
            if (Application["purchasProcesHost"] == null)
            {
                Application.Add("purchasProcesHost", new PurchaseProcessHost());
            }

            return (PurchaseProcessHost)Application["purchasProcesHost"];
        }
    }
}
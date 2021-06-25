//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.Samples.WF.PurchaseProcess.WebClient
{
    public partial class GetVendorProposal : System.Web.UI.Page
    {
        Guid instanceId;
        int vendorId;

        // retrieve the Rquest for Proposals and show it in the UI
        void Page_Load(object sender, EventArgs e)
        {
            // get data from the request   
            instanceId = new Guid(Request["id"]);
            vendorId = int.Parse(Request["vendorId"]);

            if (!IsPostBack)
            {
                // retrieve the Request for Proposals from the repository
                RequestForProposal rfp = RfpRepository.Retrieve(instanceId);

                // show general info
                this.Title = string.Format("Submit Proposal (Vendor {0})", this.vendorId);
                this.lblTitle.Text = rfp.Title;
                this.lblDescription.Text = rfp.Description;
                this.lblCreated.Text = rfp.CreationDate.ToString();

                if (rfp.IsInvited(vendorId))
                {
                    if (this.GetHost().CanSubmitProposalToInstance(this.instanceId, this.vendorId))
                    {
                        this.pnlVendorOffer.Visible = true;
                        this.pnlSubmited.Visible = false;
                        this.pnlNotInvited.Visible = false;
                    }
                    else
                    {
                        this.pnlSubmited.Visible = true;
                        this.pnlVendorOffer.Visible = false;
                        this.pnlNotInvited.Visible = false;
                    }
                }
                else
                {
                    this.pnlNotInvited.Visible = true;
                    this.pnlSubmited.Visible = false;
                    this.pnlVendorOffer.Visible = false;
                }
            }
        }

        // event fired when the Save button is clicked
        protected void OnInputProposal(object sender, EventArgs e)
        {
            // check again if the vendor can submit the propsal
            if (this.GetHost().CanSubmitProposalToInstance(this.instanceId, this.vendorId))
            {
                // send the proposal to the workflow
                this.GetHost().SubmitVendorProposal(this.instanceId, this.vendorId, Convert.ToDouble(this.txtProposal.Text));

                // set ui to thank the vendor for submiting his proposal
                this.pnlThanks.Visible = true;
                this.pnlGoBack.Visible = true;
                this.pnlVendorOffer.Visible = false;
            }
            else
            {
                // set ui to communicate the vendor that he can't submit offer
                this.pnlVendorOffer.Visible = false;
                this.pnlGoBack.Visible = true;
            }
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
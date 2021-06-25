//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Web.UI.WebControls;

namespace Microsoft.Samples.WF.PurchaseProcess.WebClient
{
    public partial class CreateRfp : System.Web.UI.Page
    {
        // set the UI
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // populate the list of vendors
                foreach (Vendor vendor in VendorRepository.RetrieveAll())
                {
                    chkVendorsList.Items.Add(new ListItem(vendor.Name, vendor.Id.ToString()));
                }
            }
        }

        // create the Request for Proposal and start the workflow
        protected void CreateAndSubmitRfp(object sender, EventArgs e)
        {
            // collect info from the UI and create the RFP        
            RequestForProposal rfp = new RequestForProposal()
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text
            };

            // add invited vendors
            for (int counter = 0; counter < chkVendorsList.Items.Count; counter++)
            {
                if (chkVendorsList.Items[counter].Selected)
                {
                    int vendorId = int.Parse(chkVendorsList.Items[counter].Value);
                    rfp.InvitedVendors.Add(VendorRepository.Retrieve(vendorId));
                }
            }

            // get the instance of the host and create and run the new workflow
            PurchaseProcessHost host = GetHost();
            host.CreateAndRun(rfp);

            // return to the main simulation page
            Response.Redirect("Default.aspx");
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
//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Text;

namespace Microsoft.Samples.WF.PurchaseProcess.WebClient
{
    public partial class _Default : System.Web.UI.Page
    {
        // show the list of active instances
        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshLists();
        }

        // refresh the list of Rfps
        void RefreshLists()
        {
            //-------------------------------------------
            // List of active rfps
            //-------------------------------------------
            StringBuilder buffer = new StringBuilder();

            // create headers
            buffer.Append("<table>");
            buffer.Append("<tr class=\"Header\">");
            buffer.Append("<td>ID</td>");
            buffer.Append("<td>Title</td>");
            buffer.Append("<td>Created</td>");
            buffer.Append("<td>Invited Vendors</td>");
            buffer.Append("<td>View as...</td>");
            buffer.Append("</tr>");

            // show rfps in the list
            foreach (var rfp in RfpRepository.RetrieveActive())
            {
                buffer.Append("<tr>");
                buffer.Append(string.Format("<td>{0}</td>", rfp.ID.ToString()));
                buffer.Append(string.Format("<td>{0}</td>", rfp.Title));
                buffer.Append(string.Format("<td>{0}</td>", rfp.CreationDate.ToString()));
                buffer.Append(string.Format("<td>{0}</td>", rfp.GetInvitedVendorsStatus(true)));
                buffer.Append(string.Format("<td>{0}</td>", RenderParticipantsCombo(rfp.ID.ToString())));
                buffer.Append("</tr>");
            }
            buffer.Append("</table>");
            this.litActive.Text = buffer.ToString();


            //-------------------------------------------
            // List of finished rfps
            //-------------------------------------------
            buffer = new StringBuilder();

            // create headers
            buffer.Append("<table>");
            buffer.Append("<tr class=\"Header\">");
            buffer.Append("<td>ID</td>");
            buffer.Append("<td>Title</td>");
            buffer.Append("<td>Created</td>");
            buffer.Append("<td>Finished</td>");
            buffer.Append("<td>Invited Vendors</td>");
            buffer.Append("<td>Winner</td>");
            buffer.Append("<td>&nbsp;</td>");
            buffer.Append("</tr>");

            // show rfps in the list
            foreach (var rfp in RfpRepository.RetrieveFinished())
            {
                buffer.Append("<tr>");
                buffer.Append(string.Format("<td>{0}</td>", rfp.ID.ToString()));
                buffer.Append(string.Format("<td>{0}</td>", rfp.Title));
                buffer.Append(string.Format("<td>{0}</td>", rfp.CreationDate.ToString()));
                buffer.Append(string.Format("<td>{0}</td>", rfp.CompletionDate.ToString()));
                buffer.Append(string.Format("<td>{0}</td>", rfp.GetInvitedVendorsStatus()));
                buffer.Append(string.Format("<td>{0} ({1} USD)</td>", rfp.BestProposal.Vendor.Name, rfp.BestProposal.Value.ToString()));
                buffer.Append(string.Format("<td><a href=\"ShowRfp.aspx?id={0}\">View as Requestor</a></td>", rfp.ID.ToString()));
                buffer.Append("</tr>");
            }
            buffer.Append("</table>");
            this.litFinished.Text = buffer.ToString();
        }

        // show the combo for selecting a participant (displayed in each table row)
        string RenderParticipantsCombo(string rfpId)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("<select id=\"sel_" + rfpId + "\">");
            foreach (Vendor vendor in VendorRepository.RetrieveAll())
            {
                buffer.Append(string.Format("<option value=\"{0}\">{1}</option>", vendor.Id, vendor.Name));
            }
            buffer.Append(string.Format("<option value=\"{0}\">{1}</option>", "", "Requestor"));
            buffer.Append("</select>");
            buffer.Append(string.Format("<input type=\"button\" value=\"View\" onclick=\"showProposal('{0}')\"/>", rfpId));
            return buffer.ToString();
        }
    }
}
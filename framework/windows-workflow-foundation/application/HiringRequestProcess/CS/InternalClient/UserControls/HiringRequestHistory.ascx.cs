//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System.Collections.Generic;
using System.Web.UI;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.WebClient
{
    public partial class HiringRequestHistory : System.Web.UI.UserControl
    {
        public string HiringRequestId { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {            
            IList<RequestHistoryRecord> history = RequestHistoryRepository.GetRequestHistory(this.HiringRequestId);

            writer.WriteLine("<table rules=\"all\" style=\"border-collapse:collapse;\" cellpadding=\"3\">");
            writer.WriteLine("<tr class=\"inboxTitle\">");
            writer.WriteLine("<td>Date</td>");
            writer.WriteLine("<td>Source State</td>");
            writer.WriteLine("<td>Action</td>");
            writer.WriteLine("<td>Employee</td>");
            writer.WriteLine("<td>Comments</td>");
            writer.WriteLine("</tr>");

            foreach (RequestHistoryRecord action in history)
            {
                writer.WriteLine("<tr>");
                writer.WriteLine(string.Format("<td>{0}</td>", action.Date.ToString()));
                writer.WriteLine(string.Format("<td>{0}</td>", action.SourceState));
                writer.WriteLine(string.Format("<td>{0}</td>", action.Action));
                writer.WriteLine(string.Format("<td>{0}</td>", action.EmployeeName));
                writer.WriteLine(string.Format("<td>{0}</td>", action.Comment));
                writer.WriteLine("</tr>");
            }
            writer.WriteLine("</table>");
        }
    }
}
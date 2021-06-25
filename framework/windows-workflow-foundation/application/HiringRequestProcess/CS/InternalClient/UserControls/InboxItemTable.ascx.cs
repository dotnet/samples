//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System.Collections.Generic;   
using System.Web.UI;
using Contoso.InboxService;

namespace Microsoft.Samples.WebClient
{
    public partial class UserControls_InboxItemTable : System.Web.UI.UserControl
    {
        public IList<InboxItem> InboxItems { get; set; }

        public string ActionText { get; set; }

        public string ActionUri { get; set; }

        public string HeaderStyle { get; set; }

        public bool IncludeStateInUri { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine("<table rules=\"all\" style=\"border-collapse:collapse;\">");
            writer.WriteLine("<tr class=\"{0}\">", HeaderStyle);
            writer.WriteLine("<td width=\"30px\">&nbsp;</td>");
            writer.WriteLine("<td width=\"315px\">Id</td>");
            writer.WriteLine("<td width=\"450px\">Title</td>");
            writer.WriteLine("<td width=\"200px\">State</td>");
            writer.WriteLine("<td width=\"100px\">&nbsp;</td>");
            writer.WriteLine("</tr>");

            bool alternate = true;
            if (this.InboxItems != null)
            {
                foreach (InboxItem item in this.InboxItems)
                {
                    writer.WriteLine("<tr bgcolor=\"{0}\">", (alternate ? "whitesmoke" : "white"));
                    writer.WriteLine("<td align=\"center\" width=\"30px\"><img src=\"../images/foundation_icon.gif\"></td>");
                    writer.WriteLine(string.Format("<td width=\"315px\">{0}</td>", item.RequestId));
                    writer.WriteLine(string.Format("<td width=\"450px\">{0}</td>", item.Title));
                    writer.WriteLine(string.Format("<td width=\"200px\">{0}</td>", item.State));
                    if (this.IncludeStateInUri)
                    {
                        writer.WriteLine(string.Format("<td><a href=\"{0}?id={1}&state={2}\">{3}</a></td>", this.ActionUri, item.RequestId, item.State, this.ActionText));
                    }
                    else
                    {
                        writer.WriteLine(string.Format("<td><a href=\"{0}?id={1}\">{2}</a></td>", this.ActionUri, item.RequestId, this.ActionText));
                    }
                    writer.WriteLine("</tr>");
                    alternate = !alternate;

                }
            }
            writer.WriteLine("</table>");
        }
    }
}
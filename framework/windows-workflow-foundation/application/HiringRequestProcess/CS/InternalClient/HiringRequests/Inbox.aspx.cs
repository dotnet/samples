//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Contoso.InboxService;
using Contoso.OrgService;

namespace Microsoft.Samples.WebClient
{
    public partial class Inbox : System.Web.UI.Page
    {
        Employee currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.currentUser = (Employee)Session["user"];

            if (!IsPostBack)
            {
                this.BindEmployees();
                this.ConfigureInboxTables();
            }
        }

        protected void ChangeUser(object sender, EventArgs e)
        {
            OrgServiceClient client = new OrgServiceClient();
            Session["user"] = client.GetEmployee(this.ddlConnectAs.SelectedValue);
            this.currentUser = (Employee)Session["user"];
            client.Close();
            this.ConfigureInboxTables();
        }

        void BindEmployees()
        {
            OrgServiceClient client = new OrgServiceClient();

            IList<Employee> employees = client.GetAllEmployees().OrderBy(e => e.Name).ToList();

            foreach (Employee employee in employees)
            {
                this.ddlConnectAs.Items.Add(new ListItem(string.Format("{0} - {1} ({2} - {3})", employee.Id, employee.Name, employee.Position.Name, employee.Department.Name), employee.Id));
            }

            if (currentUser != null)
            {
                this.ddlConnectAs.Items.FindByValue(currentUser.Id).Selected = true;
            }
            else
            {
                Session["user"] = client.GetEmployee(ddlConnectAs.SelectedItem.Value);
                this.currentUser = (Employee)Session["user"];
            }

            client.Close();
        }

        void ConfigureInboxTables()
        {
            if (this.currentUser != null)
            {
                // create the client for the inbox service
                InboxServiceClient client = new InboxServiceClient();

                // 1. Inbox
                this.inboxParticipate.InboxItems = client.GetRequestsFor(currentUser.Id);

                // 2. Started by Me
                this.activeStartedByMe.InboxItems = client.GetRequestsStartedBy(currentUser.Id);

                // 3. Completed started by Me
                this.archivedStartedByMe.InboxItems = client.GetArchivedRequestsStartedBy(currentUser.Id);

                // close the service client
                client.Close();
            }
        }
    }
}
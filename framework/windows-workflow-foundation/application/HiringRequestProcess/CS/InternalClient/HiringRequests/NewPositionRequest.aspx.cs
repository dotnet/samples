//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using Contoso.HiringRequestService;
using Contoso.OrgService;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.WebClient
{
    public partial class NewPositionRequest : System.Web.UI.Page
    {
        Employee currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            currentUser = (Employee)Session["user"];

            HiringRequestInfo info = new HiringRequestInfo { RequesterId = currentUser.Id };
            this.hiringRequestDetails1.HiringRequestInfo = info;
            this.hiringRequestDetails1.ShowCommentsField = false;
            this.hiringRequestDetails1.Editable = true;
        }

        protected void GoBackToInboxClick(object sender, EventArgs e)
        {
            Response.Redirect("inbox.aspx");
        }

        protected void CreateClick(object sender, EventArgs e)
        {
            // create the hiringRequest info and populate it with data from the UI
            HiringRequestInfo requestInfo = new HiringRequestInfo()
            {
                Id = Guid.NewGuid(),
                RequesterId = currentUser.Id,
                CreationDate = DateTime.Now,
                DepartmentId = this.hiringRequestDetails1.DepartmentId,
                PositionId = this.hiringRequestDetails1.PositionId,
                Description = this.hiringRequestDetails1.Description,
                Title = this.hiringRequestDetails1.CreateRequestTitle(this.currentUser)
            };

            // start the hiring request
            HiringRequestServiceClient client = new HiringRequestServiceClient();
            client.StartProcess(requestInfo);
            client.Close();

            // update ui
            this.btnCancel.Text = "Request created, click here to go back to the Inbox";
            this.btnCancel.Width = new System.Web.UI.WebControls.Unit(400, System.Web.UI.WebControls.UnitType.Pixel);
            this.btnCreate.Visible = false;
        }
    }
}
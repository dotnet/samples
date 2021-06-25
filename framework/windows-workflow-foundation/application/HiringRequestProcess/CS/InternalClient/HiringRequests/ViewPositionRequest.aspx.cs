//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.ServiceModel;
using System.ServiceModel.Activities;
using Contoso.OrgService;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.WebClient
{
    public partial class ViewPositionRequest : System.Web.UI.Page
    {
        string requestId;
        HiringRequestInfo info;
        Employee currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // initialize data
            requestId = Request["id"];
            this.info = HiringRequestRepository.Load(requestId);
            this.currentUser = (Employee)Session["user"];

            // show request info
            this.hiringRequestDetails1.HiringRequestInfo = info;
            this.hiringRequestDetails1.Editable = false;
            this.hiringRequestHistory1.HiringRequestId = requestId;

            // only show the cancellation button to the requester
            if (!IsPostBack)
            {
                this.btnCancel.Visible = (!info.IsCompleted && info.RequesterId.Equals(currentUser.Id));
            }
        }

        protected void OnCancelClick(object sender, EventArgs e)
        {
            // perform cancellation
            WorkflowControlClient controlClient = new WorkflowControlClient(new BasicHttpBinding(), new EndpointAddress(new Uri("http://127.0.0.1/hiringProcess")));
            controlClient.Cancel(info.WorkflowInstanceId);

            // set UI
            this.btnCancel.Visible = false;
            this.btnBack.Width = new System.Web.UI.WebControls.Unit(400, System.Web.UI.WebControls.UnitType.Pixel);
            this.btnBack.Text = "Cancellation sent, click here to go back to the Inbox";
        }

        protected void OnGoBackToInboxClick(object sender, EventArgs e)
        {
            Response.Redirect("Inbox.aspx");
        }
    }
}
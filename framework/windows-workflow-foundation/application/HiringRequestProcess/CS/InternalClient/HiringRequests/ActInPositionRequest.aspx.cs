//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.Web.UI.WebControls;
using Contoso.HiringRequestService;
using Contoso.OrgService;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.WebClient
{
    public partial class ActInPositionRequest : System.Web.UI.Page
    {
        string requestId;
        string state;
        HiringRequestInfo info;
        Employee currentUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            // get information from state
            requestId = Request["id"];
            state = Request["state"];

            // get the user 
            currentUser = (Employee)Session["user"];

            // configure the control to show hiring request info        
            info = HiringRequestRepository.Load(requestId);

            // set UI
            this.SetControlsVisibility();

            this.hiringRequestDetails1.HiringRequestInfo = info;
            this.hiringRequestDetails1.ShowCommentsField = true;
            this.hiringRequestHistory1.HiringRequestId = requestId;
        }

        // Set visibility of UI buttons + form data
        void SetControlsVisibility()
        {
            switch (state)
            {
                case HiringRequestProcessStates.WaitingForManagerApproval:
                    this.hiringRequestDetails1.Editable = false;
                    this.btnAccept.Visible = true;
                    this.btnReject.Visible = true;
                    this.btnReview.Visible = true;
                    break;
                case HiringRequestProcessStates.WaitingForDepartmentOwnerApproval:
                    this.hiringRequestDetails1.Editable = false;
                    this.btnAccept.Visible = true;
                    this.btnReject.Visible = true;
                    this.btnReview.Visible = false;
                    break;
                case HiringRequestProcessStates.InReview:
                    this.hiringRequestDetails1.Editable = true;
                    this.btnAccept.Visible = true;
                    this.btnReject.Visible = false;
                    this.btnReview.Visible = false;
                    break;
                case HiringRequestProcessStates.WaitingForHrManagersOrCeoApproval:
                    this.hiringRequestDetails1.Editable = false;
                    this.btnAccept.Visible = true;
                    this.btnReject.Visible = true;
                    this.btnReview.Visible = false;
                    break;
            }

            // only show the cancellation button to the requester
            this.btnCancel.Visible = info.RequesterId.Equals(currentUser.Id);
        }

        // Handler of click on back button
        protected void OnBackClick(object sender, EventArgs e)
        {
            Response.Redirect("Inbox.aspx");
        }

        protected void OnCancelClick(object sender, EventArgs e)
        {
            WorkflowControlClient controlClient = new WorkflowControlClient(new BasicHttpBinding(), new EndpointAddress(new Uri("http://127.0.0.1/hiringProcess")));
            controlClient.Cancel(info.WorkflowInstanceId);

            // hide all button        
            this.HideAllButtons("Cancellation sent, click here to go back to the Inbox");
        }

        // Handler of click on an action button (Accept, Reject, Review)
        protected void OnActionClick(object sender, EventArgs e)
        {
            // get data from the clicked buttton
            string action = ((Button)sender).Text;

            // perform the action according with the current state
            HiringRequestServiceClient client = new HiringRequestServiceClient();
            switch (state)
            {
                case HiringRequestProcessStates.WaitingForManagerApproval:
                    client.ReceiveManagerApproval(this.requestId, action, this.hiringRequestDetails1.Comments, currentUser.Id);
                    break;
                case HiringRequestProcessStates.WaitingForDepartmentOwnerApproval:
                    client.ReceiveDeptOwnerApproval(this.requestId, action, this.hiringRequestDetails1.Comments, currentUser.Id);
                    break;
                case HiringRequestProcessStates.InReview:
                    client.CompleteReview(this.requestId, this.hiringRequestDetails1.PositionId, this.hiringRequestDetails1.DepartmentId, this.hiringRequestDetails1.Description, this.hiringRequestDetails1.Comments, this.hiringRequestDetails1.CreateRequestTitle(currentUser), currentUser.Id);
                    break;
                case HiringRequestProcessStates.WaitingForHrManagersOrCeoApproval:
                    client.ReceiveHrManagerApproval(this.requestId, action, this.hiringRequestDetails1.Comments, currentUser.Id);
                    break;
            }

            // close client
            client.Close();

            // hide all button
            this.HideAllButtons("Message sent, click here to go back to the Inbox");
        }

        void HideAllButtons(string message)
        {
            this.btnAccept.Visible = false;
            this.btnReject.Visible = false;
            this.btnReview.Visible = false;
            this.btnCancel.Visible = false;
            this.btnBack.Text = message;
            this.btnBack.Width = new System.Web.UI.WebControls.Unit(400, System.Web.UI.WebControls.UnitType.Pixel);            
        }
    }
}
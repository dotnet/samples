<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="UserControls/Title.ascx" %>
<%@ Import Namespace="Microsoft.Samples.ContosoHR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">    
    JobPosting jobPosting;
    
    // show data when the page is loaded
    void Page_Load(object sender, EventArgs e)
    {
        // load the job posting
        jobPosting = JobPostingRepository.Load(Request["id"]);
        
        // show the job posting
        if (!IsPostBack)
        {                        
            this.lblId.Text = jobPosting.Id.ToString();
            this.lblDate.Text = jobPosting.CreationDate.ToString();
            this.lblTitle.Text = jobPosting.Title;
            this.lblDescription.Text = jobPosting.Description;
        }
    }

    // update data when save button is clicked
    void OnSave(object sender, EventArgs e)
    {
        using (Contoso.ResumeService.JobPostingServiceClient client = new Contoso.ResumeService.JobPostingServiceClient())
        {
            // create the resumee
            JobPostingResume resumee = new JobPostingResume()
            {
                JobPosting = this.jobPosting,
                CandidateMail = this.txtMail.Text,
                CandidateName = this.txtName.Text,
                ResumeeText = this.txtResumee.Text,
            };

            // send it
            client.ReceiveResume(resumee);

            // set UI visibility
            pnlThanks.Visible = true;
            pnlResumeeInfo.Visible = false;
        }
    }

    // go back if back button is clicked
    void OnGoBack(object sender, EventArgs e)
    {
        Response.Redirect("List.aspx");
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="css/Stylesheet1.css" rel="stylesheet" type="text/css" />
    <title>Job Posting Details</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="List of Resumees" />
        <br />
        <contoso:Title ID="title1" runat="server" Text="Job Posting Details" />            
        <table cellpadding="4" cellspacing="2">
            <tr>
                <td class="fieldName">Id</td>
                <td><asp:Label ID="lblId" runat="server"/></td>
            </tr>            
            <tr>
                <td class="fieldName">Date</td>
                <td><asp:Label ID="lblDate" runat="server"/></td>
            </tr>            
            <tr>
                <td class="fieldName">Title</td>
                <td><asp:Label ID="lblTitle" runat="server"/></td>
            </tr>
            <tr>
                <td class="fieldName">Description</td>
                <td><asp:Label ID="lblDescription" runat="server"/></td>
            </tr>
        </table>
        
        <br />
        <br />
        <asp:Panel ID="pnlResumeeInfo" runat="server">
            <contoso:Title ID="title2" runat="server" Text="Apply Now" />            
            <table cellpadding="4" cellspacing="2">
                <tr>
                    <td class="fieldName">Name</td>
                    <td><asp:TextBox ID="txtName" runat="server"/></td>
                </tr>            
                <tr>
                    <td class="fieldName">eMail Address</td>
                    <td><asp:TextBox ID="txtMail" runat="server"/></td>
                </tr>            
                <tr>
                    <td class="fieldName">Resumee</td>
                    <td><asp:TextBox ID="txtResumee" runat="server" TextMode="MultiLine" Rows="10" Columns="80"/></td>
                </tr>            
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button id="Button1" runat="server" Text="Apply for this Position!" OnClick="OnSave" />
                        <asp:Button id="Button2" runat="server" Text="Go Back to the List" OnClick="OnGoBack" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        
        <asp:Panel ID="pnlThanks" runat="server" Visible="false">
            <contoso:Title ID="title3" runat="server" Text="Thanks for Applying!" />            
            <br />
            Thanks for applying to this position, our HR representatives will contact you soon
            <br />
            <br />
            <asp:Button id="Button3" runat="server" Text="Go Back to the List" OnClick="OnGoBack" />
        </asp:Panel>        
    </div>
    </form>
</body>
</html>

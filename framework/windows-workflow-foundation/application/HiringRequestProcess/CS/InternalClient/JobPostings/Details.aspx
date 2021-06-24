<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>
<%@ Import Namespace="Microsoft.Samples.ContosoHR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    // show data when the page is loaded
    void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string id = Request["id"];
            JobPosting jobPosting = JobPostingRepository.Load(id);
            this.lblId.Text = id;
            this.lblDate.Text = jobPosting.CreationDate.ToString();
            this.txtTitle.Text = jobPosting.Title;
            this.txtDescription.Text = jobPosting.Description;
        }
    }

    // update data when save button is clicked
    void OnSave(object sender, EventArgs e)
    {
        using (Contoso.ResumeService.JobPostingServiceClient client = new Contoso.ResumeService.JobPostingServiceClient())
        {
            client.ReceiveJobPostingData(
                    Request["id"],
                    this.txtTitle.Text,
                    this.txtDescription.Text,
                    int.Parse(this.txtTimeOut.Text));
            Response.Redirect("List.aspx");
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
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />
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
                <td><asp:TextBox ID="txtTitle" runat="server"/></td>
            </tr>
            <tr>
                <td class="fieldName">Description</td>
                <td><asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" Columns="60"/></td>
            </tr>
            <tr>
                <td class="fieldName">TimeOut</td>
                <td><asp:TextBox ID="txtTimeOut" runat="server" Columns="5" Text="10" /> minutes</td>
            </tr>                                                
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button id="btnSave" runat="server" Text="Save" OnClick="OnSave" />
                    <asp:Button id="btnCancel" runat="server" Text="Go Back to List" OnClick="OnGoBack" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>

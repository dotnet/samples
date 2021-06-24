<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.Data.SqlClient"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    
    void Page_Load(object sender, EventArgs e)
    {
        using (Contoso.ResumeService.JobPostingServiceClient client = new Contoso.ResumeService.JobPostingServiceClient())
        {
            client.StopProcess(Request["id"]);
        }
        Response.Redirect("List.aspx");
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />
    <title>Job Postings List</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    </form>
</body>
</html>

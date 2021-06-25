<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="UserControls/Title.ascx" %>
<%@ Register TagPrefix="contoso" TagName="InboxItemTable" Src="UserControls/InboxItemTable.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Hiring Request Web Client</title>
    <link href="css/Stylesheet1.css" rel="stylesheet" type="text/css" />
</head>
<body>    
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="Web Client" />                       
        <br />
        <br />
        <br />
        <a href="HiringRequests/Inbox.aspx">Click here to start the application when all the services are started</a>        
    </div>
    </form>
</body>
</html>

<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Inbox.aspx.cs" Inherits="Microsoft.Samples.WebClient.Inbox" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>
<%@ Register TagPrefix="contoso" TagName="InboxItemTable" Src="../UserControls/InboxItemTable.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Inbox</title>
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />
</head>
<body>    
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="Inbox" />        
        <!-- Menu -->
        <table>
            <tr>
                <td><a href="NewPositionRequest.aspx">Request New Position</a></td>
                <td> | <a href="Inbox.aspx"> Refresh</a></td>
                <td> | <a href="../JobPostings/List.aspx"> Job Postings</a></td>
                <td> | Connect as <asp:DropDownList ID="ddlConnectAs" runat="server" OnSelectedIndexChanged="ChangeUser" AutoPostBack="true"/></td>
            </tr>
        </table>
        <br />
        
        <!-- Inbox -->                
        <br />
        <contoso:title id="title1" runat="server" Text="Inbox" />
        <contoso:InboxItemTable ID="inboxParticipate" runat="server" IncludeStateInUri="true" ActionText="Act" ActionUri="ActInPositionRequest.aspx" HeaderStyle="inboxTitle" />
        
        <!-- Started by me (ongoing) -->
        <br />
        <contoso:title id="title2" runat="server" Text="Active Requests Started by Me" />
        <contoso:InboxItemTable ID="activeStartedByMe" runat="server" IncludeStateInUri="false" ActionText="View" ActionUri="ViewPositionRequest.aspx" HeaderStyle="inboxTitleStartedByMe"/>
        
        <!-- Started by me (completed) -->    
        <br />
        <contoso:title id="title3" runat="server" Text="Completed Requests Started by Me" />
        <contoso:InboxItemTable ID="archivedStartedByMe" runat="server" IncludeStateInUri="false" ActionText="View" ActionUri="ViewPositionRequest.aspx" HeaderStyle="inboxTitleFinished" />
    </div>
    </form>
</body>
</html>

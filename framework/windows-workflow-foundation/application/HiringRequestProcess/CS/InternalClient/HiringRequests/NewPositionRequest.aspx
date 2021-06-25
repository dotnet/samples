<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewPositionRequest.aspx.cs" Inherits="Microsoft.Samples.WebClient.NewPositionRequest" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>
<%@ Register TagPrefix="contoso" TagName="HiringRequestDetails" Src="../UserControls/HiringRequestDetails.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Create a New Position Request</title>
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="New position request" />
        <br />
        <!-- Request data -->
        <contoso:Title ID="title1" runat="server" Text="Position information" />
        <contoso:HiringRequestDetails ID="hiringRequestDetails1" runat="server" />
        
        <!-- Actions (Create, Cancel) -->
        <br />
        <table cellpadding="4" cellspacing="2">            
            <tr>
                <td>                    
                    <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="CreateClick"  CssClass="actionButtonOk"/>
                    <asp:Button ID="btnCancel" runat="server" Text="<< Back To Inbox" OnClick="GoBackToInboxClick" CssClass="actionButton"/>                       
                </td>
            </tr>             
        </table>        
        <br />           
    </div>
    </form>
</body>
</html>

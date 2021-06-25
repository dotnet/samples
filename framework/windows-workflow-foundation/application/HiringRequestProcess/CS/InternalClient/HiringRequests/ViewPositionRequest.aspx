<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewPositionRequest.aspx.cs" Inherits="Microsoft.Samples.WebClient.ViewPositionRequest" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>
<%@ Register TagPrefix="contoso" TagName="HiringRequestDetails" Src="../UserControls/HiringRequestDetails.ascx" %>
<%@ Register TagPrefix="contoso" TagName="HiringRequestHistory" Src="../UserControls/HiringRequestHistory.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>View position</title>
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="Participate in position request" />
        
        <!-- Request data -->
        <br />        
        <contoso:Title ID="title1" runat="server" Text="Position information" />            
        <contoso:HiringRequestDetails ID="hiringRequestDetails1" runat="server" />

        <!-- Go Back-->
        <br />           
        <table cellpadding="4" cellspacing="2">
            <tr>                
                <td>         
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="OnCancelClick" CssClass="actionButtonErr"/>                       
                    <asp:Button ID="btnBack" runat="server" Text="<< Back to Inbox" OnClick="OnGoBackToInboxClick" CssClass="actionButton"/>
                </td>
            </tr>                        
        </table>       
        <br />                
        <br />
        <br />
        <br />
        <!-- History  -->
        <contoso:Title ID="title2" runat="server" Text="Position request history" />            
        <contoso:HiringRequestHistory ID="hiringRequestHistory1" runat="server" />        
    </div>
    </form>
</body>
</html>

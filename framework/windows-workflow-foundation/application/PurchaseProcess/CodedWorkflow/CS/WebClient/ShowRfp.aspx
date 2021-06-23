<!-- Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowRfp.aspx.cs" Inherits="Microsoft.Samples.WF.PurchaseProcess.WebClient.ShowRfp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Windows Workflow Foundation 4.0 SDK</title>
    <link rel="Stylesheet" type="text/css" href="css/StyleSheet.css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Purchase Process Sample - Best Vendor Selection</h1>        
        This screen is used by the employees of Company X for viewing the status of an RFP.
        <hr />
        
        <h2>RPF Details</h2>
        <table>
            <tr>
                <td>Title</td>
                <td><asp:Label ID="lblTitle" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>
            <tr>
                <td>Description</td>
                <td><asp:Label ID="lblDescription" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>
            <tr>
                <td>Invited Vendors</td>
                <td><asp:Label ID="lblInvitedVendors" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>                        
            <tr>
                <td>Creation Date</td>
                <td><asp:Label ID="lblCreated" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>                        
            <tr>
                <td>Time Out</td>
                <td><asp:Label ID="lblTimeOut" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>            
            <tr>
                <td>End Date</td>
                <td><asp:Label ID="lblEndDate" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>                        
        </table>
        
        <asp:Panel ID="pnlBestProposal" runat="server" Visible="false">
            <br />
            <h2>Best Vendor Proposal</h2>
            <asp:Label ID="lblBestProposalValue" runat="server" /> USD from <asp:Label ID="lblBestProposalVendor" runat="server" />
            <br />
        </asp:Panel>

        <h2>Vendor Proposals</h2>        
        <table>
            <tr class="header">
                <td>Vendor</td>
                <td>Value</td>
                <td>Date</td>
            </tr>
            <asp:Literal id="litVendorProposalsTableRows" runat="server" />
        </table>
        
        <br />
        <input type="button" value="Go Back" onclick="history.back();" /> 
        
    </div>
    </form>
</body>
</html>

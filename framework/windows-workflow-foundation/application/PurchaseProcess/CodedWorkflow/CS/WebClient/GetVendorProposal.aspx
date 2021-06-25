<!-- Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetVendorProposal.aspx.cs" Inherits="Microsoft.Samples.WF.PurchaseProcess.WebClient.GetVendorProposal" %>

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
        This screen is used by the vendors for submiting proposals to the RFP.
        <hr />
    
        <h2>Introduce your Offer</h2>
        <h3>RFP Details</h3>
        <table id="formTable">
            <tr>
                <td>Title</td>
                <td><asp:Label ID="lblTitle" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>
            <tr>
                <td>Description</td>
                <td><asp:Label ID="lblDescription" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>
            <tr>
                <td>Created</td>
                <td><asp:Label ID="lblCreated" runat="server" Font-Names="Calibri, Arial"/></td>
            </tr>            
        </table>
        <br />
        
        <asp:Panel ID="pnlVendorOffer" runat="server" Visible="false">
            <h3>Introduce your Proposal</h3>
            <table>
                <tr>
                    <td>Value</td>
                    <td>
                        <asp:TextBox ID="txtProposal" runat="server"  Columns="8" /> USD
                    </td>
                </tr>            
                <tr>
                    <td colspan="2" align="center"><br />
                    <input type="button" value="Go Back" onclick="document.location='default.aspx';" />
                    <asp:Button ID="btnCreate" runat="server" Text="Submit Offer" OnClick="OnInputProposal"/></td>
                </tr>            
            </table>    
        </asp:Panel>
        
        <br />
        
        <asp:Panel ID="pnlSubmited" runat="server" Visible="false">
            <br />
            <br />
            <h2>You have already submited your proposal</h2>
            <b>You can't submit offers anymore to this RFP...</b>
            <br />            
        </asp:Panel>      
        
        <asp:Panel ID="pnlNotInvited" runat="server" Visible="false">
            <br />
            <br />
            <h2>You can't submit a proposal</h2>
            <b>You have been not invited to submit proposals to this Request for Proposals</b>
            <br />            
        </asp:Panel>      
                
        <asp:Panel ID="pnlThanks" runat="server" Visible="false">
            <h2>Thanks for submiting your proposal!</h2>
            Your proposal will be evaluated against the rest of the proposals received from our vendors.
            <br />            
        </asp:Panel>  
        
        <asp:Panel ID="pnlGoBack" runat="server" Visible="false">
            <a href="Default.aspx">Click here to back go to the main page</a>
        </asp:Panel>              

    </div>
    </form>
</body>
</html>

<!-- Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateRfp.aspx.cs" Inherits="Microsoft.Samples.WF.PurchaseProcess.WebClient.CreateRfp" %>
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
        Create a new Request for Proposals. This screen is used by the employees of the buying department of Company X.
        <hr />
        
        <h2>Create Request for Proposals</h2>
        <table>
            <tr>
                <td>Title</td>
                <td><asp:TextBox ID="txtTitle" runat="server" Columns="50" Font-Names="Calibri, Arial"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Description</td>
                <td><asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Columns="50" Rows="8" Font-Names="Calibri, Arial"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Invited Vendors</td>
                <td>
                    <asp:CheckBoxList ID="chkVendorsList" runat="server" TextAlign="Right"  />
                </td>
            </tr>        
            <tr>
                <td colspan="2" align="center"><br />
                    <input type="button" value="Go Back" onclick="history.back();" /> 
                    <asp:Button ID="btnCreate" runat="server" Text="Submit RFP" OnClick="CreateAndSubmitRfp" />
                </td>
            </tr>            
        </table>
    </div>
    </form>
</body>
</html>

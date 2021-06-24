<!-- Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="Microsoft.Samples.WF.PurchaseProcess.WebClient._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Windows Workflow Foundation 4.0 SDK</title>
    <link rel="Stylesheet" type="text/css" href="css/StyleSheet.css" />
    <script language="javascript" type="text/javascript">
        function showProposal(id) {
            var combo = document.getElementById('sel_' + id);
            if (combo.value == '') {
                document.location = 'ShowRfp.aspx?id=' + id;
            }
            else {
                document.location = 'GetVendorProposal.aspx?id=' + id + '&vendorId=' + combo.value;
            }           
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Purchase Process Sample - Best Vendor Selection</h1>        
        This sample shows a best vendor selection scenario within a corporate purchase process using Windows Workflow Foundation 4.0.         
        <br />In this sample, an employee from an imaginary company
        creates a Request For Proposals (RFP) and submits it to several vendors. The vendors can access to the system, read the RFP and introduce a target value. After all the vendors respond to the
        Request for Proposal, the Workflow selects the best offer based on the vendor's response and his reputation.
        <br />
        <hr />
        <br />
        This screen allows emulating several participants. After a workflow instance is created, it will be displayed in the list (on the <b>Active</b> section) and you will be able to access as several participants.        
        <br />        
        <br />
        <input type="button" value="Create a new RFP" onclick="document.location='CreateRfp.aspx'" />
        <input type="button" value="Refresh" onclick="document.location.reload()" />
        <hr />
        <br />
        <h2>Active</h2>
        <asp:Literal ID="litActive" runat="server" />
        <br />
        <h2>Finished</h2>
        <asp:Literal ID="litFinished" runat="server" />
    </div>
    </form>
</body>
</html>

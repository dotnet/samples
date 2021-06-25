<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HiringRequestDetails.ascx.cs" Inherits="Microsoft.Samples.WebClient.HiringRequestDetails" %>

<table cellpadding="4" cellspacing="2">
    <tr>
        <td class="fieldName">Requester</td>
        <td>
            <asp:Label ID="lblRequester" runat="server"/>
        </td>
    </tr>
    <tr>
        <td class="fieldName">Position</td>
        <td>
            <asp:Label ID="lblPosition" runat="server"/>
            <asp:DropDownList ID="ddlPosition" runat="server" Visible="false" />
        </td>        
    </tr>            
    <tr>
        <td class="fieldName">Department</td>
        <td>
            <asp:Label ID="lblDepartment" runat="server"/>
            <asp:DropDownList ID="ddlDepartment" runat="server" Visible="false" />
        </td>        
    </tr>            
    <tr>
        <td class="fieldName">Description</td>
        <td>
            <asp:Label ID="lblDescription" runat="server"/>
            <asp:TextBox ID="txtDescription" runat="server" Visible="false" TextMode="MultiLine" Columns="60" Rows="4" />
        </td>
    </tr>      
    <tr runat="server" id="trComments" visible="false">
        <td class="fieldName">Comments</td>
        <td>
            <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" Columns="60" Rows="4"/>
        </td>
    </tr>
</table>     
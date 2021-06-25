<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="UserControls/Title.ascx" %>
<%@ Import Namespace="Microsoft.Samples.ContosoHR"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">    
    void Page_Load(object sender, EventArgs e)
    {                
        DataGrid1.DataSource = JobPostingRepository.SelectActiveJobPostings().Tables[0];        
        DataGrid1.DataBind();
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/Stylesheet1.css" rel="stylesheet" type="text/css" />
    <title>Job Postings List</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="List of Resumees" />
        <br />
        <contoso:Title ID="title1" runat="server" Text="Open Positions, Apply Now!" />                            
        <br />

        <asp:DataGrid ID="DataGrid1" runat="server" GridLines="Both" AutoGenerateColumns="false" CellPadding="4" Width="80%" >
            <Columns>           
                <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date" />   
                <asp:TemplateColumn HeaderText="Title">
                     <ItemTemplate> 
                        <a href="Details.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>"><%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="ResumeesReceived" HeaderText="# Resumees" />
                <asp:TemplateColumn>
                     <ItemTemplate>
                        <a href="Details.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">Apply Now!</a>
                    </ItemTemplate>
                </asp:TemplateColumn>                
            </Columns>             
            <AlternatingItemStyle BackColor="WhiteSmoke" /> 
            <ItemStyle BackColor="White" ForeColor="#333333" /> 
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />             
        </asp:DataGrid>
        <br />
    </div>
    </form>
</body>
</html>

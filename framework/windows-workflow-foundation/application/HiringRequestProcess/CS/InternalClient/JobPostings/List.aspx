<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.Data.SqlClient"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    
    void Page_Load(object sender, EventArgs e)
    {
        DataGrid1.DataSource = Microsoft.Samples.ContosoHR.JobPostingRepository.SelectNotStartedJobPostings().Tables[0];
        DataGrid1.DataBind();

        DataGrid2.DataSource = Microsoft.Samples.ContosoHR.JobPostingRepository.SelectActiveJobPostings().Tables[0];
        DataGrid2.DataBind();

        DataGrid3.DataSource = Microsoft.Samples.ContosoHR.JobPostingRepository.SelectClosedJobPostings().Tables[0];
        DataGrid3.DataBind();
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Job Postings List</title>
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />    
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="Job Postings" />
        <!-- Menu -->
        <table>
            <tr>
                <td><a href="/InternalClient/HiringRequests/Inbox.aspx">Go Back to Inbox</a> | <a href="List.aspx"> Refresh</a></td>                
            </tr>
        </table>        

        <!-- List of Requests -->
        <br />
        <contoso:Title ID="title1" runat="server" Text="Not Started" />            

        <asp:DataGrid ID="DataGrid1" runat="server" GridLines="Both" AutoGenerateColumns="false" CellPadding="4" >
            <Columns>           
                <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date" />   
                <asp:TemplateColumn HeaderText="Title">
                     <ItemTemplate>
                        <a href="Details.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>"><%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Hiring Request">
                     <ItemTemplate>
                        <a href="../HiringRequests/ViewPositionRequest.aspx?id=<%# DataBinder.Eval(Container.DataItem, "HiringRequestId") %>"><%# DataBinder.Eval(Container.DataItem, "HiringRequestId")%></a>
                    </ItemTemplate>
                </asp:TemplateColumn>                                        
                <asp:TemplateColumn>
                     <ItemTemplate>
                        <a href="Details.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">Edit</a>
                    </ItemTemplate>
                </asp:TemplateColumn>                
            </Columns> 
            <AlternatingItemStyle BackColor="White" /> 
            <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" /> 
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" /> 
        </asp:DataGrid>
        <br />
        <br />
        <br />
        <contoso:Title ID="title2" runat="server" Text="Receiving Resumes" />            
        <asp:DataGrid ID="DataGrid2" runat="server" GridLines="Both" AutoGenerateColumns="false" CellPadding="4" >
            <Columns>           
                <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date" />   
                <asp:TemplateColumn HeaderText="Title">
                     <ItemTemplate>
                        <a href="Resumes.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>"><%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="Description" HeaderText="Description" />   
                <asp:BoundColumn DataField="ResumeesReceived" HeaderText="# Resumes" />   
                <asp:TemplateColumn HeaderText="Hiring Request">
                     <ItemTemplate>
                        <a href="../HiringRequests/ViewPositionRequest.aspx?id=<%# DataBinder.Eval(Container.DataItem, "HiringRequestId") %>"><%# DataBinder.Eval(Container.DataItem, "HiringRequestId")%></a>
                    </ItemTemplate>
                </asp:TemplateColumn>                        
                
                <asp:TemplateColumn>
                     <ItemTemplate>
                        <a href="Resumes.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">View Resumes</a>
                    </ItemTemplate>
                </asp:TemplateColumn>                
                
                <asp:TemplateColumn>
                     <ItemTemplate>
                        <a href="Stop.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">Stop</a>
                    </ItemTemplate>
                </asp:TemplateColumn>                
            </Columns>             
            <AlternatingItemStyle BackColor="White" /> 
            <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" /> 
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" /> 
        </asp:DataGrid>        
        
        <br />
        <br />
        <br />
        <contoso:Title ID="title3" runat="server" Text="Closed" />            
        <asp:DataGrid ID="DataGrid3" runat="server" GridLines="Both" AutoGenerateColumns="false" CellPadding="4" >
            <Columns>           
                <asp:BoundColumn DataField="CreationDate" HeaderText="Creation Date" />   
                <asp:TemplateColumn HeaderText="Title">
                     <ItemTemplate>
                        <a href="Resumes.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>"><%# DataBinder.Eval(Container.DataItem, "Title") %></a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="Description" HeaderText="Description" />   
                <asp:BoundColumn DataField="ResumeesReceived" HeaderText="# Resumes" />   
                <asp:TemplateColumn HeaderText="Hiring Request">
                     <ItemTemplate>
                        <a href="../HiringRequests/ViewPositionRequest.aspx?id=<%# DataBinder.Eval(Container.DataItem, "HiringRequestId") %>"><%# DataBinder.Eval(Container.DataItem, "HiringRequestId")%></a>
                    </ItemTemplate>
                </asp:TemplateColumn>                        
                
                <asp:TemplateColumn>
                     <ItemTemplate>
                        <a href="Resumes.aspx?id=<%# DataBinder.Eval(Container.DataItem, "Id") %>">View Resumes</a>
                    </ItemTemplate>
                </asp:TemplateColumn>                                
            </Columns>             
            <AlternatingItemStyle BackColor="White" /> 
            <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" /> 
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" /> 
        </asp:DataGrid>        
    </div>
    </form>
</body>
</html>

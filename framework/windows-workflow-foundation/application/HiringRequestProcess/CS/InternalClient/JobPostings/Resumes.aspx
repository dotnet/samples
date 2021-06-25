<!--  Copyright (c) Microsoft Corporation.  All rights reserved. -->
<%@ Page Language="C#" %>
<%@ Register TagPrefix="contoso" TagName="top" Src="../UserControls/Top.ascx" %>
<%@ Register TagPrefix="contoso" TagName="title" Src="../UserControls/Title.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    
    // bind data when the page is loaded
    void Page_Load(object sender, EventArgs e)
    {
        BindGrid("SelectJobPostingResumees", DataGrid1);        
    }

    // do the data binding
    void BindGrid(string procedureName, DataGrid grid)
    {
        string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

        using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(cnnString))
        {
            cnn.Open();
            System.Data.SqlClient.SqlCommand command = cnn.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = procedureName;
            command.Parameters.Add(new System.Data.SqlClient.SqlParameter("@id", Request["id"]));

            grid.DataSource = command.ExecuteReader();
            grid.DataBind();
        }               
    }

    // click on the button to go back to the list
    void GoBack(object sender, EventArgs e)
    {
        Response.Redirect("List.aspx");
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../css/Stylesheet1.css" rel="stylesheet" type="text/css" />
    <title>Resumees for Job Posting</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <contoso:Top id="top1" runat="server" Text="List of Resumees" />
        <br />
        <contoso:Title ID="title1" runat="server" Text="Received Resumees" />            

        <asp:DataGrid ID="DataGrid1" runat="server" GridLines="Both" AutoGenerateColumns="false" CellPadding="4" >
            <Columns>           
                <asp:BoundColumn DataField="ReceivedDate" HeaderText="Date" />   
                <asp:BoundColumn DataField="CandidateMail" HeaderText="Mail" />   
                <asp:BoundColumn DataField="CandidateName" HeaderText="Name" />   
                <asp:BoundColumn DataField="Resumee" HeaderText="Resumee" />   
            </Columns> 
            <AlternatingItemStyle BackColor="White" /> 
            <ItemStyle BackColor="#FFFBD6" ForeColor="#333333" /> 
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" /> 
        </asp:DataGrid>
        
        <br />
        <br />
        <asp:Button id="btnCancel" runat="server" Text="<< Back to List" OnClick="GoBack" class="actionButton" />
     </div>
    </form>
</body>
</html>

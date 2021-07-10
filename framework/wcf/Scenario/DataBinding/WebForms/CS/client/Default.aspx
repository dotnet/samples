<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.ServiceModel" %>
<%@ Import Namespace="Microsoft.Samples.WebForms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<body>
    <h3><font face="Verdana">Weather Service</font></h3>

    <form id="Form2" runat="server">

        <asp:DataGrid id="dataGrid1" runat="server"
          BorderColor="black"
          BorderWidth="1"
          GridLines="Both"
          CellPadding="3"
          CellSpacing="0"
          HeaderStyle-BackColor="#aaaadd"
        />

    </form>
</body>
</html>




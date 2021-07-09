<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>JSONP Service Client Page</title>

    <script type="text/javascript">
    // <![CDATA[

        function makeCall() {
            var proxy = new JsonpAjaxService.CustomerService();
            proxy.set_enableJsonp(true);
            proxy.GetCustomer(onSuccess, onFail, null);
        }
        
        // This function is called when the result from the service call is received
        function onSuccess(result) {
            document.getElementById("name").value = result.Name;
            document.getElementById("address").value = result.Address;
        }

        // This function is called if the service call fails
        function onFail(){
            document.getElementById("name").value = "Error";
            document.getElementById("address").value = "Error";
        }
    
    // ]]>
    </script>

</head>
<body>
    <form id="form1" runat="server"> 
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="http://localhost:33695/service.svc" />
        </Services>
    </asp:ScriptManager>
    <h1>
        JSONP Service Client Page</h1>
        Customer:
        <p/>
        Name: <input type="text" id="name"/>
        <p/>
        Address: <input type="text" id="address"/>
        
        <script type="text/javascript" defer="defer">makeCall();</script>
   
    </form>
</body>
</html>

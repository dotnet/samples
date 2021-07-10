<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Complex Type AJAX Service Client Page</title>

    <script type="text/javascript">
    // <![CDATA[
    
    // This function creates an asynchronous call to the service
    function makeCall(){
        var n1 = document.getElementById("num1").value;
        var n2 = document.getElementById("num2").value;
        
        // If user filled out these fields, call the service
        if(n1 && n2){
        
            // Instantiate a service proxy
            var proxy = new ComplexTypeAjaxService.ICalculator();
        
            // Call DoMath operation on proxy
            proxy.DoMath(parseFloat(n1), parseFloat(n2), onSuccess, onFail, null);
        }
    }

    // This function is called when the result from the service call is received
    function onSuccess(mathResult){
        document.getElementById("sum").value = mathResult.sum;
        document.getElementById("difference").value = mathResult.difference;
        document.getElementById("product").value = mathResult.product;
        document.getElementById("quotient").value = mathResult.quotient;
    }

    // This function is called if the service call fails
    function onFail(){
        document.getElementById("sum").value = "Error";
        document.getElementById("difference").value = "Error";
        document.getElementById("product").value = "Error";
        document.getElementById("quotient").value = "Error";
    }
    
    // ]]>
    </script>

</head>
<body>
    <h1>
        Complex Type AJAX Service Client Page</h1>
    <p>
        First Number:
        <input type="text" id="num1" /></p>
    <p>
        Second Number:
        <input type="text" id="num2" /></p>
    <input id="btn" type="button" onclick="return makeCall();" value="Perform calculation" />
    <p>
        Sum:
        <input type="text" id="sum" /></p>
    <p>
        Difference:
        <input type="text" id="difference" /></p>
    <p>
        Product:
        <input type="text" id="product" /></p>
    <p>
        Quotient:
        <input type="text" id="quotient" /></p>
    <form id="mathForm" action="" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
        <Services>
            <asp:ServiceReference Path="service.svc" />
        </Services>
    </asp:ScriptManager>
    </form>
</body>
</html>

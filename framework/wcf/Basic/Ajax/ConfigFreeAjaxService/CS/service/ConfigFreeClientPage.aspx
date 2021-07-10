<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Config Free AJAX Service Client Page</title>

    <script type="text/javascript">
    // <![CDATA[
    
    // This function creates an asynchronous call to the service
    function makeCall(operation){
        var n1 = document.getElementById("num1").value;
        var n2 = document.getElementById("num2").value;

        // If user filled out these fields, call the service
        if(n1 && n2){
        
            // Instantiate a service proxy
            var proxy = new  ConfigFreeAjaxService.ICalculator();

            // Call correct operation on proxy       
            switch(operation){
                case "Add":
                    proxy.Add(parseFloat(n1), parseFloat(n2), onSuccess, onFail, null);            
                break;
                
                case "Subtract":
                    proxy.Subtract(parseFloat(n1), parseFloat(n2), onSuccess, onFail, null);                        
                break;
                
                case "Multiply":
                    proxy.Multiply(parseFloat(n1), parseFloat(n2), onSuccess, onFail, null);            
                break;
                
                case "Divide":
                    proxy.Divide(parseFloat(n1), parseFloat(n2), onSuccess, onFail, null);            
                break;
            }
        }
    }

    // This function is called when the result from the service call is received
    function onSuccess(mathResult){
        document.getElementById("result").value = mathResult;
    }

    // This function is called if the service call fails
    function onFail(){
        document.getElementById("result").value = "Error";
    }
    // ]]>
    </script>
</head>
<body>
    <h1>
        Config Free AJAX Service Client Page</h1>
    <p>
        First Number:
        <input type="text" id="num1" /></p>
    <p>
        Second Number:
        <input type="text" id="num2" /></p>
    <input id="btnAdd" type="button" onclick="return makeCall('Add');" value="Add" />
    <input id="btnSubtract" type="button" onclick="return makeCall('Subtract');" value="Subtract" />
    <input id="btnMultiply" type="button" onclick="return makeCall('Multiply');" value="Multiply" />
    <input id="btnDivide" type="button" onclick="return makeCall('Divide');" value="Divide" />
    <p>
        Result:
        <input type="text" id="result" /></p>
    <form id="mathForm" action="" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server">
        <Services>
            <asp:ServiceReference Path="service.svc" />
        </Services>
    </asp:ScriptManager>
    </form>
</body>
</html>

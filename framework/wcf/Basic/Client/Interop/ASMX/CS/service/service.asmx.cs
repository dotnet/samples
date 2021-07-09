
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Web.Services;

namespace Microsoft.Samples.WCFClientInteropASMX
{
	/// <summary>
	/// Simple ASMX Calculator Web Service.	
	/// </summary>
	[WebService(Namespace="http://Microsoft.Samples.WCFClientInteropASMX")]
    public class CalculatorService : System.Web.Services.WebService
    {        
        [WebMethod]
        public double Add(double n1, double n2)
        {
            return n1 + n2;
        }
        [WebMethod]
        public double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }
        [WebMethod]
        public double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }
        [WebMethod]
        public double Divide(double n1, double n2)
        {
            return n1 / n2;
        }
    } 
        
}


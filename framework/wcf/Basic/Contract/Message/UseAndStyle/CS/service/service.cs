
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Diagnostics;

namespace Microsoft.Samples.UseAndStyle
{
    // Style (OperationFormatStyle): 
    // This property determines how the WSDL metadata for the service is formatted.
    // Possible values are Document and Rpc.

    // This version of the service contract uses Use.Literal and OperationFormatStyle.Rpc
    // on a DataContractFormat attribute..
    // [ServiceContract(Namespace = "http://Microsoft.Samples.UseAndStyle"),
    // DataContractFormat(Style = OperationFormatStyle.Rpc)]

    // This version of the service contract uses Use.Encoded and OperationFormatStyle.Rpc
    //on a XmlSerializerFormat attribute.
    [ServiceContract(Namespace = "http://Microsoft.Samples.UseAndStyle"),
     XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
 
    public interface IUseAndStyleCalculator
    {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    public class CalculatorService : IUseAndStyleCalculator
    {
        // Create a static TraceSource to the configured trace listeners
        static TraceSource ts = new TraceSource("CalculatorServiceTraceSource");


        public double Add(double n1, double n2)
        {
            double result = n1 + n2;

            ts.TraceInformation("Received Add({0},{1})",n1,n2);
            ts.TraceInformation("Return: {0}",result);
            ts.Flush();

            return result;
        }

        public double Subtract(double n1, double n2)
        {
            double result = n1 - n2;

            ts.TraceInformation("Received Subtract({0},{1})", n1, n2);
            ts.TraceInformation("Return: {0}", result);
            ts.Flush();

            return result;
        }

        public double Multiply(double n1, double n2)
        {
            double result = n1*n2;

            ts.TraceInformation("Received Multiply({0},{1})", n1, n2);
            ts.TraceInformation("Return: {0}", result);
            ts.Flush();

            return result;
        }

        public double Divide(double n1, double n2)
        {
            double result = n1/n2;

            ts.TraceInformation("Received Divide({0},{1})", n1, n2);
            ts.TraceInformation("Return: {0}", result);
            ts.Flush();

            return result;
        }
    }

}

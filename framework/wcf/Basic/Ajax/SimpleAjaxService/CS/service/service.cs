//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Samples.SimpleAjaxService 
{
    // Define a service contract.
    [ServiceContract(Namespace = "SimpleAjaxService")]
    public interface ICalculator
    {
        [WebGet]
        double Add(double n1, double n2);
        [WebGet]
        double Subtract(double n1, double n2);
        [WebGet]
        double Multiply(double n1, double n2);
        [WebGet]
        double Divide(double n1, double n2);
    }

    public class CalculatorService : ICalculator
    {

        public double Add(double n1, double n2)
        {
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            return n1 / n2;
        }
    }

}

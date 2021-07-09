
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.ServiceModel;

namespace Microsoft.Samples.RouteByBody
{
    [ServiceContract(Namespace = "http://Microsoft.Samples.RouteByBody"), XmlSerializerFormat, DispatchByBodyBehavior]
    public interface ICalculator
    {
        [OperationContract(Action="")]
        double Add(double n1, double n2);
        [OperationContract(Action = "")]
        double Subtract(double n1, double n2);
        [OperationContract(Action = "")]
        double Multiply(double n1, double n2);
        [OperationContract(Action = "")]
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

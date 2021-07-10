
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    // Document it.
    [WsdlDocumentation("The ICalculator contract performs basic calculation services.")]
    public interface ICalculator
    {
        [OperationContract]
        [WsdlDocumentation("The Add operation adds two numbers and returns the result.")]
        [return:WsdlParamOrReturnDocumentation("The result of adding the two arguments together.")]
        double Add(
          [WsdlParamOrReturnDocumentation("The first value to add.")]double n1, 
          [WsdlParamOrReturnDocumentation("The second value to add.")]double n2
        );
        
        [OperationContract]
        [WsdlDocumentation("The Subtract operation subtracts the second argument from the first.")]
        [return:WsdlParamOrReturnDocumentation("The result of the second argument subtracted from the first.")]
        double Subtract(
          [WsdlParamOrReturnDocumentation("The value from which the second is subtracted.")]double n1, 
          [WsdlParamOrReturnDocumentation("The value that is subtracted from the first.")]double n2
        );
        
        [OperationContract]
        [WsdlDocumentation("The Multiply operation multiplies two values.")]
        [return:WsdlParamOrReturnDocumentation("The result of multiplying the first and second arguments.")]
        double Multiply(
          [WsdlParamOrReturnDocumentation("The first value to multiply.")]double n1, 
          [WsdlParamOrReturnDocumentation("The second value to multiply.")]double n2
        );
  
        [OperationContract]
        [WsdlDocumentation("The Divide operation returns the value of the first argument divided by the second argument.")]
        [return:WsdlParamOrReturnDocumentation("The result of dividing the first argument by the second.")]
        double Divide(
          [WsdlParamOrReturnDocumentation("The numerator.")]double n1, 
          [WsdlParamOrReturnDocumentation("The denominator.")]double n2
        );
    }

    // Service class which implements the service contract.
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


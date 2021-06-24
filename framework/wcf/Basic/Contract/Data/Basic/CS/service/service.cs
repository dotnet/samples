
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Microsoft.Samples.Data
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.Data")]
    public interface IDataContractCalculator
    {
        [OperationContract]
        ComplexNumber Add(ComplexNumber n1, ComplexNumber n2);
        [OperationContract]
        ComplexNumber Subtract(ComplexNumber n1, ComplexNumber n2);
        [OperationContract]
        ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2);
        [OperationContract]
        ComplexNumber Divide(ComplexNumber n1, ComplexNumber n2);
    }

    [DataContract(Namespace="http://Microsoft.Samples.Data")]
    public class ComplexNumber
    {
        [DataMember]
        public double Real;
        [DataMember]
        public double Imaginary;

        public ComplexNumber(double real, double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }
    }

    // Service class which implements the service contract.
    public class DataContractCalculatorService : IDataContractCalculator
    {
        public ComplexNumber Add(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Real + n2.Real, n1.Imaginary + n2.Imaginary);
        }

        public ComplexNumber Subtract(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Real - n2.Real, n1.Imaginary - n2.Imaginary);
        }

        public ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2)
        {
            double real1 = n1.Real * n2.Real;
            double imaginary1 = n1.Real * n2.Imaginary;
            double imaginary2 = n2.Real * n1.Imaginary;
            double real2 = n1.Imaginary * n2.Imaginary * -1;
            return new ComplexNumber(real1 + real2, imaginary1 + imaginary2);
        }

        public ComplexNumber Divide(ComplexNumber n1, ComplexNumber n2)
        {
            ComplexNumber conjugate = new ComplexNumber(n2.Real, -1*n2.Imaginary);
            ComplexNumber numerator = Multiply(n1, conjugate);
            ComplexNumber denominator = Multiply(n2, conjugate);
            return new ComplexNumber(numerator.Real / denominator.Real, numerator.Imaginary);
        }
    }

}

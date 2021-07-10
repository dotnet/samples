
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace Microsoft.Samples.XmlSerializer
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.XmlSerializer"), XmlSerializerFormat]
    public interface IXmlSerializerCalculator
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

    public class ComplexNumber
    {
        private double real;
        private double imaginary;

        public ComplexNumber() { }

        public ComplexNumber(double real, double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }

        [XmlAttribute]
        public double Real
        {
            get { return real; }
            set { real = value; }
        }

        [XmlAttribute]
        public double Imaginary
        {
            get { return imaginary; }
            set { imaginary = value; }
        }
    }

    // Service class which implements the service contract.
    public class XmlSerializerCalculatorService : IXmlSerializerCalculator
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
            return new ComplexNumber(numerator.Real / denominator.Real, numerator.Imaginary / denominator.Real);
        }
    }
}

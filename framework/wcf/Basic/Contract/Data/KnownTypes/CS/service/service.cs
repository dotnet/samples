
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Microsoft.Samples.KnownTypes
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.KnownTypes")]
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

    // Define the data contract for a complex number
    [DataContract(Namespace="http://Microsoft.Samples.KnownTypes")]
    //Indicate that ComplexNumberWithMagnitude may be used in place of ComplexNumber
    [KnownType(typeof(ComplexNumberWithMagnitude))]
    public class ComplexNumber
    {
        [DataMember]
        private double real;
        [DataMember]
        private double imaginary;

        public ComplexNumber(double r1, double i1)
        {
            this.Real = r1;
            this.Imaginary = i1;
        }

        public double Real
        {
            get { return real; }
            set { real = value; }
        }

        public double Imaginary
        {
            get { return imaginary; }
            set { imaginary = value; }
        }
    }

    //Define the data contract for ComplexNumberWithMagnitude
[DataContract(Namespace="http://Microsoft.Samples.KnownTypes")]
public class ComplexNumberWithMagnitude : ComplexNumber
{
    public ComplexNumberWithMagnitude(double real, double imaginary) : base(real, imaginary) { }

    [DataMember]
    public double Magnitude
    {
        get { return Math.Sqrt(Imaginary*Imaginary  + Real*Real); }
        set { throw new NotImplementedException(); }
    }
}

    // Service class which implements the service contract.
    public class DataContractCalculatorService : IDataContractCalculator
    {
        public ComplexNumber Add(ComplexNumber n1, ComplexNumber n2)
        {
            //Return the derived type
            return new ComplexNumberWithMagnitude(n1.Real + n2.Real, n1.Imaginary + n2.Imaginary);
        }

        public ComplexNumber Subtract(ComplexNumber n1, ComplexNumber n2)
        {
            //Return the derived type
            return new ComplexNumberWithMagnitude(n1.Real - n2.Real, n1.Imaginary - n2.Imaginary);
        }

        public ComplexNumber Multiply(ComplexNumber n1, ComplexNumber n2)
        {
            double real1 = n1.Real * n2.Real;
            double imaginary1 = n1.Real * n2.Imaginary;
            double imaginary2 = n2.Real * n1.Imaginary;
            double real2 = n1.Imaginary * n2.Imaginary * -1;
            //Return the base type
            return new ComplexNumber(real1 + real2, imaginary1 + imaginary2);
        }

        public ComplexNumber Divide(ComplexNumber n1, ComplexNumber n2)
        {
            ComplexNumber conjugate = new ComplexNumber(n2.Real, -1*n2.Imaginary);
            ComplexNumber numerator = Multiply(n1, conjugate);
            ComplexNumber denominator = Multiply(n2, conjugate);
            //Return the base type
            return new ComplexNumber(numerator.Real / denominator.Real, numerator.Imaginary);
        }
    }

}

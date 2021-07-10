
//-----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//-----------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.KAA.Types
{

    [DataContract]
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

    [DataContract]
    public class ComplexNumberWithMagnitude : ComplexNumber
    {
        public ComplexNumberWithMagnitude(double real, double imaginary) : base(real, imaginary) { }

        [DataMember]
        public double Magnitude
        {
            get { return Math.Sqrt(Imaginary * Imaginary + Real * Real); }
            set { }
        }
    }

}

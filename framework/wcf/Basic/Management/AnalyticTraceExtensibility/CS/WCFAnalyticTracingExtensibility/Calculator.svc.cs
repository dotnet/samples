//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.WCFAnalyticTracingExtensibility
{

    public class Calculator : ICalculator
    {
        static WCFUserEventProvider eventWriter = new WCFUserEventProvider();

        public double Add(double n1, double n2)
        {
            double ret = n1 + n2;
            if (ret == double.PositiveInfinity || ret == double.NegativeInfinity)
            {
                eventWriter.WriteWarningEvent("PotentialArithmeticOverflow", string.Format("{0}+{1}={2}", n1, n2, ret));
            }

            eventWriter.WriteInformationEvent("DidAddLogic", string.Format("{0}+{1}={2}", n1, n2, ret));
            return ret;
        }

        public double Subtract(double n1, double n2)
        {
            double ret = n1 - n2;
            eventWriter.WriteInformationEvent("DidSubtractLogic", string.Format("{0}-{1}={2}", n1, n2, ret));
            return ret;
        }

        public double Multiply(double n1, double n2)
        {
            double ret = n1 * n2;
            eventWriter.WriteInformationEvent("DidMultiplyLogic", string.Format("{0}*{1}={2}", n1, n2, ret));
            return ret;
        }

        public double Divide(double n1, double n2)
        {
            if (n2 == 0)
            {
                eventWriter.WriteErrorEvent("DivideByZero", string.Format("User passed in zero as the divisor.  Dividend was {0}", n1));
                throw new DivideByZeroException();
            }

            double ret = n1 / n2;
            eventWriter.WriteInformationEvent("DidDivideLogic", string.Format("{0}/{1}={2}", n1, n2, ret));
            return ret;
        }
    }
}

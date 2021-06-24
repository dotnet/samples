//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Samples.XmlAjaxService 
{
    // Define a service contract.
    [ServiceContract(Namespace = "XmlAjaxService")]
    public interface ICalculator
    {
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        MathResult DoMathJson(double n1, double n2);

        [WebInvoke(ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]
        MathResult DoMathXml(double n1, double n2);

    }

    public class CalculatorService : ICalculator
    {

        public MathResult DoMathJson(double n1, double n2)
        {
            return DoMath(n1, n2);
        }

        public MathResult DoMathXml(double n1, double n2)
        {
            return DoMath(n1, n2);
        }

        private MathResult DoMath(double n1, double n2)
        {
            MathResult mr = new MathResult();
            mr.sum = n1 + n2;
            mr.difference = n1 - n2;
            mr.product = n1 * n2;
            mr.quotient = n1 / n2;
            return mr;
        }
    }

    [DataContract]
    public class MathResult
    {
        [DataMember]
        public double sum;

        [DataMember]
        public double difference;

        [DataMember]
        public double product;

        [DataMember]
        public double quotient;
    }

}

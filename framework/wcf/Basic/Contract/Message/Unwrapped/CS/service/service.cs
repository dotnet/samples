
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Diagnostics;

namespace Microsoft.Samples.Unwrapped
{
    [ServiceContract(Namespace="http://Microsoft.Samples.Unwrapped")]
    public interface ICalculator
    {
        [OperationContract]
        ResponseMessage Add(RequestMessage request);
        [OperationContract]
        ResponseMessage Subtract(RequestMessage request);
        [OperationContract]
        ResponseMessage Multiply(RequestMessage request);
        [OperationContract]
        ResponseMessage Divide(RequestMessage request);
    }

    //setting IsWrapped to false means the n1 and n2
    //members will be direct children of the soap body element
    [MessageContract(IsWrapped = false)]
    public class RequestMessage
    {
        [MessageBodyMember]
        private double n1;
        [MessageBodyMember]
        private double n2;
        public double N1
        {
            get { return n1; }
            set { n1 = value; }
        }
        public double N2
        {
            get { return n2; }
            set { n2 = value; }
        }
 
    }

    //setting IsWrapped to false means the result
    //member will be a direct child of the soap body element
    [MessageContract(IsWrapped = false)]
    public class ResponseMessage
    {
        [MessageBodyMember]
        private double result;
        public double Result
        {
            get { return result; }
            set { result = value; }
        }

    }

    public class CalculatorService : ICalculator
    {
        public ResponseMessage Add(RequestMessage request)
        {
            double n1 = request.N1;
            double n2 = request.N2;
            double result = n1 + n2;

            ResponseMessage response = new ResponseMessage();
            response.Result = result;
            return response;
        }

        public ResponseMessage Subtract(RequestMessage request)
        {
            double n1 = request.N1;
            double n2 = request.N2;
            double result = n1 - n2;

            ResponseMessage response = new ResponseMessage();
            response.Result = result;
            return response;
        }

        public ResponseMessage Multiply(RequestMessage request)
        {
            double n1 = request.N1;
            double n2 = request.N2;
            double result = n1*n2;

            ResponseMessage response = new ResponseMessage();
            response.Result = result;
            return response;
        }

        public ResponseMessage Divide(RequestMessage request)
        {
            double n1 = request.N1;
            double n2 = request.N2;
            double result = n1/n2;

            ResponseMessage response = new ResponseMessage();
            response.Result = result;
            return response;
        }
    }

}

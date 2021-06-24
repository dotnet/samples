
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml;
using System.Globalization;

namespace Microsoft.Samples.Untyped
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.Untyped")]
    public interface ICalculator
    {
        [OperationContract(Action = CalculatorService.RequestAction, ReplyAction = CalculatorService.ReplyAction)]
        Message ComputeSum(Message request);
    }

   
    // Service class which implements the service contract.
    public class CalculatorService : ICalculator
    {
        // Perform a calculation.
        public const String ReplyAction = "http://test/Message_ReplyAction";
        public const String RequestAction = "http://test/Message_RequestAction";

        public Message ComputeSum(Message request)
        {
            //The body of the message contains a list of numbers which will be read as a int[] using GetBody<T>
            int result = 0;

            int[] inputs = request.GetBody<int[]>();
            foreach (int i in inputs)
            {
                result += i;
            }

            Message response = Message.CreateMessage(request.Version, ReplyAction, result);
            return response;
        }
    }

}

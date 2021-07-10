
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Web;

namespace Microsoft.Samples.AspNetCompatibility
{
    // Define a service contract that uses a session.
    // ICalculatorSession allows one to perform multiple operations on a running result
    // One can retrieve the current result by calling Equals()
    // One can begin calculating a new result by calling Clear()
    [ServiceContract(Namespace="http://Microsoft.Samples.AspNetCompatibility")]
    public interface ICalculatorSession
    {
        [OperationContract]
        void Clear();
        [OperationContract]
        void AddTo(double n);
        [OperationContract]
        void SubtractFrom(double n);
        [OperationContract]
        void MultiplyBy(double n);
        [OperationContract]
        void DivideBy(double n);
        [OperationContract]
        double Result();
    }

    // Service class which implements the service contract.
    // Utilize AspSessionState to manage each calculator session.
    // Requiring AspNetCompatibilityMode allows one access to the HttpContext and Session.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CalculatorService : ICalculatorSession
    {
        double result
        {   // store result in AspNet Session
            get
            {
                if (HttpContext.Current.Session["Result"] != null)
                    return (double)HttpContext.Current.Session["Result"];
                return 0.0D;
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        public void Clear()
        {
            result = 0.0D;
        }

        public void AddTo(double n)
        {
            result += n;
        }

        public void SubtractFrom(double n)
        {
            result -= n;
        }

        public void MultiplyBy(double n)
        {
            result *= n;
        }

        public void DivideBy(double n)
        {
            result /= n;
        }

        public double Result()
        {
            return result;
        }
    }

}

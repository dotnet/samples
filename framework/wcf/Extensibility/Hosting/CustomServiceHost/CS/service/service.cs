
// THIS IS A DUMMY FILE FOR THE BCZ AND IS NOT NEEDED, NOT USED. 
// THE SERVICE CODE IS ENTIRELY CONTAINED IN SERVICE.SVC.

//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    public interface ICalculator
    {
    }

    // Service class which implements the service contract.
    [ServiceBehavior]
    public class CalculatorService : ICalculator
    {
    }

}

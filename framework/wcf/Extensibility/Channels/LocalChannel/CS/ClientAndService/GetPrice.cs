//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;

namespace Microsoft.Samples.LocalChannel
{

    # region Service Contract
    [ServiceContract]
    public interface IGetPrice
    {
        [OperationContract]
        string GetPriceForProduct(int productCode);
    }

    public class GetPrice : IGetPrice
    {
        public string GetPriceForProduct(int productId)
        {
            return (String.Format("The price of product Id {0} is ${1}.",
                productId, new Random().Next(50, 100)));
        }
    }
    # endregion
}

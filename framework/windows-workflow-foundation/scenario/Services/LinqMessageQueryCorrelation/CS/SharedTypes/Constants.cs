//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;

namespace Microsoft.Samples.LinqMessageQueryCorrelation.SharedTypes
{
    public static class Constants
    {
        public const string ServiceAddress = "http://localhost:8080/Service";
        public static readonly Binding Binding = new BasicHttpBinding();
        public const string DefaultNamespace = "http://Microsoft.ServiceModel.Samples";
        public const string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

        public static readonly XName POContractName = XName.Get("IPurchaseOrder", DefaultNamespace);
        public const string SubmitPOName = "SubmitPO";
        public const string ConfirmPurchaseOrder = "ConfirmPurchaseOrder";
    }
}

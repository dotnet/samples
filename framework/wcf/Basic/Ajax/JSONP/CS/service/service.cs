//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Microsoft.Samples.Jsonp
{
    [DataContract]
    public class Customer
    {      
        [DataMember]
        public string Name;
        
        [DataMember]
        public string Address;
    }


    [ServiceContract(Namespace="JsonpAjaxService")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CustomerService
    {
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public Customer GetCustomer()
        {
           return new Customer() { Name="Bob", Address="1 Example Way"};
        }
    }
}

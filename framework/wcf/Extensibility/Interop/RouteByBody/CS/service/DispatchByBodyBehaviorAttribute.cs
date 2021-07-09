
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.Xml;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Samples.RouteByBody
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    sealed class DispatchByBodyBehaviorAttribute : Attribute, IContractBehavior
	{
        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // no binding parameters need to be set here
            return;
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            // this is a dispatch-side behavior which doesn't require
            // any action on the client
            return;
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
            // We iterate over the operation descriptions in the contract and
            // record the QName of the request body child element and corresponding operation name
            // to the dictionary to be used for dispatch 
            Dictionary<XmlQualifiedName,string> dispatchDictionary = new Dictionary<XmlQualifiedName,string>();
            foreach( OperationDescription operationDescription in contractDescription.Operations )
            {
                XmlQualifiedName qname =
                    new XmlQualifiedName(operationDescription.Messages[0].Body.WrapperName, operationDescription.Messages[0].Body.WrapperNamespace);
                
                dispatchDictionary.Add(qname, operationDescription.Name);                
            }
            
            // Lastly, we create and assign and instance of our operation selector that
            // gets the dispatch dictionary we've just created.
            dispatchRuntime.OperationSelector = 
                new DispatchByBodyElementOperationSelector(dispatchDictionary);
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            // 
        }

        #endregion
    }
}

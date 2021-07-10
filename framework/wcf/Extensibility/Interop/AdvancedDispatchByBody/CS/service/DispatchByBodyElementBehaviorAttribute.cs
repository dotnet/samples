
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.Xml;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Samples.AdvancedDispatchBody
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
	sealed class DispatchByBodyElementBehaviorAttribute : Attribute, IContractBehavior
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
            // try to locate an DispatchBodyElementAttribute behaviors on each 
            // operation. If found, we add the operation, keyed by QName of the body element 
            // that selects which calls shall be dispatched to this operation to a 
            // dictionary. 
            Dictionary<XmlQualifiedName,string> dispatchDictionary = new Dictionary<XmlQualifiedName,string>();
            foreach( OperationDescription operationDescription in contractDescription.Operations )
            {
                DispatchBodyElementAttribute dispatchBodyElement = 
                    operationDescription.Behaviors.Find<DispatchBodyElementAttribute>();
                if ( dispatchBodyElement != null )
                {
                    dispatchDictionary.Add(dispatchBodyElement.QName, operationDescription.Name);
                }
            }
            
            // Lastly, we create and assign and instance of our operation selector that
            // gets the dispatch dictionary we've just created.
            dispatchRuntime.OperationSelector = 
                new DispatchByBodyElementOperationSelector(
                   dispatchDictionary, 
                   dispatchRuntime.UnhandledDispatchOperation.Name);
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            // 
        }

        #endregion
    }
}

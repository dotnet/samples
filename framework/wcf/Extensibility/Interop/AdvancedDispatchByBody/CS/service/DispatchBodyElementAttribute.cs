
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.Xml;

namespace Microsoft.Samples.AdvancedDispatchBody
{
    [AttributeUsage(AttributeTargets.Method)]
	sealed class DispatchBodyElementAttribute : Attribute, IOperationBehavior
	{
        XmlQualifiedName qname;
        
        public DispatchBodyElementAttribute(string name)
        {
            qname = new XmlQualifiedName(name);
        }

        public DispatchBodyElementAttribute(string name, string ns)
        {
            qname = new XmlQualifiedName(name, ns);
        }

        internal XmlQualifiedName QName
        {
            get { return qname; }
            set { qname = value; }
        }


        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
            
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        #endregion
    }
}

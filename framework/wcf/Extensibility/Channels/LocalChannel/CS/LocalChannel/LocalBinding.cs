//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{
    
    public class LocalBinding : Binding, IBindingRuntimePreferences
    {
        protected LocalTransportBindingElement transport;

        public LocalBinding() : base()
        {
            transport = new LocalTransportBindingElement();
        }

        public override string Scheme
        {
            get { return this.transport.Scheme; }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection bindingElements = new BindingElementCollection();
            bindingElements.Add(this.transport);
            return bindingElements.Clone();
        }

        public bool ReceiveSynchronously
        {
            get { return false; }
        }
    }
}

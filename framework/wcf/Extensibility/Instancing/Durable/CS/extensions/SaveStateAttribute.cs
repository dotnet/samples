using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{
    //This class contains the implementation of the attribute that 
    //can be used with the service methods to notify that the service  
    //instance must be persisted after the method invocation.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SaveStateAttribute : Attribute, IOperationBehavior
    {
        public SaveStateAttribute()
        {
        }

        public void AddBindingParameters(OperationDescription operationDescription,
                                         BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription,
                                        ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription,
                                          DispatchOperation dispatchOperation)
        {
            if (dispatchOperation == null)
                throw new ArgumentNullException("dispatchOperation");

            // Wrap the operation invoker inside our custom operation 
            // invoker which does the persisting work.
            dispatchOperation.Invoker = new OperationInvoker(dispatchOperation.Invoker);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{

    //This class contains the implementation of IOperationInvoker
    //which is used to persist the service instance after invoking the 
    //operation.
    class OperationInvoker : IOperationInvoker
    {
        IOperationInvoker innerOperationInvoker;

        public OperationInvoker(IOperationInvoker innerOperationInvoker)
        {
            this.innerOperationInvoker = innerOperationInvoker;
        }

        public object[] AllocateInputs()
        {
            return innerOperationInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            // Invoke the operation using the inner operation 
            // invoker.
            object result = innerOperationInvoker.Invoke(instance,
                inputs, out outputs);

            // Save the instance using the storage manager saved in the 
            // current InstanceContext.
            InstanceContextExtension extension =
                OperationContext.Current.InstanceContext.Extensions.Find<InstanceContextExtension>();

            extension.StorageManager.SaveInstance(extension.ContextId, instance);
            return result;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs,
            AsyncCallback callback, object state)
        {
            return innerOperationInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult asyncResult)
        {
            // Finish invoking the operation using the inner operation 
            // invoker.
            object result = innerOperationInvoker.InvokeEnd(instance, out outputs, asyncResult);

            // Save the instance using the storage manager saved in the 
            // current InstanceContext.
            InstanceContextExtension extension =
                OperationContext.Current.InstanceContext.Extensions.Find<InstanceContextExtension>();

            extension.StorageManager.SaveInstance(extension.ContextId, instance);
            return result;
        }

        public bool IsSynchronous
        {
            get { return innerOperationInvoker.IsSynchronous; }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    //This class contains the implementation of an IInstanceProvider
    //which provides the instances created by the storage manager.
    class InstanceProvider : IInstanceProvider
    {
        Type serviceType;

        public InstanceProvider(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            object instance = null;

            InstanceContextExtension extension =
                instanceContext.Extensions.Find<InstanceContextExtension>();

            string contextId = extension.ContextId;
            IStorageManager storageManager = extension.StorageManager;

            //Get the instance from the storage manager.
            instance = storageManager.GetInstance(contextId, serviceType);

            // If the storage manager returns null (which means that there is 
            // no instance available for the given context id) create a new 
            // instance and return it.
            if (instance == null)
            {
                instance = Activator.CreateInstance(serviceType);
            }

            return instance;
        }
    }
}


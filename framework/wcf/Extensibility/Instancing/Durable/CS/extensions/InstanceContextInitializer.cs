using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    //This class implements an IInstanceContextInitializer to 
    //hookup our custom InstanceContext extension to the instance 
    //context being constructed.
    class InstanceContextInitializer : IInstanceContextInitializer
    {
        IStorageManager storageManager;

        public InstanceContextInitializer(IStorageManager storageManager)
        {
            this.storageManager = storageManager;
        }

        public void Initialize(InstanceContext instanceContext, Message message)
        {
            // Read the context id from the Message.Properties collection. 
            // NOTE: This property is made available by our custom channel.
            string contextId =
                (string)message.Properties[DurableInstanceContextUtility.ContextIdProperty];

            InstanceContextExtension extension =
                new InstanceContextExtension(contextId, storageManager);
            instanceContext.Extensions.Add(extension);
        }
    }
}


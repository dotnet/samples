using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples
{
    class InstanceContextExtension : IExtension<InstanceContext>
    {
        string contextId;
        IStorageManager storageManager;

        public InstanceContextExtension(string contextId,
            IStorageManager storageManager)
        {
            this.contextId = contextId;
            this.storageManager = storageManager;

        }

        void IExtension<InstanceContext>.Attach(InstanceContext owner)
        {
            // Do not do anything here as we stick 
            // our state using the constructor.
        }

        void IExtension<InstanceContext>.Detach(InstanceContext owner)
        {
            // Do not do anything here as we do not 
            // have anything to clean when the extension
            // is being detached.
        }

        public string ContextId
        {
            get { return this.contextId; }
        }

        public IStorageManager StorageManager
        {
            get { return this.storageManager; }
        }
    }
}


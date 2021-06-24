#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

#endregion

namespace Microsoft.ServiceModel.Samples
{
     //This class contains the implementation of the attribute that 
     //can be used with the service class to enable 
     //durable instancing.
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DurableInstanceContextAttribute : Attribute, IServiceBehavior
    {
        #region Private members

        private Type storageManagerType;

        #endregion

        #region Constructors

        public DurableInstanceContextAttribute()
        {
        }

        #endregion

        #region Properties
        public Type StorageManagerType
        {
            get { return this.storageManagerType; }
            set { this.storageManagerType = value; }
        }

        #endregion        

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Durable instancing could not be used with 
            // singleton instancing.
            ServiceBehaviorAttribute serviceBehavior =
                serviceDescription.Behaviors.Find<ServiceBehaviorAttribute>();
            
            if (serviceBehavior != null &&
                serviceBehavior.InstanceContextMode == InstanceContextMode.Single)
            {
                throw new InvalidOperationException(
                    ResourceHelper.GetString("ExSingeltonInstancingNotSupported"));
            }

            // Use the StorageManagerFactory to create an instance of a 
            // storage manager.
            IStorageManager storageManager = 
                StorageManagerFactory.GetStorageManager(storageManagerType);
            
            InstanceContextInitializer contextInitializer =
                new InstanceContextInitializer(storageManager);

            InstanceProvider instanceProvider =
                new InstanceProvider(serviceDescription.ServiceType);

            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;

                if (cd != null)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceContextInitializers.Add(contextInitializer);
                        ed.DispatchRuntime.InstanceProvider = instanceProvider;
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {            
        }

        #endregion
    }
}


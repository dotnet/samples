//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System;

using System.ServiceModel.Dispatcher;

using System.ServiceModel.Description;

using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Collections.ObjectModel;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// This class contains the implementation for the attribute
    /// used to add pooling behavior to the service instance.
    /// </summary>
    public sealed class ObjectPoolingAttribute : Attribute, IServiceBehavior
    {
        #region Private Fields

        ObjectPoolInstanceProvider instanceProvider;
        int creationTimeout;
        int maxPoolSize;
        int minPoolSize;
        bool enabled;

        #endregion

        #region Constructor

        public ObjectPoolingAttribute()
        {
            instanceProvider = null;
            enabled = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object creation time out.
        /// </summary>
        public int CreationTimeout
        {
            get { return creationTimeout; }
            set { creationTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of objects 
        /// can be created in the pool.
        /// </summary>
        public int MaxPoolSize
        {
            get { return maxPoolSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(
                        ResourceHelper.GetString("ExNegetivePoolSize"));
                }

                if (value < minPoolSize)
                {
                    throw new ArgumentException(
                        ResourceHelper.GetString("ExLowMaxPool"));
                }

                maxPoolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum number of objects 
        /// created in the pool.
        /// </summary>
        public int MinPoolSize
        {
            get { return minPoolSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(
                        ResourceHelper.GetString("ExNegetivePoolSize"));
                }

                if (maxPoolSize > 0 && value > maxPoolSize)
                {
                    throw new ArgumentException(
                        ResourceHelper.GetString("ExHighMinPool"));
                }

                minPoolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the object 
        /// pooling is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        #endregion

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters) { }

        /// <summary>
        /// Performs the necessary actions to apply pooling behavior.
        /// </summary>        
        /// <remarks>
        /// This method is invoked by the runtime.
        /// </remarks>
        public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            if (enabled)
            {
                // Create an instance of the ObjectPoolInstanceProvider.
                instanceProvider = new ObjectPoolInstanceProvider(description.ServiceType,
                    maxPoolSize, minPoolSize, creationTimeout);

                // Assign our instance provider to Dispatch behavior in each 
                // endpoint.
                foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
                {
                    ChannelDispatcher cd = cdb as ChannelDispatcher;
                    if (cd != null)
                    {
                        foreach (EndpointDispatcher ed in cd.Endpoints)
                        {
                            ed.DispatchRuntime.InstanceProvider = instanceProvider;
                        }
                    }
                }
            }
        }

        public void Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            // Throw if the instance context mode is Single.
            ServiceBehaviorAttribute serviceBehavior =
                description.Behaviors.Find<ServiceBehaviorAttribute>();

            if (serviceBehavior != null &&
                serviceBehavior.InstanceContextMode == InstanceContextMode.Single)
            {
                throw new InvalidOperationException(
                    ResourceHelper.GetString("ExInvalidContext"));
            }

        }

        #endregion
    }
}

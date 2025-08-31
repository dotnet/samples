﻿//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{
    // This class contains the implementation for the attribute
    // used to add pooling behavior to the service instance.
    public sealed class ObjectPoolingAttribute : Attribute, IServiceBehavior
    {
        private const int DefaultMaxPoolSize = 32;
        private const int DefaultMinPoolSize = 0;
        private int _maxPoolSize = DefaultMaxPoolSize;
        private int _minPoolSize = DefaultMinPoolSize;

        // If the service does not already have a ServiceThrottlingBehavior, we will create
        // one and forward all IServiceBehavior calls to it.  This is used to implement
        // MaxPoolSize.
        private ServiceThrottlingBehavior throttlingBehavior = null;

        // Gets or sets the maximum number of objects that can be created in the pool
        public int MaxPoolSize
        {
            get => _maxPoolSize;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ResourceHelper.GetString("ExNegativePoolSize"));
                }

                _maxPoolSize = value;
            }
        }

        // Gets or sets the minimum number of objects that can be created in the pool
        public int MinPoolSize
        {
            get => _minPoolSize;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ResourceHelper.GetString("ExNegativePoolSize"));
                }

                _minPoolSize = value;
            }
        }

        #region IServiceBehavior Members

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
            // Forward the call if we created a ServiceThrottlingBehavior.
            if (throttlingBehavior != null)
            {
                ((IServiceBehavior)throttlingBehavior).AddBindingParameters(description, serviceHostBase, endpoints, parameters);
            }
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            // Create an instance of the ObjectPoolInstanceProvider.
            ObjectPoolingInstanceProvider instanceProvider = new ObjectPoolingInstanceProvider(description.ServiceType,
                                                                                               _minPoolSize);

            // Forward the call if we created a ServiceThrottlingBehavior.
            if (throttlingBehavior != null)
            {
                ((IServiceBehavior)throttlingBehavior).ApplyDispatchBehavior(description, serviceHostBase);
            }

            // In case there was already a ServiceThrottlingBehavior (throttlingBehavior==null),
            // it should have initialized a single ServiceThrottle on all ChannelDispatchers.  As
            // we loop through the ChannelDispatchers, we verify that and modify the ServiceThrottle
            // to guard MaxPoolSize.
            ServiceThrottle throttle = null;

            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    // Make sure there is exactly one throttle used by all endpoints.
                    // If there were others, we could not enforce MaxPoolSize.
                    if ((throttlingBehavior == null) && (_maxPoolSize != int.MaxValue))
                    {
                        if (throttle == null)
                        {
                            throttle = cd.ServiceThrottle;
                        }
                        if (cd.ServiceThrottle == null)
                        {
                            throw new InvalidOperationException(ResourceHelper.GetString("ExNullThrottle"));
                        }
                        if (throttle != cd.ServiceThrottle)
                        {
                            throw new InvalidOperationException(ResourceHelper.GetString("ExDifferentThrottle"));
                        }
                    }

                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        // Assign it to DispatchBehavior in each endpoint.
                        ed.DispatchRuntime.InstanceProvider = instanceProvider;
                    }
                }
            }

            // Set the MaxConcurrentInstances to limit the number of items that will
            // ever be requested from the pool.
            if ((throttle != null) && (throttle.MaxConcurrentInstances > _maxPoolSize))
            {
                throttle.MaxConcurrentInstances = _maxPoolSize;
            }
        }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            if (_maxPoolSize < _minPoolSize)
            {
                throw new InvalidOperationException(ResourceHelper.GetString("ExMinLargerThanMax"));
            }

            // throw if the instance context mode is Single
            ServiceBehaviorAttribute serviceBehavior = description.Behaviors.Find<ServiceBehaviorAttribute>();

            if (serviceBehavior != null &&
                serviceBehavior.InstanceContextMode == InstanceContextMode.Single)
            {
                throw new InvalidOperationException(ResourceHelper.GetString("ExInvalidContext"));
            }

            // We need ServiceThrottlingBehavior to run before us, because it properly
            // initializes the ServiceThrottle property of the endpoints.  If there is
            // no ServiceThrottlingBehavior, we will create one and run it ourselves.
            // If there is one, we validate that it comes before us.
            int throttlingIndex = GetBehaviorIndex(description, typeof(ServiceThrottlingBehavior));
            if (throttlingIndex == -1)
            {
                throttlingBehavior = new ServiceThrottlingBehavior();
                throttlingBehavior.MaxConcurrentInstances = MaxPoolSize;

                // Forward the call if we created a ServiceThrottlingBehavior.
                ((IServiceBehavior)throttlingBehavior).Validate(description, serviceHostBase);
            }
            else
            {
                int poolingIndex = GetBehaviorIndex(description, typeof(ObjectPoolingAttribute));
                if (poolingIndex < throttlingIndex)
                {
                    throw new InvalidOperationException(ResourceHelper.GetString("ExThrottleBeforePool"));
                }
            }
        }

        #endregion

        private int GetBehaviorIndex(ServiceDescription description, Type behaviorType)
        {
            for (int i = 0; i < description.Behaviors.Count; i++)
            {
                if (behaviorType.IsInstanceOfType(description.Behaviors[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}

//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Samples
{
    public static class CustomHeader
    {
        public const string HeaderName = "InstanceId";
        public const string HeaderNamespace = "http://Microsoft.ServiceModel.Samples/Lifetime";
    }

    /// <summary>
    /// This class contains the implementation for 
    /// custom lifetime lease. It implements 
    /// IShareableInstanceContextLifetime in order to be able 
    /// to attach to the service model layer.
    /// </summary>
    internal class CustomLifetimeLease : IInstanceContextProvider
    {
        #region Private Fields

        private readonly double _timeout;
        private bool _isIdle;
        private readonly Dictionary<string, InstanceContext> _instanceContextCache;

        // Lock must be acquired on this before
        // accessing the isIdle member.
        // ===============
        // VERY IMPORTANT: 
        // ===============
        // This is a simple approach to make it work  
        // with current API.
        // This approach is highly not acceptable as 
        // it will result in a considerable perf hit or 
        // even cause dead locks depending on how 
        // service model handles threads. 
        private readonly object _thisLock;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of CustomLifetimeLease class.
        /// </summary>
        public CustomLifetimeLease(double timeout)
        {
            _timeout = timeout;
            _thisLock = new object();
            _instanceContextCache = new Dictionary<string, InstanceContext>();
        }

        #endregion

        #region IInstanceContextProvider Members

        public bool IsIdle(InstanceContext instanceContext)
        {
            lock (_thisLock)
            {
                if (_isIdle)
                {
                    Utility.WriteMessageToConsole(
                        ResourceHelper.GetString("MsgIdle"));
                }
                else
                {
                    Utility.WriteMessageToConsole(
                        ResourceHelper.GetString("MsgNotIdle"));
                }

                bool idleCopy = _isIdle;
                _isIdle = false;
                return idleCopy;
            }
        }

        public void NotifyIdle(InstanceContextIdleCallback callback,
            InstanceContext instanceContext)
        {
            lock (_thisLock)
            {
                ICustomLease customLease =
                    instanceContext.Extensions.Find<ICustomLease>();

                customLease.Callback = callback;
                _isIdle = customLease.IsIdle;
                if (_isIdle)
                {
                    callback(instanceContext);
                }
            }
        }

        /// <summary>
        /// This implements a PerCall InstanceContextMode behavior. If a cached InstanceContext is not found
        /// then WCF will create a new one.
        /// </summary>     
        public InstanceContext GetExistingInstanceContext(System.ServiceModel.Channels.Message message, IContextChannel channel)
        {
            //Per Session behavior
            //To implement a PerSession behavior (If underlyin binding supports it) where in all 
            //methods from one ChannelFactory will be serviced by the same InstanceContext

            //Check if the incoming request has the InstanceContext id it wants to connect with.
            if (message.Headers.FindHeader(CustomHeader.HeaderName, CustomHeader.HeaderNamespace) != -1)
            {
                string sharingId = message.Headers.GetHeader<string>(CustomHeader.HeaderName, CustomHeader.HeaderNamespace);
                if (sharingId != null && _instanceContextCache.ContainsKey(sharingId))
                {
                    Utility.WriteMessageToConsole(string.Format(ResourceHelper.GetString("InstanceContextLookup"), sharingId));
                    //Retrieve the InstanceContext from the map
                    InstanceContext context = _instanceContextCache[sharingId];
                    if (context != null)
                    {
                        //Before returning, stop the timer on this InstanceContext
                        CustomLeaseExtension extension = context.Extensions.Find<CustomLeaseExtension>();
                        Utility.WriteMessageToConsole(string.Format(ResourceHelper.GetString("StopInstanceContextIdleTimer"), sharingId));
                        extension.StopTimer();

                        Utility.WriteMessageToConsole(ResourceHelper.GetString("CachedInstanceContextFound"));
                        return _instanceContextCache[sharingId];
                    }
                }
            }

            //No existing InstanceContext was found so return null and WCF will create a new one.
            return null;
        }

        public void InitializeInstanceContext(InstanceContext instanceContext, System.ServiceModel.Channels.Message message, IContextChannel channel)
        {
            //Look if the Client has given us a unique ID to add to this InstanceContext
            int headerIndex = message.Headers.FindHeader(CustomHeader.HeaderName, CustomHeader.HeaderNamespace);
            string headerId = null;
            if (headerIndex != -1)
            {
                headerId = message.Headers.GetHeader<string>(headerIndex);
            }

            if (headerId == null)
            {
                //If no header was sent by the Client, then create a new one and assign it to this InstanceContext.
                headerId = Guid.NewGuid().ToString();
                Utility.WriteMessageToConsole(string.Format(ResourceHelper.GetString("NoHeaderFound")));
            }

            Utility.WriteMessageToConsole(string.Format(ResourceHelper.GetString("InstanceContextAddedToCache"), headerId));

            //Add this to the Cache
            _instanceContextCache[headerId] = instanceContext;

            //Register the Closing event of this InstancContext so it can be removed from the collection
            instanceContext.Closing += RemoveInstanceContext;

            IExtension<InstanceContext> customLeaseExtension =
                new CustomLeaseExtension(_timeout, headerId);
            instanceContext.Extensions.Add(customLeaseExtension);
        }

        public void RemoveInstanceContext(object o, EventArgs args)
        {
            InstanceContext context = o as InstanceContext;
            CustomLeaseExtension extension = context.Extensions.Find<CustomLeaseExtension>();
            string id = (extension != null) ? extension.InstanceId : null;
            if (_instanceContextCache[id] != null)
            {
                Utility.WriteMessageToConsole(string.Format(ResourceHelper.GetString("InstanceContextRemovedFromCache"), id));
                _instanceContextCache.Remove(id);
            }
        }
        #endregion
    }
}

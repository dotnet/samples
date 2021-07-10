//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Timers;
using Timer = System.Timers.Timer;
using System.ServiceModel.Dispatcher;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ServiceModel.Samples
{
    public static class CustomHeader
    {
        public static readonly String HeaderName = "InstanceId";
        public static readonly String HeaderNamespace = "http://Microsoft.ServiceModel.Samples/Lifetime";
    }

    /// <summary>
    /// This class contains the implementation for 
    /// custom lifetime lease. It implements 
    /// IShareableInstanceContextLifetime in order to be able 
    /// to attach to the service model layer.
    /// </summary>
    class CustomLifetimeLease : IInstanceContextProvider
    {
        #region Private Fields
        
        double timeout;
        bool isIdle;
        Dictionary<String, InstanceContext> instanceContextCache;

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
        object thisLock;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of CustomLifetimeLease class.
        /// </summary>
        public CustomLifetimeLease(double timeout)
        {
            this.timeout = timeout;
            thisLock = new object();
            this.instanceContextCache = new Dictionary<String, InstanceContext>();
        }

        #endregion

        #region IInstanceContextProvider Members

        public bool IsIdle(InstanceContext instanceContext)
        {
            lock (thisLock)
            {
                if (isIdle)
                {
                    Utility.WriteMessageToConsole(
                        ResourceHelper.GetString("MsgIdle"));
                }
                else
                {
                    Utility.WriteMessageToConsole(
                        ResourceHelper.GetString("MsgNotIdle"));
                }

                bool idleCopy = isIdle;
                isIdle = false;
                return idleCopy;
            }
        }

        public void NotifyIdle(InstanceContextIdleCallback callback, 
            InstanceContext instanceContext)
        {
            lock (thisLock)
            {
                ICustomLease customLease =
                    instanceContext.Extensions.Find<ICustomLease>();

                customLease.Callback = callback;                
                isIdle = customLease.IsIdle;
                if (isIdle)
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
                String sharingId = message.Headers.GetHeader<string>(CustomHeader.HeaderName, CustomHeader.HeaderNamespace);
                if (sharingId != null && instanceContextCache.ContainsKey(sharingId))
                {
                    Utility.WriteMessageToConsole(String.Format(ResourceHelper.GetString("InstanceContextLookup"),sharingId));
                    //Retrieve the InstanceContext from the map
                    InstanceContext context = instanceContextCache[sharingId];
                    if (context != null)
                    {
                        //Before returning, stop the timer on this InstanceContext
                        CustomLeaseExtension extension = context.Extensions.Find<CustomLeaseExtension>();
                        Utility.WriteMessageToConsole(String.Format(ResourceHelper.GetString("StopInstanceContextIdleTimer"), sharingId));
                        extension.StopTimer();

                        Utility.WriteMessageToConsole(ResourceHelper.GetString("CachedInstanceContextFound"));
                        return instanceContextCache[sharingId];
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
            String headerId = null;
            if (headerIndex != -1)
            {
                headerId = message.Headers.GetHeader<string>(headerIndex);
            }

            if (headerId == null)
            {
                //If no header was sent by the Client, then create a new one and assign it to this InstanceContext.
                headerId = Guid.NewGuid().ToString();
                Utility.WriteMessageToConsole(String.Format(ResourceHelper.GetString("NoHeaderFound")));
            }

            Utility.WriteMessageToConsole(String.Format(ResourceHelper.GetString("InstanceContextAddedToCache"), headerId));

            //Add this to the Cache
            this.instanceContextCache[headerId] = instanceContext;

            //Register the Closing event of this InstancContext so it can be removed from the collection
            instanceContext.Closing += this.RemoveInstanceContext;

            IExtension<InstanceContext> customLeaseExtension =
                new CustomLeaseExtension(timeout, headerId);
            instanceContext.Extensions.Add(customLeaseExtension);            
        }

        public void RemoveInstanceContext(object o, EventArgs args)
        {
            InstanceContext context = o as InstanceContext;
            CustomLeaseExtension extension = context.Extensions.Find<CustomLeaseExtension>();
            String id = (extension != null) ? extension.InstanceId : null;
            if (this.instanceContextCache[id] != null)
            {
                Utility.WriteMessageToConsole(String.Format(ResourceHelper.GetString("InstanceContextRemovedFromCache"), id));
                this.instanceContextCache.Remove(id);
            }
        }
        #endregion
    }
}

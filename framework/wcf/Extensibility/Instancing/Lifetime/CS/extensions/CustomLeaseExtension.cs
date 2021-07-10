//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Timers;
using Timer = System.Timers.Timer;
using System.ServiceModel.Dispatcher;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    interface ICustomLease
    {
        bool IsIdle { get; }        
        InstanceContextIdleCallback Callback { get; set; }
    }

    /// <summary>
    /// This class contains the implementation of an extension to InstanceContext. 
    /// This enables extended lifetime for the InstanceContext.
    /// </summary>
    class CustomLeaseExtension : IExtension<InstanceContext>, ICustomLease
    {
        #region Private Fields

        // Reference to the InstanceContext instance owns this 
        // extension instance.
        InstanceContext owner;
        
        bool isIdle;
        object thisLock;
        Timer idleTimer;
        double idleTimeout;
        InstanceContextIdleCallback callback;
        string instanceId;

        public String InstanceId
        {
            get
            {
                return this.instanceId;
            }
        }

        #endregion

        #region Constructor

        public CustomLeaseExtension(double idleTimeout, String instanceId)
        {
            owner = null;
            isIdle = false;
            thisLock = new object();
            idleTimer = new Timer();
            this.idleTimeout = idleTimeout;
            this.instanceId = instanceId;
        }

        #endregion

        #region IExtension<InstanceContext> Members

        /// <summary>
        /// Attaches this extension to current instance of 
        /// InstanceContext. 
        /// </summary>       
        /// <remarks>
        /// This method is called by WCF at the time it attaches this
        /// extension.
        /// </remarks>
        public void Attach(InstanceContext owner)
        {
            this.owner = owner;            
        }

        public void Detach(InstanceContext owner)
        {            
        }

        #endregion

        #region ICustomLease Members

        /// <summary>
        /// Gets or sets a value indicating whether this 
        /// InstanceContext is idle or not.
        /// </summary>
        public bool IsIdle
        {
            get
            {
                lock (thisLock)
                {
                    if (isIdle)
                    {
                        return true;
                    }
                    else
                    {
                        StartTimer();
                        return false;
                    }
                }
            }
        }
     
        /// <summary>
        /// Gets or sets the InstanceContextIdleCallback.
        /// </summary>
        public InstanceContextIdleCallback Callback
        {
            get
            {
                lock (thisLock)
                {
                    return callback;
                }
            }
            set
            {
                // Immutable state.
                if (idleTimer.Enabled)
                {
                    throw new InvalidOperationException(
                        ResourceHelper.GetString("ExCannotChangeCallback"));
                }

                lock (thisLock)
                {
                    callback = value;
                }
            }
        }

        #endregion

        #region Helper members

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void StartTimer()
        {
            lock (thisLock)
            {
                idleTimer.Interval = idleTimeout;
                idleTimer.Elapsed += new ElapsedEventHandler(idleTimer_Elapsed);

                if (!idleTimer.Enabled)
                {
                    idleTimer.Start();
                }
            }
        }

        public void StopTimer()
        {
            lock (thisLock)
            {
                if (idleTimer.Enabled)
                {
                    idleTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Timer elapsed event handler.
        /// </summary>        
        void idleTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            lock (thisLock)
            {
                StopTimer();
                isIdle = true;
                Utility.WriteMessageToConsole(
                    ResourceHelper.GetString("MsgLeaseExpired"));
                callback(owner);
            }
        }        

        #endregion
       
    }
}

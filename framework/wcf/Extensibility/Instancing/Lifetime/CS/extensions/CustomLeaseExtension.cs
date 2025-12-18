//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Timers;
using Timer = System.Timers.Timer;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    internal interface ICustomLease
    {
        bool IsIdle { get; }
        InstanceContextIdleCallback Callback { get; set; }
    }

    /// <summary>
    /// This class contains the implementation of an extension to InstanceContext. 
    /// This enables extended lifetime for the InstanceContext.
    /// </summary>
    internal class CustomLeaseExtension : IExtension<InstanceContext>, ICustomLease
    {
        #region Private Fields

        // Reference to the InstanceContext instance owns this 
        // extension instance.
        private InstanceContext _owner;
        private bool _isIdle;
        private readonly object _thisLock;
        private readonly Timer _idleTimer;
        private readonly double _idleTimeout;
        private InstanceContextIdleCallback _callback;

        public string InstanceId { get; }

        #endregion

        #region Constructor

        public CustomLeaseExtension(double idleTimeout, string instanceId)
        {
            _owner = null;
            _isIdle = false;
            _thisLock = new object();
            _idleTimer = new Timer();
            _idleTimeout = idleTimeout;
            InstanceId = instanceId;
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
        public void Attach(InstanceContext owner) => _owner = owner;

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
                lock (_thisLock)
                {
                    if (_isIdle)
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
                lock (_thisLock)
                {
                    return _callback;
                }
            }
            set
            {
                // Immutable state.
                if (_idleTimer.Enabled)
                {
                    throw new InvalidOperationException(
                        ResourceHelper.GetString("ExCannotChangeCallback"));
                }

                lock (_thisLock)
                {
                    _callback = value;
                }
            }
        }

        #endregion

        #region Helper members

        /// <summary>
        /// Starts the timer.
        /// </summary>
        private void StartTimer()
        {
            lock (_thisLock)
            {
                _idleTimer.Interval = _idleTimeout;
                _idleTimer.Elapsed += new ElapsedEventHandler(idleTimer_Elapsed);

                if (!_idleTimer.Enabled)
                {
                    _idleTimer.Start();
                }
            }
        }

        public void StopTimer()
        {
            lock (_thisLock)
            {
                if (_idleTimer.Enabled)
                {
                    _idleTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Timer elapsed event handler.
        /// </summary>        
        private void idleTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            lock (_thisLock)
            {
                StopTimer();
                _isIdle = true;
                Utility.WriteMessageToConsole(
                    ResourceHelper.GetString("MsgLeaseExpired"));
                _callback(_owner);
            }
        }

        #endregion

    }
}

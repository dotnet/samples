//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Microsoft.ServiceModel.Samples
{
    // This class contains the implementation for the object pool. This implements the 
    // IInstanceProvider interface in order to be able to plugin to the dispatcher layer.
    internal class ObjectPoolingInstanceProvider : IInstanceProvider
    {
        private const int IdleTimeout = 5 * 60 * 1000;     // 5 minutes

        #region Private Fields

        // Minimum number of objects in the pool
        private readonly int _minPoolSize;

        // Type of the object created in the pool
        private readonly Type _instanceType;

        // Stack used for storing objects in the pool
        private readonly Stack<object> _pool;

        // Lock should acquired before accessing the pool stack
        private readonly object _poolLock = new object();

        // Keeps track of the number of objects returned from the pool.
        private int _activeObjectsCount;

        // Timer object used to trigger the clean up process
        // after a given period of idle time.
        private readonly Timer _idleTimer;

        #endregion

        public ObjectPoolingInstanceProvider(Type instanceType, int minPoolSize)
        {
            _minPoolSize = minPoolSize;
            _instanceType = instanceType;

            _pool = new Stack<object>();
            _activeObjectsCount = 0;

            // Initialize the timer and subscribe to the "Elapsed" event
            _idleTimer = new Timer(IdleTimeout);
            _idleTimer.Elapsed += new System.Timers.ElapsedEventHandler(idleTimer_Elapsed);

            // Initialize the minimum number of objects if possible
            Initialize();
        }

        #region IInstanceProvider Members

        object IInstanceProvider.GetInstance(InstanceContext instanceContext) => ((IInstanceProvider)this).GetInstance(instanceContext, null);

        object IInstanceProvider.GetInstance(InstanceContext instanceContext, Message message)
        {
            object obj = null;

            lock (_poolLock)
            {
                if (_pool.Count > 0)
                {
                    obj = _pool.Pop();
                }
                else
                {
                    obj = CreateNewPoolObject();
                }
                _activeObjectsCount++;
            }

            WritePoolMessage(ResourceHelper.GetString("MsgNewObject"));

            _idleTimer.Stop();

            return obj;
        }

        void IInstanceProvider.ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            lock (_poolLock)
            {
                _pool.Push(instance);
                _activeObjectsCount--;

                WritePoolMessage(ResourceHelper.GetString("MsgObjectPooled"));

                // When the service goes completely idle (no requests are being processed),
                // the idle timer is started
                if (_activeObjectsCount == 0)
                    _idleTimer.Start();
            }
        }

        #endregion

        // Initialize the pool with minimum number of instances
        private void Initialize()
        {
            for (int i = 0; i < _minPoolSize; i++)
            {
                _pool.Push(CreateNewPoolObject());
            }
        }

        // Handles the instantiation of the types created by this instance
        private object CreateNewPoolObject() => Activator.CreateInstance(_instanceType);

        // Clean up procedure
        private void idleTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            _idleTimer.Stop();

            lock (_poolLock)
            {
                if (_activeObjectsCount == 0)
                {
                    while (_pool.Count > _minPoolSize)
                    {
                        WritePoolMessage(ResourceHelper.GetString("MsgObjectRemoving"));

                        object removedItem = _pool.Pop();

                        if (removedItem is IDisposable)
                        {
                            ((IDisposable)removedItem).Dispose();
                        }
                    }
                }
            }
        }

        // Writes a given message to the console in red color
        private void WritePoolMessage(string message)
        {
            ConsoleColor currentForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = currentForegroundColor;
        }
    }
}

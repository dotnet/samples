//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Microsoft.ServiceModel.Samples
{
    // This class contains the implementation for the object pool. This implements the 
    // IInstanceProvider interface in order to be able to plugin to the dispatcher layer.
    class ObjectPoolingInstanceProvider : IInstanceProvider
    {
        const int idleTimeout = 5 * 60 * 1000;     // 5 minutes

        #region Private Fields

        // Minimum number of objects in the pool
        int minPoolSize;
        
        // Type of the object created in the pool
        Type instanceType;

        // Stack used for storing objects in the pool
        Stack<object> pool;
        
        // Lock should acquired before accessing the pool stack
        object poolLock = new object();
        
        // Keeps track of the number of objects returned from the pool.
        int activeObjectsCount;

        // Timer object used to trigger the clean up process
        // after a given period of idle time.
        Timer idleTimer;
        
        #endregion

        public ObjectPoolingInstanceProvider(Type instanceType, int minPoolSize)
        {            
            this.minPoolSize = minPoolSize;
            this.instanceType = instanceType;

            pool = new Stack<object>();
            activeObjectsCount = 0;

            // Initialize the timer and subscribe to the "Elapsed" event
            idleTimer = new Timer(idleTimeout);
            idleTimer.Elapsed += new System.Timers.ElapsedEventHandler(idleTimer_Elapsed);

            // Initialize the minimum number of objects if possible
            Initialize();
        }

        #region IInstanceProvider Members

        object IInstanceProvider.GetInstance(InstanceContext instanceContext)
        {
            return ((IInstanceProvider)this).GetInstance(instanceContext, null);
        }

        object IInstanceProvider.GetInstance(InstanceContext instanceContext, Message message)
        {
            object obj = null;

            lock (poolLock)
            {
                if (pool.Count > 0)
                {
                    obj = pool.Pop();
                }
                else
                {
                    obj = CreateNewPoolObject();
                }
                activeObjectsCount++;
            }

            WritePoolMessage(ResourceHelper.GetString("MsgNewObject"));

            idleTimer.Stop();

            return obj;          
        }

        void IInstanceProvider.ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            lock (poolLock)
            {
                pool.Push(instance);
                activeObjectsCount--;

                WritePoolMessage(ResourceHelper.GetString("MsgObjectPooled"));

                // When the service goes completely idle (no requests are being processed),
                // the idle timer is started
                if (activeObjectsCount == 0)
                    idleTimer.Start();                     
            }
        }
        
        #endregion

        // Initialize the pool with minimum number of instances
        void Initialize()
        {
            for (int i = 0; i < minPoolSize; i++)
            {
                pool.Push(CreateNewPoolObject());
            }
        }

        // Handles the instantiation of the types created by this instance
        private object CreateNewPoolObject()
        {
            return Activator.CreateInstance(instanceType);            
        }

        // Clean up procedure
        void idleTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            idleTimer.Stop();

            lock (poolLock)
            {
                if (activeObjectsCount == 0)
                {
                    while (pool.Count > minPoolSize)
                    {
                        WritePoolMessage(ResourceHelper.GetString("MsgObjectRemoving"));
                        
                        object removedItem = pool.Pop();
                        
                        if (removedItem is IDisposable)
                        {
                            ((IDisposable)removedItem).Dispose();
                        }
                    }
                }                
            }
        }

        // Writes a given message to the console in red color
        void WritePoolMessage(string message)
        {
            ConsoleColor currentForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = currentForegroundColor;
        }
    }
}

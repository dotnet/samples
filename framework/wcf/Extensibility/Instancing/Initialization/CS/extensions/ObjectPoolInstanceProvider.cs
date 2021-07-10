//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// This class contains the implementation for the 
    /// object pool. This implements the IInstanceProvider 
    /// in order to be able to plugin to the service model
    /// layer.
    /// </summary>
    class ObjectPoolInstanceProvider : IInstanceProvider
    {
        #region Constants

        // Five minutes.
        // Q: What's the default idle timeout 
        // in ES?
        const long idleTimeout = 1000; //300000 * 5;
        const int defaultMaxPoolSize = 1048576;
        const int defaultCreationTimeout = 60000;
        const int defaultMinPoolSize = 0;

        #endregion

        #region Private Fields

        int maxPoolSize;
        int minPoolSize;
        int creationTimeout;
        
        // Type of the objects created in the pool.
        Type instanceType;

        // Stack used for storing the objects in the pool.
        Stack<object> pool;

        // Semaphore used to synchronize access to 
        // the pool.
        Semaphore availableCount;

        // A lock should be acquired before accessing the         
        // pool stack.
        object poolLock = new object();
        
        // Keeps track of the number of objects returned 
        // from the pool.
        int activeObjectsCount;

        // Timer object used to trigger the clean up 
        // procedure after a given period of idle time.
        Timer idleTimer;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the 
        /// ObjectPoolInstanceProvider class.
        /// </summary>
        /// <param name="instanceType">
        /// Type of the objects created in the pool.
        /// </param>
        /// <param name="maxPoolSize">
        /// Maximum number of objects can be created
        /// in the pool.
        /// </param>
        /// <param name="minPoolSize">
        /// Minimum number of objects to be created, when
        /// initializing the pool.
        /// </param>
        /// <param name="creationTimeout">
        /// Object creation timeout value.
        /// </param>
        public ObjectPoolInstanceProvider(Type instanceType, int maxPoolSize, 
            int minPoolSize, int creationTimeout)
        {            
            this.maxPoolSize = maxPoolSize;
            this.minPoolSize = minPoolSize;
            this.creationTimeout = creationTimeout;
            this.instanceType = instanceType;
            
            // Verify the constructor args and 
            // apply the default values if applicable.
            VerifyAttributes();

            pool = new Stack<object>();
            availableCount = new Semaphore(this.maxPoolSize, this.maxPoolSize);
            
            // Initialize the timer and subscribe to 
            // the "Elapsed" event.
            idleTimer = new Timer(idleTimeout);
            idleTimer.Elapsed += 
                new System.Timers.ElapsedEventHandler(idleTimer_Elapsed);

            // Initialize the minmum number of objects 
            // requested.
            Initialize();
        }

        #endregion

        #region IInstanceProvider Members

        /// <summary>
        /// Returns an object from the pool.
        /// </summary>
        /// <remarks>
        /// This method is invoked by WCF runtime.
        /// </remarks>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return GetObjectFromThePool();
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }
        
        /// <summary>
        /// Puts an object back to the pool.
        /// </summary>
        /// <remarks>
        /// This method is invoked by WCF runtime.
        /// </remarks>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            lock (poolLock)
            {
                // Check whether the object can be pooled. 
                // Call the Deactivate method if possible.
                if (instance is IObjectControl)
                {
                    IObjectControl objectControl = (IObjectControl)instance;
                    objectControl.Deactivate();

                    if (objectControl.CanBePooled)
                    {
                        pool.Push(instance);

                        #if(DEBUG)
                        WritePoolMessage(
                            ResourceHelper.GetString("MsgObjectPooled"));
                        #endif                        
                    }
                    else
                    {
                        #if(DEBUG)
                        WritePoolMessage(
                            ResourceHelper.GetString("MsgObjectWasNotPooled"));
                        #endif
                    }
                }
                else
                {
                    pool.Push(instance);

                    #if(DEBUG)
                    WritePoolMessage(
                        ResourceHelper.GetString("MsgObjectPooled"));
                    #endif 
                }
                                
                activeObjectsCount--;

                if (activeObjectsCount == 0)
                {
                    idleTimer.Start();                     
                }
            }

            availableCount.Release(1);
        }
        
        #endregion

        #region Helper Methods

        /// <summary>
        /// Initialize th minimum number of instances.
        /// </summary>
        private void Initialize()
        {
            for (int i = 0; i < minPoolSize; i++)
            {
                pool.Push(CreateNewPoolObject());
            }
        }

        /// <summary>
        /// Handles the instantiation of the types created by this 
        /// ObjectPool instance.
        /// </summary>        
        private object CreateNewPoolObject()
        {
            return Activator.CreateInstance(instanceType);            
        }

        /// <summary>
        /// Implements the logic for returning an object
        /// from the pool.
        /// </summary>        
        private object GetObjectFromThePool()
        {
            bool didNotTimeout = 
                availableCount.WaitOne(creationTimeout, true);
            
            if(didNotTimeout)
            {
                object obj = null;

                lock (poolLock)
                {
                    if (pool.Count != 0)
                    {
                        obj = pool.Pop();
                        activeObjectsCount++;
                    }
                    else if (pool.Count == 0)
                    {
                        if (activeObjectsCount < maxPoolSize)
                        {
                            obj = CreateNewPoolObject();
                            activeObjectsCount++;
                            
                            #if (DEBUG)
                            WritePoolMessage(
                                ResourceHelper.GetString("MsgNewObject"));
                            #endif
                        }                        
                    }

                    idleTimer.Stop();
                }

                // Call the Activate method if possible.
                if (obj is IObjectControl)
                {
                    ((IObjectControl)obj).Activate();
                }

                return obj;
            }

            throw new TimeoutException(
                ResourceHelper.GetString("ExObjectCreationTimeout"));
        }

        /// <summary>
        /// Clean up procedure.
        /// </summary>        
        void idleTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            idleTimer.Stop();

            lock (poolLock)
            {
                if (activeObjectsCount == 0)
                {
                    // Remove the surplus objects.
                    if (pool.Count > minPoolSize)
                    {
                        while (pool.Count != minPoolSize)
                        {
                            #if(DEBUG)

                            WritePoolMessage(
                                ResourceHelper.GetString("MsgObjectRemoving"));
                            
                            #endif
                            
                            object removedItem = pool.Pop();
                            
                            if (removedItem is IDisposable)
                            {
                                ((IDisposable)removedItem).Dispose();
                            }
                        }
                    }                    
                    else if (pool.Count < minPoolSize)
                    {
                        // Reinitialize the missing objects.
                        while(pool.Count != minPoolSize)
                        {
                            pool.Push(CreateNewPoolObject());
                        }
                    }
                }                
            }            
        }

        /// <summary>
        /// Writes a given message to the console
        /// in red color.
        /// </summary>        
        void WritePoolMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Verifies the attribute values and 
        /// applies the defaults if applicable.
        /// </summary>
        void VerifyAttributes()
        {
            if (this.maxPoolSize == 0)
            {
                this.maxPoolSize = defaultMaxPoolSize;
            }

            if (this.minPoolSize == 0)
            {
                this.minPoolSize = defaultMinPoolSize;
            }

            if (this.creationTimeout == 0)
            {
                this.creationTimeout = defaultCreationTimeout;
            }
        }

        #endregion
    }
}

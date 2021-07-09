
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace Microsoft.ServiceModel.Samples.Activation
{
    [DataContract]
    class AppInstance
    {
        static int currentId = 1;

        [DataMember]
        int id;

        [DataMember]
        string appKey;
        public AppInstance(string appKey)
        {
            id = Interlocked.Increment(ref currentId);
            this.appKey = appKey;
        }

        public int Id
        {
            get
            {
                return this.id;
            }
        }

        public string AppKey
        {
            get
            {
                return this.appKey;
            }
        }

        public byte[] Serialize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(AppInstance));
                serializer.WriteObject(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static AppInstance Deserialize(byte[] blob)
        {
            using (MemoryStream memoryStream = new MemoryStream(blob))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(AppInstance));
                return (AppInstance)serializer.ReadObject(memoryStream);
            }
        }
    }

    class App
    {
        object syncRoot = new object();
        AppInstance instance;
        string appPoolId;
        string path;
        string appKey;
        InputQueue<RequestContext> messageQueue;

        public App(string appKey, string path, int siteId, string appPoolId)
        {
            this.path = path;
            this.appPoolId = appPoolId;
            this.appKey = appKey;
            this.messageQueue = new InputQueue<RequestContext>();
        }

        public string Path
        {
            get
            {
                return path;
            }
        }

        public string AppPoolId
        {
            get
            {
                return appPoolId;
            }
        }

        public void EnsureStarted(out bool firstStart)
        {
            firstStart = false;
            if (instance != null)
                return;

            lock (ThisLock)
            {
                if (instance != null)
                {
                    return;
                }

                instance = new AppInstance(this.appKey);
                firstStart = true;
            }
        }

        public AppInstance Instance
        {
            get
            {
                return this.instance;
            }
        }

        public void EnqueueAndDispatch(FramingData data)
        {
            // Schedule message dispatch
            RequestContext context = new RequestContext(data);
            messageQueue.EnqueueAndDispatch(context, context.OnContextDequeued, false);

            // Wait for the message to be dequeued.
            context.OnContextEnqueued();
        }

        public IAsyncResult BeginDequeue()
        {
            return messageQueue.BeginDequeue(TimeSpan.MaxValue, null, null);
        }

        public FramingData EndDequeue(IAsyncResult result)
        {
            RequestContext context;
            messageQueue.EndDequeue(result, out context);
            return context.Data;
        }

        object ThisLock
        {
            get
            {
                return syncRoot;
            }
        }

        class RequestContext : IDisposable
        {
            ManualResetEvent contextDequeued;
            FramingData data;
            public RequestContext(FramingData data)
            {
                this.data = data;
                this.contextDequeued = new ManualResetEvent(false);
            }

            public void OnContextDequeued()
            {
                contextDequeued.Set();
            }

            public void OnContextEnqueued()
            {
                contextDequeued.WaitOne();
            }

            public FramingData Data
            {
                get
                {
                    return this.data;
                }
            }

            public void Dispose()
            {
                contextDequeued.Close();
            }
        }
    }

    class AppManager
    {
        Dictionary<string, App> apps;

        public AppManager()
        {
            apps = new Dictionary<string, App>(StringComparer.OrdinalIgnoreCase);
        }

        public App CreateApp(string appKey, string path, int siteId, string appPoolId)
        {
            lock (apps)
            {
                if (apps.Count > 0)
                {
                    throw new NotSupportedException("Sorry that we only allow one application to be activated.");
                }

                App app = new App(appKey, path, siteId, appPoolId);
                this.apps.Add(appKey, app);
                return app;
            }
        }
    }
}


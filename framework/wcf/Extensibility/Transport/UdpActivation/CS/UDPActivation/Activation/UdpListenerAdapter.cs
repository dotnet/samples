
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.ServiceModel.Samples.Activation
{
    public class UdpListenerAdapter : CommunicationObject
    {
        static UdpListenerAdapter singleton;
        WebhostListenerCallbacks webHostCallbacks;
        int protocolHandle;
        ManualResetEvent initializedEvent;
        ServiceHost serviceHost;
        UriLookupTable<App> appQueue;
        AppManager appManager;
        UdpListenerManager listenerManager;

        UdpListenerAdapter()
        {
            webHostCallbacks = new WebhostListenerCallbacks();
            webHostCallbacks.dwBytesInCallbackStructure = Marshal.SizeOf(webHostCallbacks);
         
            webHostCallbacks.applicationAppPoolChanged = new WCB.ApplicationAppPoolChanged(OnApplicationAppPoolChanged);
            webHostCallbacks.applicationBindingsChanged = new WCB.ApplicationBindingsChanged(OnApplicationBindingsChanged);
            webHostCallbacks.applicationCreated = new WCB.ApplicationCreated(OnApplicationCreated);
            webHostCallbacks.applicationDeleted = new WCB.ApplicationDeleted(OnApplicationDeleted);
            webHostCallbacks.applicationPoolAllListenerChannelInstancesStopped = new WCB.ApplicationPoolAllListenerChannelInstancesStopped(OnApplicationPoolAllListenerChannelInstancesStopped);
            webHostCallbacks.applicationPoolCanOpenNewListenerChannelInstance = new WCB.ApplicationPoolCanOpenNewListenerChannelInstance(OnApplicationPoolCanOpenNewListenerChannelInstance);
            webHostCallbacks.applicationPoolCreated = new WCB.ApplicationPoolCreated(OnApplicationPoolCreated);
            webHostCallbacks.applicationPoolDeleted = new WCB.ApplicationPoolDeleted(OnApplicationPoolDeleted);
            webHostCallbacks.applicationPoolIdentityChanged = new WCB.ApplicationPoolIdentityChanged(OnApplicationPoolIdentityChanged);
            webHostCallbacks.applicationPoolStateChanged = new WCB.ApplicationPoolStateChanged(OnApplicationPoolStateChanged);
            webHostCallbacks.applicationRequestsBlockedChanged = new WCB.ApplicationRequestsBlockedChanged(OnApplicationRequestsBlockedChanged);
            webHostCallbacks.configManagerConnected = new WCB.ConfigManagerConnected(OnConfigManagerConnected);
            webHostCallbacks.configManagerDisconnected = new WCB.ConfigManagerDisconnected(OnConfigManagerDisconnected);
            webHostCallbacks.configManagerInitializationCompleted = new WCB.ConfigManagerInitializationCompleted(OnConfigManagerInitializationCompleted);

            initializedEvent = new ManualResetEvent(false);
            appManager = new AppManager();
            appQueue = new UriLookupTable<App>();
            listenerManager = new UdpListenerManager(new DataReceivedCallback(OnDataReceived));
        }

        public static void Start()
        {
            if (singleton != null)
            {
                throw new InvalidOperationException("Only one instance of UdpListenerAdapter is allowed.");
            }

            singleton = new UdpListenerAdapter();
            singleton.Open();
        }

        public static void Stop()
        {
            if (singleton == null)
            {
                throw new InvalidOperationException("The UdpListenerAdapter is not started.");
            }

            singleton.Close();
            singleton = null;
        }

        void OnDataReceived(FramingData data)
        {
            Uri uri = data.To;
            App app = appQueue.Lookup(uri);
            if (app == null)
            {
                // Not found
                return;
            }

            lock (ThisLock)
            {
                bool firstStart;
                app.EnsureStarted(out firstStart);

                if (firstStart)
                {
                    WasHelper.OpenListenerChannelInstance(this.protocolHandle,
                        app.AppPoolId, app.Instance.Id, app.Instance.Serialize());
                }

                app.EnqueueAndDispatch(data);
            }
        }

        internal static void Register(ControlRegistrationData data, UdpControlService serviceInstance)
        {
            singleton.RegisterInternal(data, serviceInstance);
        }

        internal static void Dispatch(ControlRegistrationData data, UdpControlService serviceInstance)
        {
            singleton.RegisterInternal(data, serviceInstance);
        }

        void RegisterInternal(ControlRegistrationData data, UdpControlService serviceInstance)
        {
            if (this.State != CommunicationState.Opened)
            {
                return;
            }

            App app = appQueue.Lookup(data.Uri);
            if (app == null)
            {
                throw new InvalidOperationException("Application does not exist.");
            }

            if (app.Instance.Id != data.InstanceId)
            {
                throw new InvalidOperationException("Application does not have instance of such id.");
            }

            serviceInstance.SetApp(app);
            ThreadPool.QueueUserWorkItem(new WaitCallback(OnNewServiceInstanceAvailable), serviceInstance);
        }

        void OnNewServiceInstanceAvailable(object state)
        {
            UdpControlService serviceInstance = (UdpControlService)state;
            serviceInstance.Dispatch();
        }

        void StartListen(App app, int port)
        {
            // FUTURE: support host configured in the binding as HTTP case.
            UriBuilder builder = new UriBuilder(UdpConstants.Scheme, "localhost", port, app.Path);
            Uri uri = builder.Uri;

            IPAddress address = IPAddress.Broadcast;
            if (uri.HostNameType == UriHostNameType.IPv4 || uri.HostNameType == UriHostNameType.IPv6)
            {
                address = IPAddress.Parse(uri.DnsSafeHost);
            }

            listenerManager.Listen(address, uri.Port);

            appQueue.Add(uri, app);
        }

        string[] ParseBindings(IntPtr bindingsMultiSz, int numberOfBindings)
        {
            if (bindingsMultiSz == IntPtr.Zero)
            {
                throw new ArgumentNullException("bindingsMultiSz");
            }

            if (numberOfBindings < 0)
            {
                throw new ArgumentException("numberOfBindings");
            }

            string[] bindings = new string[numberOfBindings];
            IntPtr bindingsBufferPtr = bindingsMultiSz;
            for (int i = 0; i < numberOfBindings; i++)
            {
                string bindingString = Marshal.PtrToStringUni(bindingsBufferPtr);
                if (string.IsNullOrEmpty(bindingString))
                {
                    throw new ArgumentException("bindingsMultiSz");
                }

                bindings[i] = bindingString;
                bindingsBufferPtr = (IntPtr)(bindingsBufferPtr.ToInt64() + (bindingString.Length + 1) * sizeof(ushort));
            }

            return bindings;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            initializedEvent.Reset();
            WasHelper.RegisterProtocol(UdpConstants.Scheme, ref webHostCallbacks, out protocolHandle);
            initializedEvent.WaitOne(timeout, true);

            serviceHost = new ServiceHost(typeof(UdpControlService),
                new Uri(HostedUdpConstants.ControlServiceAddress));

            // FUTURE: For security purpose, we need to set the right ACL to the Named-Pipe. Unfortunately
            // this is not supported in current WCF API.
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            serviceHost.AddServiceEndpoint(typeof(IUdpControlRegistration),
                binding, "");

            serviceHost.Open();
        }

        protected override void OnClosing()
        {
            base.OnClosing();

            // FUTURE: It would be nice if we close all listenerchannel instances for a graceful shutdown.
            // Otherwise WAS may terminate the worker process unexpected.

            WasHelper.UnregisterProtocol(protocolHandle);
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }

#region webhost_callback_impl
        void OnConfigManagerConnected(IntPtr context) { }
        void OnConfigManagerConnectRejected(IntPtr context, int hresult) { }
        void OnConfigManagerDisconnected(IntPtr context, int hresult) { }
        void OnConfigManagerInitializationCompleted(IntPtr context)
        {
            Debug.Assert(this.State == CommunicationState.Opening);
            initializedEvent.Set();
        }

        void OnApplicationCreated(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, [MarshalAs(UnmanagedType.LPWStr)] string path, int siteId, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, IntPtr bindingsMultiSz, int numberOfBindings, bool requestsBlocked)
        {
            if (!requestsBlocked)
            {
                string[] bindings = null;

                try
                {
                    bindings = ParseBindings(bindingsMultiSz, numberOfBindings);
                }
                catch (ArgumentException)
                {
                    return;
                }

                if (bindings.Length > 1)
                {
                    // We do not support multiple bindings.
                }

                App app = null;
                try
                {
                    app = appManager.CreateApp(appKey, path, siteId, appPoolId);
                }
                catch (NotSupportedException)
                {
                    // FUTURE: We can add the support for multiple apps later.
                }

                int port = 0;

                // Binding format: <protocol>:<number>
                string[] parts = bindings[0].Split(':');
                if (parts.Length != 2 || !int.TryParse(parts[1], out port))
                {
                    return;
                }

                StartListen(app, port);
            }
        }

        void OnApplicationPoolAllListenerChannelInstancesStopped(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, int listenerChannelId) { }
        void OnApplicationAppPoolChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId) { }
        void OnApplicationBindingsChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, IntPtr bindingsMultiSz, int numberOfBindings) { }
        void OnApplicationDeleted(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey) { }
        void OnApplicationPoolCanOpenNewListenerChannelInstance(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, int listenerChannelId) { }
        void OnApplicationPoolCreated(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, IntPtr sid) { }
        void OnApplicationPoolDeleted(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId) { }
        void OnApplicationPoolIdentityChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, IntPtr sid) { }
        void OnApplicationPoolStateChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, bool isEnabled) { }
        void OnApplicationRequestsBlockedChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, bool requestsBlocked) { }
#endregion

        protected override void OnAbort()
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnClose(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnOpen(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }
    }
}


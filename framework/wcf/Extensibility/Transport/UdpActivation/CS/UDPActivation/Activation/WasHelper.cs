
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.ServiceModel.Samples.Activation
{
    static class WAPI
    {
        public delegate int WebhostRegisterProtocol([MarshalAs(UnmanagedType.LPWStr)] string protocolId, ref WebhostListenerCallbacks listenerCallbacks, IntPtr context, out int protocolHandle);
        public delegate int WebhostUnregisterProtocol(int protocolHandle);
        public delegate int WebhostOpenListenerChannelInstance(int protocolHandle, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, int listenerChannelId, byte[] queueBlob, int queueBlobByteCount);
        public delegate int WebhostCloseAllListenerChannelInstances(int protocolHandle, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, int listenerChannelId);
    }

    static class WCB
    {
        public delegate void ConfigManagerConnected(IntPtr context);
        public delegate void ConfigManagerConnectRejected(IntPtr context, int hresult);
        public delegate void ConfigManagerDisconnected(IntPtr context, int hresult);
        public delegate void ConfigManagerInitializationCompleted(IntPtr context);

        public delegate void ApplicationCreated(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, [MarshalAs(UnmanagedType.LPWStr)] string path, int siteId, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, IntPtr bindingsMultiSz, int numberOfBindings, bool requestsBlocked);
        public delegate void ApplicationPoolAllListenerChannelInstancesStopped(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, int listenerChannelId);        
        public delegate void ApplicationAppPoolChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId);
        public delegate void ApplicationBindingsChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, IntPtr bindingsMultiSz, int numberOfBindings);
        public delegate void ApplicationDeleted(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey);
        public delegate void ApplicationPoolCanOpenNewListenerChannelInstance(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, int listenerChannelId);
        public delegate void ApplicationPoolCreated(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, IntPtr sid);
        public delegate void ApplicationPoolDeleted(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId);
        public delegate void ApplicationPoolIdentityChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, IntPtr sid);
        public delegate void ApplicationPoolStateChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appPoolId, bool isEnabled);
        public delegate void ApplicationRequestsBlockedChanged(IntPtr context, [MarshalAs(UnmanagedType.LPWStr)] string appKey, bool requestsBlocked);
    }

#if true
    [StructLayout(LayoutKind.Sequential)]
    struct WebhostListenerCallbacks
    {
        public int dwBytesInCallbackStructure;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ConfigManagerConnected configManagerConnected;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ConfigManagerDisconnected configManagerDisconnected;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ConfigManagerInitializationCompleted configManagerInitializationCompleted;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolCreated applicationPoolCreated;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolDeleted applicationPoolDeleted;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolIdentityChanged applicationPoolIdentityChanged;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolStateChanged applicationPoolStateChanged;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolCanOpenNewListenerChannelInstance applicationPoolCanOpenNewListenerChannelInstance;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolAllListenerChannelInstancesStopped applicationPoolAllListenerChannelInstancesStopped;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationCreated applicationCreated;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationDeleted applicationDeleted;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationBindingsChanged applicationBindingsChanged;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationAppPoolChanged applicationAppPoolChanged;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationRequestsBlockedChanged applicationRequestsBlockedChanged;
    }
#else
    [StructLayout(LayoutKind.Sequential)]
    struct WebhostListenerCallbacks
    {
        public int dwBytesInCallbackStructure;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ConfigManagerConnected configManagerConnected;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ConfigManagerInitializationCompleted configManagerInitializationCompleted;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationPoolAllListenerChannelInstancesStopped applicationPoolAllListenerChannelInstancesStopped;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WCB.ApplicationCreated applicationCreated;
    }
#endif

    class WasHelper
    {
        static object syncRoot = new object();
        static bool initialized = false;
        static string libName = Environment.SystemDirectory + "\\inetsrv\\wbhstipm.dll";
        static SafeFreeLibrary wasAPILib;
        static WAPI.WebhostRegisterProtocol webhostRegisterProtocol;
        static WAPI.WebhostUnregisterProtocol webhostUnregisterProtocol;
        static WAPI.WebhostOpenListenerChannelInstance webhostOpenListenerChannelInstance;
        static WAPI.WebhostCloseAllListenerChannelInstances webhostCloseAllListenerChannelInstances;

        static void EnsureInitialized()
        {
            if (initialized)
                return;

            lock (syncRoot)
            {
                if (initialized)
                    return;

                wasAPILib = NativeMethods.LoadLibraryEx(libName, IntPtr.Zero, NativeMethods.LOAD_WITH_ALTERED_SEARCH_PATH);
                webhostRegisterProtocol = (WAPI.WebhostRegisterProtocol)GetProcDelegate<WAPI.WebhostRegisterProtocol>(wasAPILib, "WebhostRegisterProtocol");
                webhostUnregisterProtocol = (WAPI.WebhostUnregisterProtocol)GetProcDelegate<WAPI.WebhostUnregisterProtocol>(wasAPILib, "WebhostUnregisterProtocol");
                webhostOpenListenerChannelInstance = (WAPI.WebhostOpenListenerChannelInstance)GetProcDelegate<WAPI.WebhostOpenListenerChannelInstance>(wasAPILib, "WebhostOpenListenerChannelInstance");
                webhostCloseAllListenerChannelInstances = (WAPI.WebhostCloseAllListenerChannelInstances)GetProcDelegate<WAPI.WebhostCloseAllListenerChannelInstances>(wasAPILib, "WebhostCloseAllListenerChannelInstances");
            }
        }

        static Delegate GetProcDelegate<TDelegate>(SafeFreeLibrary lib, string procName)
        {
            IntPtr funcPtr = NativeMethods.GetProcAddress(lib, procName);
            if (funcPtr == IntPtr.Zero)
            {
                throw new Win32Exception(string.Format("Function '{0}' not found.", procName));
            }

            return Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(TDelegate));
        }

        public static void RegisterProtocol(string protocolId, ref WebhostListenerCallbacks listenerCallbacks, out int protocolHandle)
        {
            EnsureInitialized();
            int hresult = webhostRegisterProtocol(protocolId, ref listenerCallbacks, IntPtr.Zero, out protocolHandle);
            if (hresult != 0)
            {
                throw new Win32Exception(hresult);
            }
        }

        public static void UnregisterProtocol(int protocolHandle)
        {
            EnsureInitialized();
            int hresult = webhostUnregisterProtocol(protocolHandle);
            if (hresult != 0)
            {
                throw new Win32Exception(hresult);
            }
        }

        public static int OpenListenerChannelInstance(int protocolHandle, string appPoolId, int listenerChannelId, byte[] queueBlob)
        {
            int queueBlobLength = (queueBlob != null) ? queueBlob.Length : 0;

            EnsureInitialized();
            return webhostOpenListenerChannelInstance(protocolHandle, appPoolId, listenerChannelId, queueBlob, queueBlobLength);
        }

        protected int CloseAllListenerChannelInstances(int protocolHandle, string appPoolId, int listenerChannelId)
        {
            if (string.IsNullOrEmpty(appPoolId))
            {
                throw new ArgumentNullException("appPoolId");
            }

            EnsureInitialized();
            return webhostCloseAllListenerChannelInstances(protocolHandle, appPoolId, listenerChannelId);
        }
    }

    class SafeFreeLibrary : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeFreeLibrary()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.FreeLibrary(handle);
        }
    }

    static class NativeMethods
    {
        internal const int LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFreeLibrary LoadLibrary([In] string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFreeLibrary LoadLibraryEx(
            [In] string lpFileName,
            [In] IntPtr hFile,
            [In] int dwFlags
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetProcAddress(
            [In] SafeFreeLibrary hModule,
            [In] string lpProcName);
    }
}


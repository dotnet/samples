//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{   
    //This class included the implementation of the service contracts defined in Interfaces.cs

    //Added ServiceBehaviors 
    //Concurrency Mode = Reentrant : To make the service method re-entrant. Required in case of a CallBack.
    //IncludeExceptionDetailInFaults : Helpful for debugging, server side exceptions are returned as Faults.
 
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant,IncludeExceptionDetailInFaults=true)]
    public class BaseServer: IAsyncOneWay, IAsyncSessionOneWay, IAsyncSessionTwoWay, IAsyncTwoWay,
        ISyncOneWay, ISyncSessionOneWay, ISyncSessionTwoWay, ISyncTwoWay, IDuplexContract, IDuplexSessionContract    
    {
        ICallBack callback = null;
        
        private static int msgCount = 0;
        public static int MsgCount
        {
            get { return BaseServer.msgCount; }
            set { BaseServer.msgCount = value; }
        }

        public void OneWay(string msg)
        {
            Interlocked.Increment(ref msgCount);
            Log.Trace("Received Message # " + msgCount + "  Message == " + msg);            
        }

        public string TwoWay(string msg)
        {
            Interlocked.Increment(ref msgCount);
            Log.Trace("Received Message # " + msgCount + "  Message == " + msg);
            return ("Received Message  # " + msgCount); 
        }

        private delegate void MyAsyncSessionOneWayDelegate(string s);
        private delegate string MyAsyncSessionTwoWayDelegate(string s);
        private delegate void MyAsyncOneWayDelegate(string s);
        private delegate string MyAsyncTwoWayDelegate(string s);

        static MyAsyncSessionOneWayDelegate myAsyncSessionOneWayDelegate = new MyAsyncSessionOneWayDelegate(AsyncOneWayMethod);
        static MyAsyncSessionTwoWayDelegate myAsyncSessionTwoWayDelegate = new MyAsyncSessionTwoWayDelegate(AsyncTwoWayMethod);
        static MyAsyncOneWayDelegate myAsyncOneWayDelegate = new MyAsyncOneWayDelegate(AsyncOneWayMethod);
        static MyAsyncTwoWayDelegate myAsyncTwoWayDelegate = new MyAsyncTwoWayDelegate(AsyncTwoWayMethod);
        
        private static void AsyncOneWayMethod(string msg)
        {
            Interlocked.Increment(ref msgCount);            
            Log.Trace("Received Message # " + msgCount + "  Message == " + msg);                
        }
        private static string AsyncTwoWayMethod(string msg)
        {
            Interlocked.Increment(ref msgCount);
            Log.Trace("Received Message # " + msgCount + "  Message == " + msg);
            return ("Received Message  # " + msgCount); 
        }    

        public IAsyncResult BeginSessionOneWayMethod(string msg, AsyncCallback callback, object state)
        {
            Log.Trace("BeginSessionOneWayMethod");
            return myAsyncSessionOneWayDelegate.BeginInvoke(msg, callback, state);
        }

        public void EndSessionOneWayMethod(IAsyncResult r)
        {
            Log.Trace("EndSessionOneWayMethod: " + r.ToString());
            myAsyncSessionOneWayDelegate.EndInvoke(r);
        }

        public IAsyncResult BeginSessionTwoWayMethod(string msg, AsyncCallback callback, object state)
        {
            Log.Trace("BeginSessionTwoWayMethod");
            return myAsyncSessionTwoWayDelegate.BeginInvoke(msg, callback, state);
        }

        public string EndSessionTwoWayMethod(IAsyncResult r)
        {
            Log.Trace("EndSessionTwoWayMethod: " + r.ToString());
            return (myAsyncSessionTwoWayDelegate.EndInvoke(r));
        }

        public IAsyncResult BeginOneWayMethod(string msg, AsyncCallback callback, object state)
        {
            Log.Trace("BeginOneWayMethod");
            return myAsyncOneWayDelegate.BeginInvoke(msg, callback, state);
        }

        public void EndOneWayMethod(IAsyncResult r)
        {
            Log.Trace("EndOneWayMethod: " + r.ToString());
            myAsyncOneWayDelegate.EndInvoke(r);
        }       

        public IAsyncResult BeginTwoWayMethod(string msg, AsyncCallback callback, object state)
        {
            Log.Trace("BeginTwoWayMethod");
            return myAsyncTwoWayDelegate.BeginInvoke(msg, callback, state);
        }

        public string EndTwoWayMethod(IAsyncResult r)
        {
            Log.Trace("EndTwoWayMethod: " + r.ToString());
            return (myAsyncTwoWayDelegate.EndInvoke(r));
        }

        public void DuplexOneWay(string msg)
        {            
            Interlocked.Increment(ref msgCount);
            Log.Trace("Received Message # " + msgCount + "  Message == " + msg);
            callback = OperationContext.Current.GetCallbackChannel<ICallBack>();
            callback.CallBackMethod("Received Message # " + msgCount);            
        }
    }

    //Implementation of the CallBack class
    public class CallBackService : ICallBack
    {
        public void CallBackMethod(string msg)
        {
            Log.Trace("CallBack Message " + msg);
        }
    }  
}

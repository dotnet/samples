//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Samples.Tools.CustomChannelsTester
{
    public enum MessageLevel
    {
        ALW = 1,
        ERR = 2,
        WRN = 3,
        INF = 4,
        TRC = 5
    }    

    //The class helps looging exceptions and traces for the tests
    
    public static class Log
    {
        private static string _processid;
        private static string _processname;
        
        static Log()
        {            
            _processid		= Process.GetCurrentProcess().Id.ToString();
			_processname	= Process.GetCurrentProcess().MainModule.ModuleName;  
            
        }
        
        public static void Trace(string message)
        {
            Log.WriteLine(DateTime.Now, MessageLevel.TRC, Thread.CurrentThread.GetHashCode(), Thread.CurrentThread.Name, message);            
        }

        public static void Exception(Exception e)
        {
            Log.WriteLine(DateTime.Now, MessageLevel.ERR, Thread.CurrentContext.GetHashCode(),Thread.CurrentThread.Name, e.Message);
        }

        static void WriteLine(DateTime msgTime, MessageLevel msglvl, int threadid, string threadName, string msg)
        {
            string message;
            DateTime msgtime = DateTime.Now;
            if(threadName == null)
                message = "<" + _processid + "." + threadid + "> " + Enum.Format(typeof(MessageLevel), msglvl, "G") + " - " + msgtime.ToLongTimeString() + " - " + msg; 
            else
                message = "<" + _processid+ "." + threadid + ". " + threadName +"> "+ Enum.Format(typeof(MessageLevel), msglvl, "G") +" - "+msgtime.ToLongTimeString()+" - "+msg; 
            Console.WriteLine("\n" + message);
        }

    }
}

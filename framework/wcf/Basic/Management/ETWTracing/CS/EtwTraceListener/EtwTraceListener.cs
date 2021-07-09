//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Samples.ServiceModel
{
   
    public class EtwTraceListener : TraceListener
    {
        public EtwTraceListener()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            currentDomain.DomainUnload += new EventHandler(ExitOrUnloadEventHandler);
            currentDomain.ProcessExit += new EventHandler(ExitOrUnloadEventHandler);
        }

        static void ExitOrUnloadEventHandler(object sender, EventArgs e)
        {
            EtwTrace.Dispose();
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            EtwTrace.Dispose();
        }

        public override void Close()
        {
            EtwTrace.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
            	if (disposing)
                    this.Close();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        void TraceInternal(TraceEventCache eventCache, string xmlApplicationData, int eventId, TraceEventType type)
        {
            EtwTrace.Trace(xmlApplicationData, TraceTypeOf(type));
        }

        public override void TraceEvent(TraceEventCache eventCache, String source, TraceEventType severity, int id, string format, params object[] args)
        {
            TraceInternal(eventCache, null == args ? format : String.Format(format, args), id, severity);
        }

        public override void TraceEvent(TraceEventCache eventCache, String source, TraceEventType severity, int id, string message)
        {
            TraceInternal(eventCache, message, id, severity);
        }

        public override void TraceData(TraceEventCache eventCache, String source, TraceEventType severity, int id, object data)
        {
            TraceInternal(eventCache, data.ToString(), id, severity);
        }

        public override void TraceData(TraceEventCache eventCache, String source, TraceEventType severity, int id, params object[] data)
        {
            NotSupported();
        }

        public override void TraceTransfer(TraceEventCache eventCache, String source, int id, string message, Guid relatedActivityId)
        {
            EtwTrace.TraceTransfer(relatedActivityId);
        }

        static TraceType TraceTypeOf(TraceEventType type)
        {
            switch (type)
            {
                case TraceEventType.Transfer:
                    return TraceType.Transfer;
                case TraceEventType.Start:
                    return TraceType.Start;
                case TraceEventType.Stop:
                    return TraceType.Stop;
                case TraceEventType.Suspend:
                    return TraceType.Suspend;
                case TraceEventType.Resume:
                    return TraceType.Resume;
                default:
                    return TraceType.Trace;
            }
        }

        public override void Write(string text)
        {
            WriteLine(text);
        }

        public override void WriteLine(string text)
        {
            EtwTrace.Trace(text, TraceType.Trace);
        }

        void NotSupported()
        {
            throw new NotSupportedException();
        }
    }


    enum EtwStructSizes
    {
        SizeofMofField = 16,
        SizeofGuid = 16,
        SizeofEventHeader = 48,
        SizeofBaseEvent = 176,
    }

    //Attempt to be consistent with generic event types from EvnTrace.h
    enum TraceType : byte
    {
        Trace       = 0,
        Start       = 1,
        Stop        = 2,
        Transfer    = 5,
        Suspend     = 10,
        Resume      = 11,
    }

    static class EtwTrace
    {
        static Guid WCFTraceGuid = new Guid("{dd21656a-7564-4769-90e2-f3434cc1fdf4}");
        static Guid ProviderGuid = new Guid("411a0819-c24b-428c-83e2-26b41091702e");

        const int MaxSupportedStringSize = 65486;
        static EtwTraceProvider provider;
        static object syncRoot = new object();
        static bool disposed = false;

        internal static EtwTraceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    lock (EtwTrace.syncRoot)
                    {
                        if (provider == null && !disposed)
                        {
                            provider = new EtwTraceProvider(ProviderGuid);
                        }
                    }
                }
                return provider;
            }
        }

        internal static void Dispose()
        {
            if (provider != null)
            {
                lock (EtwTrace.syncRoot)
                {
                    if (provider != null && !disposed)
                    {
                        ((IDisposable)provider).Dispose();
                        provider = null;
                        disposed = true;
                    }
                }
            }
        }

        internal static void Trace(string xml, TraceType type)
        {
            TraceInternal(EtwTrace.GetActivityId(), xml, type);
        }

        static Guid GetActivityId()
        {
            object id = System.Diagnostics.Trace.CorrelationManager.ActivityId;
            return id == null ? Guid.Empty : (Guid)id;
        }

        static internal unsafe uint TraceTransfer(Guid relatedId)
        {
            return TraceTransfer(GetActivityId(), relatedId);
        }

        static unsafe uint TraceTransfer(Guid activityId, Guid relatedId)
        {
            uint result = unchecked((uint)-1);

            if (null != Provider && Provider.ShouldTrace)
            {
                Guid2Event evt = new Guid2Event();
                evt.Header.Guid = WCFTraceGuid;
                evt.Header.Type = (byte)TraceType.Transfer;
                evt.Header.ClientContext = 0;
                evt.Header.Flags = WnodeFlags.WnodeFlagTracedGuid;
                evt.Header.BufferSize = (ushort)EtwStructSizes.SizeofEventHeader + 2 * (ushort)EtwStructSizes.SizeofGuid;


                evt.Guid1 = activityId;
                evt.Guid2 = relatedId;

                if (null != Provider)
                {
                    result = provider.Trace((MofEvent*)&evt);
                }
            }

            return result;
        }

        static unsafe uint TraceInternal(Guid guid, string xml, TraceType type)
        {
            uint result = unchecked((uint)-1);

            if (null != Provider && Provider.ShouldTrace)
            {
                int dataLength = (xml.Length + 1) * 2 < MaxSupportedStringSize ? (xml.Length + 1) * 2 : MaxSupportedStringSize;

                Mof2Event evt = new Mof2Event();
                evt.Header.Guid = WCFTraceGuid;
                evt.Header.Type = (byte)type;
                evt.Header.ClientContext = 0;
                evt.Header.Flags = WnodeFlags.WnodeFlagTracedGuid | WnodeFlags.WnodeFlagUseMofPointer;
                evt.Header.BufferSize = (ushort)EtwStructSizes.SizeofEventHeader + 2 * (ushort)EtwStructSizes.SizeofMofField;

                evt.Mof2.Length = (uint)dataLength;


                evt.Mof1.Length = 16;
                evt.Mof1.Data = (IntPtr)(&guid);


                fixed (char* pdata = xml)
                {
                    evt.Mof2.Data = (IntPtr)pdata;
                    if (null != Provider)
                    {
                        result = provider.Trace((MofEvent*)&evt);
                    }
                }
            }

            return result;
        }
    }

    internal unsafe delegate uint EtwTraceCallback([In] uint requestCode,
                                                   [In] System.IntPtr requestContext,
                                                   [In] System.IntPtr bufferSize,
                                                   [In] byte* buffer);

    class EtwTraceProvider : IDisposable
    {
        Guid controlGuid;
        EtwTraceCallback etwProc;
        ulong registrationHandle;
        ulong traceHandle;
        bool isDisposed = false;

        internal EtwTraceProvider(Guid guid)
        {
            Initialize(guid);
        }

        internal bool ShouldTrace { get { return traceHandle != 0; } }

        internal unsafe uint Trace(MofEvent* evt)
        {
            bool shouldRedispose = this.isDisposed;
            if (shouldRedispose)
            {
                this.Initialize(this.controlGuid);
            }
            uint retval = EtwNativeMethods.TraceEvent(traceHandle, (char*)evt);
            if (shouldRedispose)
            {
                this.Dispose(true);
            }
            return retval;
        }

        void IDisposable.Dispose()
        {
            if (!this.isDisposed)
            {
                Dispose(true);
            }
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                int result = EtwNativeMethods.UnregisterTraceGuids(this.registrationHandle);
                // PreSharp requires this: ignore result as we can neither trace nor throw
            }

            this.isDisposed = true;
        }

        unsafe void Initialize(Guid guid)
        {
            controlGuid = guid;
            TraceGuidRegistration guidReg = new TraceGuidRegistration();
            Guid dummyGuid = new Guid("{aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaabbb}");

            etwProc = new EtwTraceCallback(EtwNotificationCallback);

            guidReg.Guid = &dummyGuid;
            guidReg.RegHandle = null;

            uint status = EtwNativeMethods.RegisterTraceGuids(etwProc,
                                                           null,
                                                           ref controlGuid,
                                                           1,
                                                           ref guidReg,
                                                           null,
                                                           null,
                                                           out registrationHandle);
            if (status != 0)
            {
                throw new Win32Exception((int)status);
            }
            this.isDisposed = false;
        }

        unsafe uint EtwNotificationCallback(uint requestCode, System.IntPtr context, System.IntPtr bufferSize, byte* buffer)
        {
            if (null == buffer)
            {
                //note that the return value will be ignored
                return uint.MaxValue;
            }

            EventTraceHeader* eventBuffer = (EventTraceHeader*)buffer;

            switch (requestCode)
            {
                case RequestCodes.EnableEvents:
                    this.traceHandle = eventBuffer->HistoricalContext;
                    int flags = EtwNativeMethods.GetTraceEnableFlags((ulong)this.traceHandle);
                    int level = EtwNativeMethods.GetTraceEnableLevel((ulong)this.traceHandle);
                    using (Process process = Process.GetCurrentProcess())
                    {
                        if (flags == process.Id)
                        {
                            //TODO, vadim, if the listener is private, forward the level
                            //if not, consider using TraceFilter
                            //Level = LevelFromInt(level);
                        }
                    }
                    break;
                case RequestCodes.DisableEvents:
                    this.traceHandle = 0;
                    break;
                default:
                    this.traceHandle = 0;
                    break;
            }

            return 0;
        }

        SourceLevels LevelFromInt(int level)
        {
            SourceLevels result = SourceLevels.Off;
            if (level >= 5)
            {
                result = SourceLevels.Verbose;
            }
            else if (level == 4)
            {
                result = SourceLevels.Information;
            }
            else if (level == 3)
            {
                result = SourceLevels.Warning;
            }
            else if (level == 2)
            {
                result = SourceLevels.Error;
            }
            else
            {
                result = SourceLevels.Critical;
            }

            return result;
        }
    }

    internal static class RequestCodes
    {
        internal const int GetAllData = 0;        // Never Used
        internal const int GetSingleInstance = 1; // Never Used
        internal const int SetSingleInstance = 2; // Never Used
        internal const int SetSingleItem = 3;     // Never Used
        internal const int EnableEvents = 4;      // Enable Tracing
        internal const int DisableEvents = 5;     // Disable Tracing
        internal const int EnableCollection = 6;  // Never Used
        internal const int DisableCollection = 7; // Never Used
        internal const int RegInfo = 8;           // Never Used
        internal const int ExecuteMethod = 9;     // Never Used
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal struct EventTraceHeader
    {
        [FieldOffset(0)]
        internal ushort BufferSize;
        [FieldOffset(4)]
        internal byte Type;
        [FieldOffset(5)]
        internal byte Level;
        [FieldOffset(6)]
        internal short Version;
        [FieldOffset(8)]
        internal ulong HistoricalContext;
        [FieldOffset(16)]
        internal Int64 TimeStamp;
        [FieldOffset(24)]
        internal System.Guid Guid;
        [FieldOffset(40)]
        internal uint ClientContext;
        [FieldOffset(44)]
        internal uint Flags;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal struct GuidEvent
    {
        [FieldOffset(0)]
        internal EventTraceHeader Header;
        [FieldOffset(48)]
        internal Guid Guid;
    }

    [StructLayout(LayoutKind.Explicit, Size = 80)]
    internal struct Guid2Event
    {
        [FieldOffset(0)]
        internal EventTraceHeader Header;
        [FieldOffset(48)]
        internal Guid Guid1;
        [FieldOffset(64)]
        internal Guid Guid2;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal struct MofEvent
    {
        [FieldOffset(0)]
        internal EventTraceHeader Header;
        [FieldOffset(48)]
        internal MofField Mof;
    }

    [StructLayout(LayoutKind.Explicit, Size = 80)]
    internal struct Mof2Event
    {
        [FieldOffset(0)]
        internal EventTraceHeader Header;
        [FieldOffset(48)]
        internal MofField Mof1;
        [FieldOffset(64)]
        internal MofField Mof2;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct MofField
    {
        [FieldOffset(0)]
        internal IntPtr Data;
        [FieldOffset(8)]
        internal uint Length;
        [FieldOffset(12)]
        internal uint Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TraceGuidRegistration
    {
        internal unsafe Guid* Guid;
        internal unsafe void* RegHandle;
    }

    internal static class WnodeFlags
    {
        internal const uint WnodeFlagTracedGuid = 0x00020000;
        internal const uint WnodeFlagLogWnode = 0x00040000;
        internal const uint WnodeFlagUseGuidPointer = 0x00080000;
        internal const uint WnodeFlagUseMofPointer = 0x00100000;
        internal const uint WnodeFlagUseNoHeader = 0x00200000;
    }

    static class EtwNativeMethods
    {
        [DllImport("advapi32", ExactSpelling = true, EntryPoint = "GetTraceEnableFlags", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern int GetTraceEnableFlags(ulong traceHandle);

        [DllImport("advapi32", ExactSpelling = true, EntryPoint = "GetTraceEnableLevel", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern char GetTraceEnableLevel(ulong traceHandle);

        [DllImport("advapi32", ExactSpelling = true, EntryPoint = "TraceEvent", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern unsafe uint TraceEvent(ulong traceHandle, char* header);

        [DllImport("advapi32", ExactSpelling = true, EntryPoint = "RegisterTraceGuidsW", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern unsafe uint RegisterTraceGuids([In]EtwTraceCallback cbFunc, [In]void* context, [In] ref System.Guid controlGuid, [In] uint guidCount, ref TraceGuidRegistration guidReg, [In]string mofImagePath, [In] string mofResourceName, [Out] out ulong regHandle);

        [DllImport("advapi32", ExactSpelling = true, EntryPoint = "UnregisterTraceGuids", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern int UnregisterTraceGuids(ulong regHandle);
    }
}

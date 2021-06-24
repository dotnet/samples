//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System;
using System.Activities.Tracking;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{

    public class ConsoleParticipant : TrackingParticipant
    {
        public ConsoleParticipant()
        {
            if (Writer == null)
            {
                Writer = Console.Out;
            }
            Writer.WriteLine("ConsoleParticipant Created");
        }

        public ConsoleParticipant(NameValueCollection parameters)
            : this()
        {
            if (parameters != null)
            {
                foreach (string name in parameters)
                {
                    if (string.Compare(name, "connectionString", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        Writer.WriteLine("ConnectionString: " + parameters[name]);
                    }
                    else if (string.Compare(name, "trackingStore", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        Writer.WriteLine("TrackingStore: " + parameters[name]);
                    }
                }
            }
        }

        public static TextWriter Writer{ get; set; }

        public override void Track(TrackingRecord record, TimeSpan timeout)
        {
            Type recordType = record.GetType();

            Writer.WriteLine("[" + record.RecordNumber + "] ({" + record.EventTime + "}) Instance: " + record.InstanceId);

            if (record.Annotations.Count > 0)
            {
                Writer.WriteLine("Annotations:");
                foreach (string key in record.Annotations.Keys)
                {
                    Writer.WriteLine("\t" + key + "\t" + record.Annotations[key]);
                }
            }

            if (recordType == typeof(WorkflowInstanceUnhandledExceptionRecord))
            {
                WorkflowInstanceUnhandledExceptionRecord crecord = (WorkflowInstanceUnhandledExceptionRecord)record;
                RecordWriteLine(crecord, "-----------Exception");
                RecordWriteLine(crecord, crecord.UnhandledException.ToString());
                RecordWriteLine(crecord, "-----------End Exception");
            }
            else if (recordType == typeof(WorkflowInstanceRecord))
            {
                WorkflowInstanceRecord crecord = (WorkflowInstanceRecord)record;
                RecordWriteLine(crecord, "Workflow State: " + crecord.State);
            }
            else if (recordType == typeof(ActivityTrackingRecord))
            {
                ActivityTrackingRecord crecord = (ActivityTrackingRecord)record;
                RecordWriteLine(crecord, "Activity, " + crecord.Name + ", entered state: " + crecord.State);

                if (crecord.Variables.Count > 0)
                {
                    Writer.WriteLine("Variables:");
                    foreach (string key in crecord.Variables.Keys)
                    {
                        Writer.WriteLine("\t" + key + "\t" + crecord.Variables[key].ToString());
                    }
                }
            }
        }

        private void RecordWriteLine(TrackingRecord record, String text)
        {
            Writer.WriteLine("[" + record.RecordNumber + "] " + text);
        }
    }
}

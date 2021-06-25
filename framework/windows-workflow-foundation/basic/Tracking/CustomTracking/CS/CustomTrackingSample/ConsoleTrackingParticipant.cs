//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Samples.CustomTrackingSample
{

    // A custom tracking participant that emits TrackingRecord objects to the console
    public class ConsoleTrackingParticipant : TrackingParticipant
    {
        private const String participantName = "ConsoleTrackingParticipant";

        public ConsoleTrackingParticipant()
        {
            Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
                "{0} Created", participantName));
        }


        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            Console.Write(String.Format(CultureInfo.InvariantCulture, 
                "{0} emitted trackRecord: {1}  Level: {2}, RecordNumber: {3}",
                  participantName, record.GetType().FullName, 
                  record.Level, record.RecordNumber));

            WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
            if (workflowInstanceRecord != null)
            {
                Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
                    " Workflow InstanceID: {0} Workflow instance state: {1}",
                    record.InstanceId, workflowInstanceRecord.State));
            }

            ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
            if (activityStateRecord != null)
            {
                IDictionary<String, object> variables = activityStateRecord.Variables;
                StringBuilder vars = new StringBuilder();

                if (variables.Count > 0)
                {
                    vars.AppendLine("\n\tVariables:");
                    foreach (KeyValuePair<string, object> variable in variables)
                    {   
                        vars.AppendLine(String.Format(
                            "\t\tName: {0} Value: {1}", variable.Key, variable.Value));
                    }
                }
                Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
                    " :Activity DisplayName: {0} :ActivityInstanceState: {1} {2}",
                       activityStateRecord.Activity.Name, activityStateRecord.State,
                    ((variables.Count > 0) ? vars.ToString() : String.Empty)));
            }

            CustomTrackingRecord customTrackingRecord = record as CustomTrackingRecord;

            if ((customTrackingRecord != null) && (customTrackingRecord.Data.Count > 0))
            {
                Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
                    "\n\tUser Data:"));
                foreach (string data in customTrackingRecord.Data.Keys)
                {
                    Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
                        " \t\t {0} : {1}", data, customTrackingRecord.Data[data]));
                }
            }
            Console.WriteLine();

        }
    }
}

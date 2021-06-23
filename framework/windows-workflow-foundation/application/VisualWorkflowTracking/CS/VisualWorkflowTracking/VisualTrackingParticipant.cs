//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;


namespace Microsoft.Samples.VisualWorkflowTracking
{
    public class VisualTrackingParticipant : System.Activities.Tracking.TrackingParticipant
    {
        public event EventHandler<TrackingEventArgs> TrackingRecordReceived;
        public Dictionary<string, Activity> ActivityIdToWorkflowElementMap { get; set; }
        

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            OnTrackingRecordReceived(record, timeout);
        }

        //On Tracing Record Received call the TrackingRecordReceived with the record received information from the TrackingParticipant. 
        //We also do not worry about Expressions' tracking data
        protected void OnTrackingRecordReceived(TrackingRecord record, TimeSpan timeout)
        {
            System.Diagnostics.Debug.WriteLine(
                String.Format("Tracking Record Received: {0} with timeout: {1} seconds.", record, timeout.TotalSeconds)
            );

            if (TrackingRecordReceived != null)
            {
                ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
                
                if((activityStateRecord != null) && (!activityStateRecord.Activity.TypeName.Contains("System.Activities.Expressions")))
                {
                    if (ActivityIdToWorkflowElementMap.ContainsKey(activityStateRecord.Activity.Id))
                    {
                        TrackingRecordReceived(this, new TrackingEventArgs(
                                                        record,
                                                        timeout,
                                                        ActivityIdToWorkflowElementMap[activityStateRecord.Activity.Id]
                                                        )

                            );
                    }
                   
                }
                else
                {
                    TrackingRecordReceived(this, new TrackingEventArgs(record, timeout,null));
                }
                
            }
        }
    }

    //Custom Tracking EventArgs
    public class TrackingEventArgs : EventArgs
    {
        public TrackingRecord Record {get; set;}
        public TimeSpan Timeout {get; set;}
        public Activity Activity { get; set; }

        public TrackingEventArgs(TrackingRecord trackingRecord, TimeSpan timeout, Activity activity)
        {
            this.Record = trackingRecord;
            this.Timeout = timeout;
            this.Activity = activity;
        }
    }
}

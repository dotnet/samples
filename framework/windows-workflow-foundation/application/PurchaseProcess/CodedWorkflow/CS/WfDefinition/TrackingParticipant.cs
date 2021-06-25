//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities.Tracking;
using System.IO;

namespace Microsoft.Samples.WF.PurchaseProcess
{

    /// <summary>
    /// Create a tracking participant that saves all events to a text file
    /// </summary>
    public class SaveAllEventsToTestFileTrackingParticipant: TrackingParticipant
    {
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            // get the tracking path
            string fileName = IOHelper.GetTrackingFilePath(record.InstanceId);

            // create a writer and open the file
            using (StreamWriter tw = File.AppendText(fileName))
            {
                // write a line of text to the file
                tw.WriteLine(record.ToString());             
            }
        }
    }
}

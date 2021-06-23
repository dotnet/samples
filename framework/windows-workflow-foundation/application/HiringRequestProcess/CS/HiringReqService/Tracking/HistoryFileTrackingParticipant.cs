//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities.Hosting;
using System.Activities.Persistence;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.HiringService
{
    public class HistoryFileTrackingParticipant : TrackingParticipant, IWorkflowInstanceExtension
    {
        PersistenceIOParticipant persistenceParticipant;
        RequestHistoryRecord historyRecord = new RequestHistoryRecord();

        public HistoryFileTrackingParticipant()
        {
            persistenceParticipant = new HistorySavePersistenceParticipant(this);
        }

        public void Save()
        {
            RequestHistoryRepository.Save(this.historyRecord);
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {                                  
            // get the custom record       
            CustomTrackingRecord customRecord = (CustomTrackingRecord)record;            
            string hiringRequestId = customRecord.Data["HiringRequestId"].ToString();

            // create the request history record
            historyRecord = new RequestHistoryRecord();
            historyRecord.RequestId = hiringRequestId;
            historyRecord.RecordNumber = record.RecordNumber;
            historyRecord.Action = GetStringFromTrackingRecord("Action", customRecord.Data);
            historyRecord.SourceState = GetStringFromTrackingRecord("State", customRecord.Data);
            historyRecord.Comment = GetStringFromTrackingRecord("Comment", customRecord.Data);
            historyRecord.EmployeeId = GetStringFromTrackingRecord("EmployeeId", customRecord.Data);
            historyRecord.EmployeeName = GetStringFromTrackingRecord("EmployeeName", customRecord.Data);
            historyRecord.Date = GetDateFromTrackingRecord(customRecord.Data);
        }

        DateTime GetDateFromTrackingRecord(IDictionary<string, object> data)
        {
            return Convert.ToDateTime(data["Date"], CultureInfo.InvariantCulture);
        }

        string GetStringFromTrackingRecord(string key, IDictionary<string, object> data)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                return data[key].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        IEnumerable<object> IWorkflowInstanceExtension.GetAdditionalExtensions()
        {
            return new object[] { this.persistenceParticipant };
        }

        public void SetInstance(WorkflowInstanceProxy instance)
        {
        }

        class HistorySavePersistenceParticipant : PersistenceIOParticipant
        {
            HistoryFileTrackingParticipant Participant { get; set; }

            public HistorySavePersistenceParticipant(HistoryFileTrackingParticipant participant)
                : base(true, false)
            {
                this.Participant = participant;
            }
            

            protected override IAsyncResult BeginOnSave(IDictionary<System.Xml.Linq.XName, object> readWriteValues, IDictionary<System.Xml.Linq.XName, object> writeOnlyValues, TimeSpan timeout, AsyncCallback callback, object state)
            {
                this.Participant.Save();
                return base.BeginOnSave(readWriteValues, writeOnlyValues, timeout, callback, state);
            }

            protected override void EndOnSave(IAsyncResult result)
            {
            }

            protected override void Abort()
            {
            }
        }
    }
}
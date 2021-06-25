//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Microsoft.Samples.SqlTracking
{

    public class SqlTrackingParticipant : TrackingParticipant
    {

        string connectionString;

        public SqlTrackingParticipant()
        {
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = GetConnectionString(value);
            }
        }


        internal static String GetConnectionString(String rawConnectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                        {
                            ConnectionString = rawConnectionString,
                            AsynchronousProcessing = true,
                            Enlist = false
                        };
            return builder.ToString();
        }


        // The track method is called when a tracking record is emitted by the workflow runtime
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            TrackingCommand trackingCommand = new TrackingCommand(record);

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    foreach (SqlParameter parameter in trackingCommand.Parameters)
                    {
                        sqlCommand.Parameters.Add(parameter);
                    }

                    int sqlTimeout = (int)timeout.TotalSeconds;
                    if (sqlTimeout > 0)
                    {
                        sqlCommand.CommandTimeout = sqlTimeout;
                    }

                    // Wrap the command in a transaction since the command may contain multiple statements. 
                    sqlCommand.Transaction = sqlConnection.BeginTransaction();
                    sqlCommand.CommandText = trackingCommand.Procedure;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Transaction.Commit();               
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, "SqlTrackingParticipant: Exception in Track {0}", e.StackTrace));
            }
        }
    }


    class TrackingCommand
    {

        List<SqlParameter> parameters;
        NetDataContractSerializer dataSerializer;

        public string Procedure { get; protected set; }
        IDictionary<string, object> NameValueDictionary { get; set; }

        public TrackingCommand()
        {
            parameters = new List<SqlParameter>();
            dataSerializer = new NetDataContractSerializer();
        }

        public TrackingCommand(TrackingRecord record)
            : this()
        {
            ActivityScheduledRecord activityScheduledRecord = record as ActivityScheduledRecord;
            if (activityScheduledRecord != null)
            {
                CreateTrackingCommand(activityScheduledRecord);
                return;
            }

            CancelRequestedRecord cancelRequestedRecord = record as CancelRequestedRecord;
            if (cancelRequestedRecord != null)
            {
                CreateTrackingCommand(cancelRequestedRecord);
                return;
            }

            FaultPropagationRecord faultPropagationRecord = record as FaultPropagationRecord;
            if (faultPropagationRecord != null)
            {
                CreateTrackingCommand(faultPropagationRecord);
                return;
            }


            ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
            if (activityStateRecord != null)
            {
                CreateTrackingCommand(activityStateRecord);
                return;
            }

            WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
            if (workflowInstanceRecord != null)
            {
                CreateTrackingCommand(workflowInstanceRecord);
                return;
            }

            BookmarkResumptionRecord bookmarkResumptionRecord = record as BookmarkResumptionRecord;
            if (bookmarkResumptionRecord != null)
            {
                CreateTrackingCommand(bookmarkResumptionRecord);
                return;
            }

            CustomTrackingRecord customTrackingRecord = record as CustomTrackingRecord;
            if (customTrackingRecord != null)
            {
                CreateTrackingCommand(customTrackingRecord);
                return;
            }

        }

        public SqlParameter[] Parameters
        {
            get { return this.parameters.ToArray(); }
        }

        SqlParameter CreateTrackingCommandParameter(string name, SqlDbType type,
            int? size, object value)
        {
            SqlParameter parameter;

            if (size.HasValue)
            {
                parameter = new SqlParameter(name, type, size.Value);
            }
            else
            {
                parameter = new SqlParameter(name, type);
            }

            if (value == null && type != SqlDbType.Structured)
            {
                parameter.IsNullable = true;
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }

            return parameter;

        }


        void CreateTrackingCommand(WorkflowInstanceRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertWorkflowInstanceEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowActivityDefinition", SqlDbType.NVarChar, 256, record.ActivityDefinitionId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@State", SqlDbType.NVarChar, 128, record.State));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));
           
            if (record is WorkflowInstanceUnhandledExceptionRecord)
            {
                this.parameters.Add(CreateTrackingCommandParameter("@ExceptionDetails", SqlDbType.NVarChar, null, ((WorkflowInstanceUnhandledExceptionRecord)record).UnhandledException.ToString()));
            }
            else
            {
                this.parameters.Add(CreateTrackingCommandParameter("@ExceptionDetails", SqlDbType.NVarChar, null, DBNull.Value));
            }

            if (record is WorkflowInstanceTerminatedRecord)
            {
                this.parameters.Add(CreateTrackingCommandParameter("@Reason", SqlDbType.NVarChar, null, ((WorkflowInstanceTerminatedRecord)record).Reason));
            }
            else
            {
                if (record is WorkflowInstanceAbortedRecord)
                {
                    this.parameters.Add(CreateTrackingCommandParameter("@Reason", SqlDbType.NVarChar, null, ((WorkflowInstanceAbortedRecord)record).Reason));
                }
                else if (record is WorkflowInstanceSuspendedRecord)
                {
                    this.parameters.Add(CreateTrackingCommandParameter("@Reason", SqlDbType.NVarChar, null, ((WorkflowInstanceSuspendedRecord)record).Reason));
                }
                else
                {
                    this.parameters.Add(CreateTrackingCommandParameter("@Reason", SqlDbType.NVarChar, null, DBNull.Value));
                }
            }
        }

        void CreateTrackingCommand(ActivityStateRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertActivityInstanceEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@State", SqlDbType.NVarChar, 128, record.State));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityRecordType", SqlDbType.NVarChar, 128, "ActivityState"));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityName", SqlDbType.NVarChar, 256, record.Activity.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityId", SqlDbType.NVarChar, 256, record.Activity.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityInstanceId", SqlDbType.NVarChar, 256, record.Activity.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityType", SqlDbType.NVarChar, 2048, record.Activity.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@ArgumentsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Arguments)));
            this.parameters.Add(CreateTrackingCommandParameter("@VariablesXml", SqlDbType.NVarChar, null, this.SerializeData(record.Variables)));        
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));

        }

        void CreateTrackingCommand(ActivityScheduledRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertActivityScheduledEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityRecordType", SqlDbType.NVarChar, 128, "ActivityScheduled"));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityName", SqlDbType.NVarChar, 1024, record.Activity == null ? null : record.Activity.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityId", SqlDbType.NVarChar, 256, record.Activity == null ? null : record.Activity.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityInstanceId", SqlDbType.NVarChar, 256, record.Activity == null ? null: record.Activity.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityType", SqlDbType.NVarChar, 2048, record.Activity == null ? null : record.Activity.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityName", SqlDbType.NVarChar, 1024, record.Child.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityId", SqlDbType.NVarChar, 256, record.Child.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityInstanceId", SqlDbType.NVarChar, 256, record.Child.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityType", SqlDbType.NVarChar, 2048, record.Child.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));          
        }

        void CreateTrackingCommand(CancelRequestedRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertActivityCancelRequestedEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityRecordType", SqlDbType.NVarChar, 128, "ActivityScheduled"));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityName", SqlDbType.NVarChar, 1024, record.Activity == null ? string.Empty : record.Activity.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityId", SqlDbType.NVarChar, 256, record.Activity == null ? string.Empty : record.Activity.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityInstanceId", SqlDbType.NVarChar, 256, record.Activity == null ? string.Empty : record.Activity.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityType", SqlDbType.NVarChar, 2048, record.Activity == null ? string.Empty : record.Activity.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityName", SqlDbType.NVarChar, 1024, record.Child.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityId", SqlDbType.NVarChar, 256, record.Child.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityInstanceId", SqlDbType.NVarChar, 256, record.Child.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ChildActivityType", SqlDbType.NVarChar, 2048, record.Child.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));
            
        }

        void CreateTrackingCommand(FaultPropagationRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertFaultPropagationEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityRecordType", SqlDbType.NVarChar, 128, "FaultPropagation"));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityName", SqlDbType.NVarChar, 256, record.FaultSource.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityId", SqlDbType.NVarChar, 256, record.FaultSource.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityInstanceId", SqlDbType.NVarChar, 256, record.FaultSource.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityType", SqlDbType.NVarChar, 2048, record.FaultSource.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@FaultDetails", SqlDbType.NVarChar, null, record.Fault.ToString()));
            this.parameters.Add(CreateTrackingCommandParameter("@FaultHandlerActivityName", SqlDbType.NVarChar, null, record.FaultHandler == null ? null: record.FaultHandler.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@FaultHandlerActivityId", SqlDbType.NVarChar, 256, record.FaultHandler == null ? null : record.FaultHandler.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@FaultHandlerActivityInstanceId", SqlDbType.NVarChar, 256, record.FaultHandler == null ? null : record.FaultHandler.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@FaultHandlerActivityType", SqlDbType.NVarChar, 2048, record.FaultHandler == null ? null : record.FaultHandler.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));
         
        }

        void CreateTrackingCommand(BookmarkResumptionRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertBookmarkResumptionEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@BookmarkName", SqlDbType.NVarChar, 1024, record.BookmarkName));
            this.parameters.Add(CreateTrackingCommandParameter("@BookmarkScope", SqlDbType.UniqueIdentifier, null, record.BookmarkScope));
            this.parameters.Add(CreateTrackingCommandParameter("@OwnerActivityName", SqlDbType.NVarChar, 256, record.Owner.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@OwnerActivityId", SqlDbType.NVarChar, 256, record.Owner.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@OwnerActivityInstanceId", SqlDbType.NVarChar, 256, record.Owner.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@OwnerActivityType", SqlDbType.NVarChar, 2048, record.Owner.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));

        }

        void CreateTrackingCommand(CustomTrackingRecord record)
        {
            this.Procedure = "[Microsoft.Samples.Tracking].[InsertCustomTrackingEvent]";

            this.parameters.Add(CreateTrackingCommandParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, null, record.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@RecordNumber", SqlDbType.BigInt, null, record.RecordNumber));
            this.parameters.Add(CreateTrackingCommandParameter("@TraceLevelId", SqlDbType.TinyInt, null, record.Level));
            this.parameters.Add(CreateTrackingCommandParameter("@CustomRecordName", SqlDbType.NVarChar, 1024, record.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityName", SqlDbType.NVarChar, 256, record.Activity.Name));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityId", SqlDbType.NVarChar, 256, record.Activity.Id));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityInstanceId", SqlDbType.NVarChar, 256, record.Activity.InstanceId));
            this.parameters.Add(CreateTrackingCommandParameter("@ActivityType", SqlDbType.NVarChar, 2048, record.Activity.TypeName));
            this.parameters.Add(CreateTrackingCommandParameter("@CustomRecordDataXml", SqlDbType.NVarChar, null, this.SerializeData(record.Data)));
            this.parameters.Add(CreateTrackingCommandParameter("@AnnotationsXml", SqlDbType.NVarChar, null, this.SerializeData(record.Annotations)));
            this.parameters.Add(CreateTrackingCommandParameter("@TimeCreated", SqlDbType.DateTime, null, record.EventTime));    

        }

        
        string SerializeData<TKey, TValue>(IDictionary<TKey, TValue> data)
        {          
            if (data.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                if (this.dataSerializer == null)
                {
                    this.dataSerializer = new NetDataContractSerializer();
                }
                try
                {
                    this.dataSerializer.WriteObject(writer, data);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, "Exception during serialization of data: {0}", e.Message));
                }

                writer.Flush();
                return builder.ToString();
            }
        }

    }

}

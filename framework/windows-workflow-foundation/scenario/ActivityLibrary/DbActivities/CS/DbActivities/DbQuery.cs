//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Windows.Markup;

namespace Microsoft.Samples.Activities.Data
{

    /// <summary>
    /// Query the database and returns a list of objects of type TResult
    /// </summary>    
    public class DbQuery<TResult> : NativeActivity<IList<TResult>>
    {
        // private variables
        IDictionary<string, Argument> parameters;
        DbHelper dbHelper;
        CompletionCallback<TResult> onRowMappingCompleted;
        DbDataReader reader;
        Variable<NoPersistHandle> noPersistHandle;

        // public arguments
        [RequiredArgument]
        [OverloadGroup("ConnectionString")]        
        [DefaultValue(null)]
        public InArgument<string> ProviderName { get; set; }

        [RequiredArgument]
        [OverloadGroup("ConnectionString")]
        [DependsOn("ProviderName")]
        [DefaultValue(null)]
        public InArgument<string> ConnectionString { get; set; }

        [RequiredArgument]
        [OverloadGroup("ConfigFileSectionName")]
        [DefaultValue(null)]
        public InArgument<string> ConfigName { get; set; }

        [DefaultValue(null)]
        public CommandType CommandType { get; set; }

        [RequiredArgument]
        public InArgument<string> Sql { get; set; }

        [DependsOn("Sql")]
        [DefaultValue(null)]
        public IDictionary<string, Argument> Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new Dictionary<string, Argument>();
                }
                return this.parameters;
            }
        }

        [OverloadGroup("DirectMapping")]
        [DefaultValue(null)]
        public Func<DbDataReader, TResult> Mapper { get; set; }

        [OverloadGroup("MultiplePulseMapping")]
        [DefaultValue(null)]
        public ActivityFunc<DbDataReader, TResult> MapperFunc { get; set; }

        CompletionCallback<TResult> OnRowMappingCompleted
        {
            get
            {
                if (this.onRowMappingCompleted == null)
                {
                    this.onRowMappingCompleted = new CompletionCallback<TResult>(RecordMappingComplete);
                }
                return this.onRowMappingCompleted;  
            }
        }

        public DbQuery(): base()
        {
            this.CommandType = CommandType.Text;
            this.noPersistHandle = new Variable<NoPersistHandle>();
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // add noPersistHandle to the list of private variables
            metadata.AddImplementationVariable(this.noPersistHandle);

            // let the base class handle the rest of the work 
            base.CacheMetadata(metadata);
        }

        protected override void Execute(NativeActivityContext context)
        {
            // setup a no persist zone (don't want to be persisted while querying or mapping values)
            NoPersistHandle noPersistHandle = this.noPersistHandle.Get(context);
            noPersistHandle.Enter(context);            

            // configure the helper object to access the database
            dbHelper = new DbHelper();
            dbHelper.ConnectionString = this.ConnectionString.Get(context);
            dbHelper.ProviderName = this.ProviderName.Get(context);
            dbHelper.ConfigName = this.ConfigName.Get(context);
            dbHelper.Sql = this.Sql.Get(context);
            dbHelper.CommandType = this.CommandType;
            dbHelper.Parameters = this.parameters;
            dbHelper.Init(context);

            // retrieve the data
            this.reader = dbHelper.ExecuteReader();

            // map the results
            if (this.Mapper != null)
            {
                // direct execution (invoke the Mapper function in a single pulse)
                DirectMapping(context);
            }
            else // execute in multiple pulses (each pulse invokes the MapperAction ActivityFunc)
            {
                // initialize results list
                this.Result.Set(context, new List<TResult>());

                // map a record
                InternalMapRecord(context);
            }
        }

        // this is executed every time a record mapping is completed
        void RecordMappingComplete(NativeActivityContext context, ActivityInstance instance, TResult result)
        {
            // add the mapped item to the results collection
            IList<TResult> results = this.Result.Get(context);
            results.Add(result);
            this.Result.Set(context, results);
            
            // map a new record
            this.InternalMapRecord(context);
        }

        // internal mapping function (schedules the ActivityFunc if it founds data in the datareader)
        void InternalMapRecord(NativeActivityContext context)
        {
            // map a record
            if (reader.Read())
            {
                context.ScheduleFunc<DbDataReader, TResult>(this.MapperFunc, this.reader, this.OnRowMappingCompleted);
            }
            else
            {
                reader.Close();
                dbHelper.Dispose();
            }
        }

        // direct mapping (use when Mapper is not null, execution is in a single pulse)
        void DirectMapping(NativeActivityContext context)
        {
            IList<TResult> results = new List<TResult>();
            while (reader.Read())
            {
                results.Add(Mapper(reader));
            }

            // set the results list
            this.Result.Set(context, results);

            // release db objects
            this.reader.Close();
            this.dbHelper.Dispose();
        }
    }
}

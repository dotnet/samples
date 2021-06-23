//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Markup;

namespace Microsoft.Samples.Activities.Data
{

    /// <summary>
    /// Query the database and return a DataSet
    /// </summary>
    public class DbQueryDataSet : AsyncCodeActivity<DataSet>
    {
        // private variables
        IDictionary<string, Argument> parameters;
        DbHelper dbHelper;

        // public arguments
        [OverloadGroup("ConnectionString")]
        [DefaultValue(null)]
        public InArgument<string> ProviderName { get; set; }

        [OverloadGroup("ConnectionString")]
        [DependsOn("ProviderName")]
        [DefaultValue(null)]
        public InArgument<string> ConnectionString { get; set; }

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

        public DbQueryDataSet()
        {
            this.CommandType = CommandType.Text;
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            // configure the helper object to access the database
            dbHelper = new DbHelper();
            dbHelper.ConnectionString = this.ConnectionString.Get(context);
            dbHelper.ProviderName = this.ProviderName.Get(context);
            dbHelper.ConfigName = this.ConfigName.Get(context);
            dbHelper.Sql = this.Sql.Get(context);
            dbHelper.CommandType = this.CommandType;
            dbHelper.Parameters = this.parameters;
            dbHelper.Init(context);

            // create the action for doing the actual work
            Func<DataSet> action = () => dbHelper.GetDataSet();
            context.UserState = action;

            return action.BeginInvoke(callback, state);
        }

        protected override DataSet EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<DataSet> action = (Func<DataSet>)context.UserState;
            DataSet dataSet = action.EndInvoke(result);

            // dispose the database connection
            dbHelper.Dispose();

            // return the state
            return dataSet;
        }
    }    
}

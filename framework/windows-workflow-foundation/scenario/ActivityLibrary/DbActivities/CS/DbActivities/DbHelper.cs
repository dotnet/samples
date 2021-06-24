//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace Microsoft.Samples.Activities.Data
{

    /// <summary>
    /// Helper class with basic functionality to handle data access. 
    /// This class is used by all DB activities (these activities don't use ADO.NET directly)
    /// </summary>
    internal class DbHelper : IDisposable
    {
        static Dictionary<ArgumentDirection, ParameterDirection> wokflowParameterToDbParameterDirectionMap;
        static Dictionary<ArgumentDirection, ParameterDirection> WokflowParameterDirectionToDbParameterMap
        {
            get
            {
                if (wokflowParameterToDbParameterDirectionMap == null)
                {
                    // Update mapEntryCount if an entry is added/removed from the map.
                    const int mapEntryCount = 3;
                    Dictionary<ArgumentDirection, ParameterDirection> map = new Dictionary<ArgumentDirection, ParameterDirection>(mapEntryCount);

                    map.Add(ArgumentDirection.In, ParameterDirection.Input);
                    map.Add(ArgumentDirection.InOut, ParameterDirection.InputOutput);
                    map.Add(ArgumentDirection.Out, ParameterDirection.Output);
                    wokflowParameterToDbParameterDirectionMap = map;
                }
                return wokflowParameterToDbParameterDirectionMap;
            }
        }

        // private members
        DbProviderFactory providerFactory;
        DbConnection connection;
        DbCommand command;

        // public members
        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }
        public string ConfigName { get; set; }
        public string Sql { get; set; }
        public CommandType CommandType { get; set; }
        public Transaction Transaction { get; private set; }
        public IDictionary<string, Argument> Parameters { get; set; }

        public DbHelper()
        {
            this.CommandType = CommandType.Text;
        }   

        public void Init(AsyncCodeActivityContext context)
        {
            // initialize internal objects
            this.Init();

            // set the transaction (from the ambient transaction)
            this.GetTransactionFromContext(context);

            // set parameters
            this.ArgumentsToDbParameters(context, this.Parameters);
        }

        public void Init(NativeActivityContext context)
        {
            // initialize internal objects
            this.Init();

            // set the transaction (from the ambient transaction)
            this.GetTransactionFromContext(context);

            // set parameters
            this.ArgumentsToDbParameters(context, this.Parameters);
        }

        // execute a query
        public int Execute()
        {
            this.OpenConnectionAndSetupCommand();
            return command.ExecuteNonQuery();
        }

        // execute a query and return a single value
        public TResult ExecuteScalar<TResult>()
        {
            this.OpenConnectionAndSetupCommand();
            return (TResult)command.ExecuteScalar();
        }

        // execute a query and return a DbDataReader
        public DbDataReader ExecuteReader()
        {
            this.OpenConnectionAndSetupCommand();
            return command.ExecuteReader();
        }

        public DataSet GetDataSet()
        {
            this.OpenConnectionAndSetupCommand();

            DataSet ds = new DataSet();
            DbDataAdapter adapter = providerFactory.CreateDataAdapter();
            adapter.SelectCommand = command;            
            adapter.Fill(ds);
            return ds;
        }

        // free resources used by this class (IDisposable)
        public void Dispose()
        {
            if (this.command != null)
            {
                command.Dispose();
            }

            if (this.connection != null)
            {
                this.connection.Dispose();
            }
        }

        // open the connection and setup the command attributes
        void OpenConnectionAndSetupCommand()
        {
            if (this.connection.State != ConnectionState.Open)
            {
                this.connection.ConnectionString = this.ConnectionString;
                this.connection.Open();
                this.command.Connection = this.connection;
            }

            if (this.Transaction != null)
            {
                this.connection.EnlistTransaction(this.Transaction);
            }

            this.command.CommandText = this.Sql;
            this.command.CommandType = this.CommandType;
        }

        void Init()
        {
            if (string.IsNullOrEmpty(ConfigName))
            {
                this.ConnectionString = this.ConnectionString;
                this.ProviderName = this.ProviderName;
            }
            else
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[this.ConfigName];
                if (settings == null)
                {
                    throw new ValidationException(string.Format("Invalid connection string config name ({0})", this.ConfigName));
                }
                else
                {
                    this.ConnectionString = settings.ConnectionString;
                    this.ProviderName = settings.ProviderName;
                }
            }

            this.providerFactory = DbProviderFactories.GetFactory(this.ProviderName);
            this.connection = providerFactory.CreateConnection();
            this.command = providerFactory.CreateCommand();
        }

        void GetTransactionFromContext(AsyncCodeActivityContext context)
        {            
            RuntimeTransactionHandle transactionHandle = context.GetProperty<RuntimeTransactionHandle>();
            if (transactionHandle != null)
            {
                this.Transaction = transactionHandle.GetCurrentTransaction(context);
            }
        }

        void GetTransactionFromContext(NativeActivityContext context)
        {
            RuntimeTransactionHandle transactionHandle = context.Properties.Find(typeof(RuntimeTransactionHandle).FullName) as RuntimeTransactionHandle;
            if (transactionHandle != null)
            {
                this.Transaction = transactionHandle.GetCurrentTransaction(context);
            }
        }

        void ArgumentsToDbParameters(ActivityContext context, IDictionary<string, Argument> arguments)
        {
            foreach (KeyValuePair<string, Argument> parameter in arguments)
            {
                DbParameter dbParameter = providerFactory.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Direction = WokflowParameterDirectionToDbParameterMap[parameter.Value.Direction];
                dbParameter.Value = parameter.Value.Get<object>(context);
                this.command.Parameters.Add(dbParameter);
            }
        }
    }
}

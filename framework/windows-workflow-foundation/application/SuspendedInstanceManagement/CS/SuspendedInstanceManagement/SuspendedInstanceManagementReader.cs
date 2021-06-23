//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Data.SqlClient;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class SuspendedInstanceManagementReader
    {
        readonly string ServerParameterName = "Server";
        readonly string DatabaseParameterName = "Database";
        readonly string CommandTypeParameterName = "Command";
        readonly string InstanceIdParameterName = "InstanceId";

        public InstanceCommandType InstanceCommandType { get; private set; }
        public string Server { get; private set; }
        public string Database { get; private set; }
        public string ConnectionString { get; private set; }
        public string InstanceId { get; private set; }

        public SuspendedInstanceManagementReader(string[] args, string delimiter, char mediaDelimiter)
        {
            if ( (args == null) || (delimiter == null) )
            {
                throw new ArgumentNullException((args == null) ? "args" : "delimiter");
            }

            ReadCommandLineArgs(args, delimiter, mediaDelimiter);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder()
            {
                DataSource = this.Server,
                InitialCatalog = this.Database,
                IntegratedSecurity = true
            };

            this.ConnectionString = builder.ConnectionString;
        }

        void ReadCommandLineArgs(string[] args, string delimiter, char mediaDelimiter)
        {
            CommandLineParser parser = new CommandLineParser();
            parser.ParseCommandLineArguments(args, delimiter, mediaDelimiter);

            string parameterOutput = parser.LookupArgument(ServerParameterName);
            if (!string.IsNullOrEmpty(parameterOutput))
            {
                this.Server = parameterOutput;
            }
            else
            {
                throw new ArgumentNullException(ServerParameterName);
            }

            parameterOutput = parser.LookupArgument(DatabaseParameterName);
            if (!string.IsNullOrEmpty(parameterOutput))
            {
                this.Database = parameterOutput;
            }
            else
            {
                throw new ArgumentNullException(DatabaseParameterName);
            }

            parameterOutput = parser.LookupArgument(CommandTypeParameterName);
            if (!string.IsNullOrEmpty(parameterOutput))
            {
                switch (parameterOutput.ToUpperInvariant())
                {
                    case "QUERY":
                        this.InstanceCommandType = InstanceCommandType.Query;
                        break;

                    case "RESUME":
                        this.InstanceCommandType = InstanceCommandType.Resume;
                        break;

                    case "TERMINATE":
                        this.InstanceCommandType = InstanceCommandType.Terminate;
                        break;

                    default:
                        this.InstanceCommandType = InstanceCommandType.None;
                        break;
                }
            }
            else
            {
                throw new ArgumentNullException(CommandTypeParameterName);
            }

            parameterOutput = parser.LookupArgument(InstanceIdParameterName);
            if (!string.IsNullOrEmpty(parameterOutput))
            {
                this.InstanceId = parameterOutput;
            }
        }
    }
}
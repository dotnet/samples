//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using System.Workflow.Activities.Rules;
using System.Workflow.Runtime.Hosting;
using Microsoft.Samples.Rules.ExternalRuleSetLibrary;

namespace Microsoft.Samples.Rules.ExternalRuleSetService
{
    public class ExternalRuleSetService : WorkflowRuntimeService
    {
        public ExternalRuleSetService()
        {
            ConnectionStringSettingsCollection connectionStringSettingsCollection = ConfigurationManager.ConnectionStrings;

            foreach (ConnectionStringSettings connectionStringSettings in connectionStringSettingsCollection)
            {
                if (string.CompareOrdinal(connectionStringSettings.Name, "RuleSetStoreConnectionString") == 0)
                    connectionString = connectionStringSettings.ConnectionString;
            }

            if (connectionString == null)
                throw new ConfigurationErrorsException("SQL connection string not available for the (should be provided in the config file).");
        }

        string connectionString;

        public RuleSet GetRuleSet(RuleSetInfo ruleSetInfo)
        {
            if (ruleSetInfo != null)
            {
                SqlConnection sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                string commandString;

                // If both the major and minor are 0, it is assumed that a specific version is not being requested.
                bool specificVersionRequested = !(ruleSetInfo.MajorVersion == 0 && ruleSetInfo.MinorVersion == 0);  

                if (specificVersionRequested)
                {
                    commandString = String.Format(CultureInfo.InvariantCulture, "SELECT TOP 1 * FROM RuleSet WHERE Name=@name AND MajorVersion={0} AND MinorVersion={1} ORDER BY MajorVersion DESC, MinorVersion DESC", ruleSetInfo.MajorVersion, ruleSetInfo.MinorVersion);
                }
                else
                {
                    commandString = "SELECT TOP 1 * FROM RuleSet WHERE Name=@name ORDER BY MajorVersion DESC , MinorVersion DESC";
                }
                SqlCommand command = new SqlCommand(commandString, sqlConn);
                command.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 128);
                command.Parameters["@name"].Value = ruleSetInfo.Name;

                SqlDataReader reader = command.ExecuteReader();

                RuleSetData data = null;

                if (reader.HasRows)
                {
                    reader.Read();

                    try
                    {
                        data = new RuleSetData();
                        data.Name = reader.GetString(0);
                        data.OriginalName = data.Name; // will be used later to see if one of these key values changed                       
                        data.MajorVersion = reader.GetInt32(1);
                        data.OriginalMajorVersion = data.MajorVersion;
                        data.MinorVersion = reader.GetInt32(2);
                        data.OriginalMinorVersion = data.MinorVersion;

                        data.RuleSetDefinition = reader.GetString(3);
                        data.Status = reader.GetInt16(4);
                        data.AssemblyPath = reader.GetString(5);
                        data.ActivityName = reader.GetString(6);
                        data.ModifiedDate = reader.GetDateTime(7);
                        data.Dirty = false;
                    }
                    catch (InvalidCastException)
                    {
                        MessageBox.Show("Error parsing table row", "RuleSet Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                sqlConn.Close();

                if (data != null)
                    return data.RuleSet;
                else if (specificVersionRequested)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Specified RuleSet version does not exist: '{0}'", ruleSetInfo.ToString())); //could use a custom exception type here
                else
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "No RuleSets exist with this name: '{0}'", ruleSetInfo.Name));
            }
            else
            {
                return null;
            }
        }
    }
}

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.Samples.ContosoHR
{
    // Repository of request history records (can be used by any request type)
    public static class RequestHistoryRepository
    {             
        // Save history for a HiringRequest
        public static void Save(RequestHistoryRecord requestHistory)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SaveRequestHistory";
                
                command.Parameters.Add(new SqlParameter("@requestId", requestHistory.RequestId));
                command.Parameters.Add(new SqlParameter("@sourceState", requestHistory.SourceState));
                command.Parameters.Add(new SqlParameter("@actionName", requestHistory.Action));
                command.Parameters.Add(new SqlParameter("@comment", requestHistory.Comment));
                command.Parameters.Add(new SqlParameter("@employeeId", requestHistory.EmployeeId));
                command.Parameters.Add(new SqlParameter("@employeeName", requestHistory.EmployeeName));                
                command.ExecuteNonQuery();
            }
        }

        // Loads the history of interactions of a particular Request
        public static IList<RequestHistoryRecord> GetRequestHistory(string requestId)
        {
            IList<RequestHistoryRecord> requestHistory = new List<RequestHistoryRecord>();
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SelectHiringRequestHistory";
                command.Parameters.Add(new SqlParameter("@id", requestId));

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while(reader.Read())
                    {
                        requestHistory.Add(
                            new RequestHistoryRecord
                            {
                                RequestId = GetStringValue(reader, "RequestId"),
                                Action = GetStringValue(reader, "Action"),
                                SourceState = GetStringValue(reader, "SourceState"),
                                Comment = GetStringValue(reader, "comment"),
                                Date = DateTime.Parse(reader["Date"].ToString(), new CultureInfo("EN-us")),
                                EmployeeId = GetStringValue(reader, "employeeId"),
                                EmployeeName = GetStringValue(reader, "employeeName"),
                            });
                    }
                }

                return requestHistory;
            }                 
        }

        static string GetStringValue(SqlDataReader reader, string fieldName)
        {
            return reader[fieldName] is DBNull ? "" : reader[fieldName].ToString();
        }      
    }    
}
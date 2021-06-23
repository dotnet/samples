//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Samples.ContosoHR
{
    // Repository of hiring request data
    public static class HiringRequestRepository
    {        
        // Save a hiringRequest
        public static void Save(HiringRequestInfo hiringRequestInfo)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SaveHiringRequest";

                command.Parameters.Add(new SqlParameter("@id", hiringRequestInfo.Id));
                command.Parameters.Add(new SqlParameter("@requesterId", hiringRequestInfo.RequesterId));
                command.Parameters.Add(new SqlParameter("@creationDate", hiringRequestInfo.CreationDate));
                command.Parameters.Add(new SqlParameter("@positionId", hiringRequestInfo.PositionId));
                command.Parameters.Add(new SqlParameter("@departmentId", hiringRequestInfo.DepartmentId));
                command.Parameters.Add(new SqlParameter("@description", hiringRequestInfo.Description));
                command.Parameters.Add(new SqlParameter("@title", hiringRequestInfo.Title));
                command.Parameters.Add(new SqlParameter("@workflowInstanceId", hiringRequestInfo.WorkflowInstanceId));
                command.Parameters.Add(new SqlParameter("@isCompleted", hiringRequestInfo.IsCompleted ? 1 : 0));
                command.Parameters.Add(new SqlParameter("@isSuccess", hiringRequestInfo.IsSuccess ? 1: 0 ));
                command.Parameters.Add(new SqlParameter("@isCancelled", hiringRequestInfo.IsCancelled ? 1: 0));

                command.ExecuteNonQuery();
            }
        }

        // Loads the data of a HiringRequest from the database
        public static HiringRequestInfo Load(string hiringRequestId)
        {
             string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

             using (SqlConnection connection = new SqlConnection(connectionString))
             {
                 connection.Open();
                 SqlCommand command = connection.CreateCommand();
                 command.CommandType = CommandType.StoredProcedure;
                 command.CommandText = "SelectHiringRequest";
                 command.Parameters.Add(new SqlParameter("@id", hiringRequestId));

                 using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                 {
                     if (reader.Read())
                     {
                         return new HiringRequestInfo
                         {
                             Id = new Guid(reader["Id"].ToString()),
                             RequesterId = reader["RequesterId"].ToString(),
                             DepartmentId = reader["DepartmentId"].ToString(),
                             PositionId = reader["PositionId"].ToString(),
                             Description = reader["Description"].ToString(),
                             Title = reader["Title"].ToString(),
                             IsSuccess = (bool)reader["IsSuccess"],
                             IsCompleted = (bool)reader["IsCompleted"],
                             IsCancelled = (bool)reader["IsCancelled"],
                             WorkflowInstanceId = reader["WorkflowInstanceId"] is DBNull ? Guid.Empty : new Guid(reader["WorkflowInstanceId"].ToString())
                         };
                     }
                     else
                     {
                         throw new ArgumentException(string.Format("Invalid argument. No data available for Hiring Request {0}", hiringRequestId));
                     }                     
                 }
             }            
        }       
    }    
}

//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Samples.ContosoHR
{
    public static class JobPostingRepository
    {
        public static void Save(JobPosting jobPosting)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SaveJobPosting";

                command.Parameters.Add(new SqlParameter("@id", jobPosting.Id));
                command.Parameters.Add(new SqlParameter("@hiringRequestId", jobPosting.HiringRequestInfo.Id));
                command.Parameters.Add(new SqlParameter("@title", jobPosting.Title));
                command.Parameters.Add(new SqlParameter("@description", jobPosting.Description));
                command.Parameters.Add(new SqlParameter("@status", jobPosting.Status));

                command.ExecuteNonQuery();
            }
        }

        public static void IncrementResumesCount(Guid jobPostingId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "UpdateResumeesCountInJobPosting";
                command.Parameters.Add(new SqlParameter("@id", jobPostingId));
                command.ExecuteNonQuery();
            }
        }

        public static void InsertResume(JobPostingResume resumee)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "InsertJobPostingResumee";
                command.Parameters.Add(new SqlParameter("@jobPostingId", resumee.JobPosting.Id));
                command.Parameters.Add(new SqlParameter("@candidateMail", resumee.CandidateMail));
                command.Parameters.Add(new SqlParameter("@candidateName", resumee.CandidateName));
                command.Parameters.Add(new SqlParameter("@resumee", resumee.ResumeeText));                

                command.ExecuteNonQuery();
            }
        }

        public static JobPosting Load(string id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SelectJobPosting";
                command.Parameters.Add(new SqlParameter("@id", id));

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {                    
                    if (reader.Read())
                    {
                        //Id = new Guid(reader["Id"].ToString()),
                        return new JobPosting
                        {
                            Id = new Guid(reader["Id"].ToString()),
                            //HiringRequestInfo = HiringRequestReposinew Guid(reader["Id"].ToString()),
                            CreationDate = (DateTime)reader["CreationDate"],                         
                            Description = reader["Description"].ToString(),
                            Title = reader["Title"].ToString(),
                        };
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Invalid argument. No data available for Job Posting {0}", id));
                    }
                }
            }            
        }

        public static DataSet SelectActiveJobPostings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();                
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SelectActiveJobPostings";

                //Create a SqlDataAdapter for the Suppliers table.
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                // Fill the DataSet.
                DataSet dataSet = new DataSet("JobOpenings");
                adapter.Fill(dataSet);

                return dataSet;
            }
        }

        public static DataSet SelectNotStartedJobPostings()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SelectNotStartedJobPostings";
                
                //Create a SqlDataAdapter for the Suppliers table.
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                // Fill the DataSet.
                DataSet dataSet = new DataSet("JobOpenings");
                adapter.Fill(dataSet);

                return dataSet;
            }
        }

        public static DataSet SelectClosedJobPostings()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SelectClosedJobPostings";

                //Create a SqlDataAdapter for the Suppliers table.
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;

                // Fill the DataSet.
                DataSet dataSet = new DataSet("JobOpenings");
                adapter.Fill(dataSet);

                return dataSet;
            }
        }
    }
}
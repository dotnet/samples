//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.Samples.Inbox
{    
    public class InboxService : IInboxService
    {
        string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ContosoHR"].ConnectionString;
            }
        }

        // register a request in the inbox for a user
        public void Add(string requestId, string title, string state, string requestCreatorId, string userId)
        {
            Console.WriteLine(string.Format("Registering {0} {1} {2} {3}", requestId, title, state, userId));            

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "InsertInEmployeeInbox";
                
                command.Parameters.Add(new SqlParameter("@requestId", new Guid(requestId)));
                command.Parameters.Add(new SqlParameter("@startedBy", requestCreatorId));
                command.Parameters.Add(new SqlParameter("@title", title));
                command.Parameters.Add(new SqlParameter("@state", state));
                command.Parameters.Add(new SqlParameter("@employeeId", userId));
                command.ExecuteNonQuery();
            }            
        }

        // remove a request from the inbox (for a particular user or the full request when userId = null)
        public void Remove(string requestId, string userId = null)
        {
            Console.WriteLine(string.Format("Removing {0} {1}", requestId, userId));            

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (userId == null)
                {
                    command.CommandText = "RemoveFromInbox";
                    command.Parameters.Add(new SqlParameter("@requestId", requestId));                    
                }
                else
                {
                    command.CommandText = "RemoveFromEmployeeInbox";
                    command.Parameters.Add(new SqlParameter("@requestId", requestId));
                    command.Parameters.Add(new SqlParameter("@employeeId", userId));
                }
                command.ExecuteNonQuery();
            }                     
        }

        // archive a request
        public void Archive(string requestId, string title, string state, string requestCreatorId)
        {
            Console.WriteLine(string.Format("Archiving {0} {1} {2}", requestId, title, state));            

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "ArchiveInboxRequest";
                command.Parameters.Add(new SqlParameter("@requestId", requestId));
                command.Parameters.Add(new SqlParameter("@state", state));
                command.ExecuteNonQuery();
            }                          
        }

        // get all requests where a user is expected to act
        public IList<InboxItem> GetRequestsFor(string userId)
        {
            Console.WriteLine(string.Format("Get Requests for {0}", userId));

            IList<InboxItem> inboxItems = new List<InboxItem>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SelectRequestsFor";
                command.Parameters.Add(new SqlParameter("@employeeId", userId));

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        inboxItems.Add(MapReaderToInboxItem(reader));
                    }
                }

                return inboxItems;
            }             
        }

        // get all requests started by a user
        public IList<InboxItem> GetRequestsStartedBy(string userId)
        {
            Console.WriteLine(string.Format("Get Requests Started By {0}", userId));            

            IList<InboxItem> inboxItems = new List<InboxItem>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SelectRequestsStartedBy";
                command.Parameters.Add(new SqlParameter("@employeeId", userId));

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        inboxItems.Add(MapReaderToInboxItem(reader));
                    }
                }

                return inboxItems;
            }            
        }

        // get all archived requests started by a user
        public IList<InboxItem> GetArchivedRequestsStartedBy(string userId)
        {
            Console.WriteLine(string.Format("Get Archived Requests Started By {0}", userId));            

            IList<InboxItem> inboxItems = new List<InboxItem>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SelectArchivedRequestsStartedBy";
                command.Parameters.Add(new SqlParameter("@employeeId", userId));

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        inboxItems.Add(MapReaderToInboxItem(reader));
                    }
                }

                return inboxItems;
            }  
        }

        InboxItem MapReaderToInboxItem(SqlDataReader reader)
        {
            return new InboxItem
                            {                                
                                RequestId = reader["requestId"].ToString(),
                                StartedBy = reader["startedBy"].ToString(),
                                Title = reader["Title"].ToString(),
                                State = reader["State"].ToString(),
                                InboxEntryDate = DateTime.Parse(reader["InboxEntryDate"].ToString(), new CultureInfo("EN-us")),
                            };
        }        
    }
}
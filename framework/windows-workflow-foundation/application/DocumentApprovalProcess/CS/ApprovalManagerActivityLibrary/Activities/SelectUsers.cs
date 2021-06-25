//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary.Designers;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary
{
    // Selects existing users known by the appoval manager system.
    //  This code will only return users of a specific type that are not the user
    //  that requested the approval (to prevent a user from being able to approve his/her own
    //  document)
    [Designer(typeof(SelectUsersDesigner))]
    public sealed class SelectUsers : CodeActivity
    {
        public InArgument<User> UserContext { get; set; }
        public InArgument<String> UserType { get; set; }
        public InArgument<int> SelectXUsers { get; set; }
        public InArgument<String> DBConnectionString { get; set; }
        public OutArgument<List<User>> SelectedUsers { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            String dbconnStr = DBConnectionString.Get(context);
            String query = "SELECT * FROM users WHERE usertype='" + this.UserType.Get(context) + "'";
            List<User> filteredUsers = new List<User>();
            DataSet ds = new DataSet();

            if (dbconnStr == null) dbconnStr = ConfigurationManager.ConnectionStrings["ApprovalProcessDB"].ConnectionString;

            using (SqlConnection sCon = new SqlConnection(dbconnStr))
            {
                using (SqlDataAdapter sAda = new SqlDataAdapter(query, sCon))
                {
                    sAda.Fill(ds);
                    if (ds.Tables["Table"] != null)
                    {
                        foreach (DataRow dr in ds.Tables["Table"].Rows)
                        {
                            // Ensure no selection of user from the user requesting approval
                            if (!(new Guid((String)dr["guid"])).Equals(this.UserContext.Get(context).Id))
                            {
                                filteredUsers.Add(new User((String)dr["username"], (String)dr["usertype"], (String)dr["addressrequest"], (String)dr["addressresponse"], (String)dr["guid"]));
                            }
                        }
                    }
                }
            }

            // Only return as many users as is requested
            int remove = filteredUsers.Count - this.SelectXUsers.Get(context);
            for (int i = 0; i < remove; i++) filteredUsers.Remove(filteredUsers.Last());

            this.SelectedUsers.Set(context, filteredUsers);
        }
    }
}

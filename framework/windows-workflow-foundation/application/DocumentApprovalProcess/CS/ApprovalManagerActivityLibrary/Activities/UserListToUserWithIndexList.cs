//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary.Designers;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary
{
    // Adds an index to every User in a list of Users
    //  This is used so that multiple requests can be in flight at the same time so that
    //  each response can be correlated back to the correct RequestApproval activity
    [Designer(typeof(UserListToUserWithIndexListDesigner))]
    public sealed class UserListToUserWithIndexList : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<User>> UserList { get; set; }
        [RequiredArgument]
        public OutArgument<List<UserWithIndex>> UserListWithIndex { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<UserWithIndex> toret = new List<UserWithIndex>();
            int i = 0;

            foreach (User u in UserList.Get(context))
            {
                toret.Add(new UserWithIndex(u, i));
                i++;
            }
            UserListWithIndex.Set(context, toret);
        }
    }
}

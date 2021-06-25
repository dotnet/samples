//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;

using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{
    // This service is how clients opt-in/out of the approval system
    [ServiceContract]
    interface ISubscriptionService
    {
        [OperationContract]
        User Subscribe(User newuser);
        [OperationContract]
        void Unsubscribe(User id);
    }

    class SubscriptionManager : ISubscriptionService
    {

        public User Subscribe(User newuser)
        {
            return UserManager.AddUser(newuser.Name, newuser.Type, newuser.AddressRequest, newuser.AddressResponse);
        }

        public void Unsubscribe(User id)
        {
            UserManager.RemoveUser(id);
        }
    }
}

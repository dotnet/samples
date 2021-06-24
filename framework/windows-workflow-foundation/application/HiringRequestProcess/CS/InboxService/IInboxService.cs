//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Samples.Inbox
{
    [ServiceContract]
    public interface IInboxService
    {
        // register a request in the inbox for a user
        [OperationContract]
        void Add(string requestId, string title, string state, string requestCreatorId, string userId);

        // remove a request from the inbox (for a particular user or the full request when userId = null)
        [OperationContract]
        void Remove(string requestId, string userId = null);

        // get all requests where a user is expected to act
        [OperationContract]
        IList<InboxItem> GetRequestsFor(string userId);

        // get all requests started by a user
        [OperationContract]
        IList<InboxItem> GetRequestsStartedBy(string userId);

        // get all archived requests started by a user
        [OperationContract]
        IList<InboxItem> GetArchivedRequestsStartedBy(string userId);

        // archive a request
        [OperationContract]
        void Archive(string requestId, string title, string state, string requestCreatorId);
    }
}

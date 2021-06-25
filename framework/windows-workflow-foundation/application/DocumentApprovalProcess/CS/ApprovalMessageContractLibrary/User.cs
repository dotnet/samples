//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary
{
    [DataContract]
    public class User
    {
        private String name;
        private String type;
        private String addressResponse;
        private String addressRequest;
        private Guid id;

        // Need default constructor to be serializable
        public User()
        {
        
        }

        public User(String uname, String utype, String endaddressresponse, String endaddressrequest)
        {
            name = uname;
            type = utype;
            addressResponse = endaddressresponse;
            addressRequest = endaddressrequest;
            id = Guid.NewGuid();
        }

        public User(String uname, String utype, String endaddressresponse, String endaddressrequest, Guid uid)
        {
            name = uname;
            type = utype;
            addressResponse = endaddressresponse;
            addressRequest = endaddressrequest;
            id = uid;
        }

        public User(String uname, String utype, String endaddressresponse, String endaddressrequest, String uid)
        {
            name = uname;
            type = utype;
            addressResponse = endaddressresponse;
            addressRequest = endaddressrequest;
            id = new Guid(uid);
        }

        [DataMember]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        [DataMember]
        public String Type
        {
            get { return type; }
            set { type = value; }
        }
        [DataMember]
        public String AddressResponse
        {
            get { return addressResponse; }
            set { addressResponse = value; }
        }
        [DataMember]
        public String AddressRequest
        {
            get { return addressRequest; }
            set { addressRequest = value; }
        }
        [DataMember]
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}
